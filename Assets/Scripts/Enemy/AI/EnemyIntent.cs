using System;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy.AI
{
    /// <summary>
    /// 敌方AI的意图数据组件：挂在每个敌方单位上，存储当前候选意图与被选中意图
    /// 仅包含数据与轻量逻辑（创建/清理/选优），不包含具体决策算法。
    /// </summary>
    public class EnemyIntent : MonoBehaviour
    {
        [Serializable]
        public enum ActionType
        {
            None = 0,
            Wait = 1,
            Move = 2,
            Attack = 3,
            Ability = 4,
            Defend = 5,
            Interact = 6,
            Retreat = 7,
            Heal = 8
        }
        
        [Serializable]
        public enum Status
        {
            Draft = 0,        // 初始候选
            Evaluated = 1,    // 已评分
            Selected = 2,     // 被选中执行
            Executed = 3,     // 已执行
            Aborted = 4       // 放弃
        }

        [Serializable]
        public struct Metric
        {
            public string name;   // 例如 "dps", "risk", "coverBonus"
            public float value;   // 指标值
        }
        
        [Serializable]
        public struct TargetInfo
        {
            public GameObject target;             // 单一目标（可空）
            public Vector2Int cell;               // 单一目标格（可选）
            public bool hasTarget;
            public bool hasCell;
            
            // 扩展：多目标 & AoE
            public List<GameObject> targets;      // 多个单位目标
            public List<Vector2Int> areaCells;    // AoE 影响区域格
            public bool hasMultipleTargets;       // 是否为多目标
            public bool hasArea;                  // 是否为 AoE 区域
        }

        [Serializable]
        public struct PathInfo
        {
            public List<Vector2Int> cells; // 包含起点到终点的路径
            public int cost;                // AP/步数消耗
            public bool reachable;          // 是否可达
        }

        [Serializable]
        public struct EffectEstimate
        {
            public float hitChance;       // 命中概率 [0,1]
            public float critChance;      // 暴击概率 [0,1]
            public float expectedDamage;  // 期望伤害（已考虑命中与暴击）
            public float expectedHeal;    // 期望治疗
            public float killChance;      // 击杀概率 [0,1]
            public float selfRisk;        // 自身风险评分（越大越危险）
            public float ffRisk;          // 友伤风险
            
            // 扩展：地形/掩体等
            public float coverBonus;      // 掩体收益（>0更好）
            public float terrainBonus;    // 地形收益（高地、增益地块）
            public float displacementValue; // 位移/控场价值（击退、拉扯等）
        }

        [Serializable]
        public struct Constraints
        {
            public int minRange;              // 最小射程（格）
            public int maxRange;              // 最大射程（格）
            public bool requiresLineOfSight;  // 需要视线
            public int cooldownRemaining;     // 冷却剩余（回合）
            public bool willBreakStealth;     // 是否会打破潜行
            public int budgetCost;            // 资源/蓝量/弹药消耗（抽象）
        }

        [Serializable]
        public class IntentData
        {
            public string intentId;          // 用于跟踪同一意图（可用Guid.NewGuid().ToString()）
            public ActionType action;        // 动作类型
            public string abilityId;         // 对于 Ability 动作，对应的技能标识
            public TargetInfo target;        // 目标信息（支持单目标/多目标/AoE）
            public PathInfo path;            // 移动路径/到位信息
            public EffectEstimate estimate;  // 效果预估
            public Constraints constraints;  // 约束与资源
            
            public int apCost;               // 行动点消耗
            public int priority;             // 粗粒度优先级（先粗排，再看 utility）
            public float utility;            // 效用分（越高越好）
            public Status status;            // 当前状态
            public string rationale;         // 选中/评分理由，便于调试
            public List<Metric> metrics = new(); // 细分指标列表（Inspector 友好）
            
            // 扩展：多步计划/时间窗
            public string planId;            // 复合计划ID（同一回合内多步动作）
            public int planStepIndex;        // 计划内步骤序号
            public long executeAtTick;       // 期望执行tick（可选）
            public long expiresAtTick;       // 过期tick（超过则作废）
        }

        [Header("Actor")]
        public string actorId;              // 敌方单位唯一标识（可选）
        public GameObject actor;            // 敌方单位对象

        [Header("Intents")]
        public List<IntentData> candidates = new(); // 候选意图池
        public IntentData selected;                 // 当前被选中的意图
        public List<IntentData> selectedPlan = new(); // 多步行动计划
        
        [Header("Ticks/Frames")]
        public long createdTick;            // 创建时间戳（帧/自定义tick）
        public long evaluatedTick;          // 最近评估时间戳

        [Header("Debug View")] 
        public bool debugDraw;      // 是否绘制调试Gizmos
        public Color pathColor = Color.green;
        public Color targetColor = Color.red;
        public float gizmoSphereRadius = 0.15f;
        
        /// <summary>
        /// 由外部设置的网格到世界坐标转换器；若未设置，调试绘制会直接使用 cell.x, cell.y 作为世界坐标。
        /// </summary>
        public static Func<Vector2Int, Vector3> CellToWorldResolver;

        // 工具方法
        public void ClearAll()
        {
            candidates.Clear();
            selected = null;
        }

        public IntentData CreateMoveIntent(Vector2Int destination, List<Vector2Int> path, int apCost, float utility, string rationale = null, Constraints constraints = default, string planId = null, int planStepIndex = 0)
        {
            var intent = new IntentData
            {
                intentId = Guid.NewGuid().ToString(),
                action = ActionType.Move,
                target = new TargetInfo { cell = destination, hasCell = true, hasTarget = false, targets = null, areaCells = null, hasMultipleTargets = false, hasArea = false },
                path = new PathInfo { cells = path ?? new List<Vector2Int>(), cost = apCost, reachable = path != null && path.Count > 0 },
                apCost = apCost,
                utility = utility,
                status = Status.Draft,
                rationale = rationale,
                constraints = constraints,
                planId = planId,
                planStepIndex = planStepIndex
            };
            candidates.Add(intent);
            return intent;
        }

        public IntentData CreateAttackIntent(GameObject target, Vector2Int targetCell, int apCost, EffectEstimate estimate, float utility, string rationale = null, Constraints constraints = default, string planId = null, int planStepIndex = 0)
        {
            var intent = new IntentData
            {
                intentId = Guid.NewGuid().ToString(),
                action = ActionType.Attack,
                target = new TargetInfo { target = target, hasTarget = target != null, cell = targetCell, hasCell = true, targets = null, areaCells = null, hasMultipleTargets = false, hasArea = false },
                path = default,
                apCost = apCost,
                utility = utility,
                estimate = estimate,
                status = Status.Draft,
                rationale = rationale,
                constraints = constraints,
                planId = planId,
                planStepIndex = planStepIndex
            };
            candidates.Add(intent);
            return intent;
        }

        public IntentData CreateAbilityIntent(string abilityId, GameObject target, Vector2Int targetCell, int apCost, EffectEstimate estimate, float utility, string rationale = null, Constraints constraints = default, string planId = null, int planStepIndex = 0)
        {
            var intent = new IntentData
            {
                intentId = Guid.NewGuid().ToString(),
                action = ActionType.Ability,
                abilityId = abilityId,
                target = new TargetInfo { target = target, hasTarget = target != null, cell = targetCell, hasCell = true, targets = null, areaCells = null, hasMultipleTargets = false, hasArea = false },
                apCost = apCost,
                utility = utility,
                estimate = estimate,
                status = Status.Draft,
                rationale = rationale,
                constraints = constraints,
                planId = planId,
                planStepIndex = planStepIndex
            };
            candidates.Add(intent);
            return intent;
        }

        /// <summary>
        /// 构建一个 AoE 能力意图（传入区域格与可能的多目标）
        /// </summary>
        public IntentData CreateAoeIntent(string abilityId, List<Vector2Int> areaCells, List<GameObject> affectedTargets, int apCost, EffectEstimate estimate, float utility, string rationale = null, Constraints constraints = default, string planId = null, int planStepIndex = 0)
        {
            var intent = new IntentData
            {
                intentId = Guid.NewGuid().ToString(),
                action = ActionType.Ability,
                abilityId = abilityId,
                target = new TargetInfo
                {
                    targets = affectedTargets ?? new List<GameObject>(),
                    areaCells = areaCells ?? new List<Vector2Int>(),
                    hasMultipleTargets = affectedTargets != null && affectedTargets.Count > 0,
                    hasArea = areaCells != null && areaCells.Count > 0,
                    hasTarget = false,
                    hasCell = false
                },
                apCost = apCost,
                utility = utility,
                estimate = estimate,
                status = Status.Draft,
                rationale = rationale,
                constraints = constraints,
                planId = planId,
                planStepIndex = planStepIndex
            };
            candidates.Add(intent);
            return intent;
        }

        /// <summary>
        /// 选优：先比 utility，再比 priority，最后偏好较低 apCost 的方案。
        /// </summary>
        public void SelectBestByUtility()
        {
            IntentData best = null;
            var bestUtility = float.NegativeInfinity;
            var bestPriority = int.MinValue;
            var bestAp = int.MaxValue;
            for (int i = 0; i < candidates.Count; i++)
            {
                var c = candidates[i];
                bool better = false;
                if (c.utility > bestUtility)
                {
                    better = true;
                }
                else if (Mathf.Approximately(c.utility, bestUtility))
                {
                    if (c.priority > bestPriority)
                    {
                        better = true;
                    }
                    else if (c.priority == bestPriority)
                    {
                        if (c.apCost < bestAp)
                        {
                            better = true;
                        }
                    }
                }

                if (better)
                {
                    bestUtility = c.utility;
                    bestPriority = c.priority;
                    bestAp = c.apCost;
                    best = c;
                }
            }
            selected = best;
            if (selected != null)
            {
                selected.status = Status.Selected;
            }
        }

        public void MarkEvaluated(long tickNow)
        {
            evaluatedTick = tickNow;
            for (int i = 0; i < candidates.Count; i++)
            {
                var c = candidates[i];
                if (c.status == Status.Draft)
                {
                    c.status = Status.Evaluated;
                    candidates[i] = c;
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (!debugDraw) return;
            if (selected == null) return;
            // 绘制路径
            if (selected.path.cells != null && selected.path.cells.Count > 0)
            {
                Gizmos.color = pathColor;
                for (int i = 0; i < selected.path.cells.Count; i++)
                {
                    var cell = selected.path.cells[i];
                    var p = CellToWorldResolver != null ? CellToWorldResolver(cell) : new Vector3(cell.x, cell.y, 0f);
                    Gizmos.DrawSphere(p, gizmoSphereRadius);
                    if (i > 0)
                    {
                        var prevCell = selected.path.cells[i - 1];
                        var pPrev = CellToWorldResolver != null ? CellToWorldResolver(prevCell) : new Vector3(prevCell.x, prevCell.y, 0f);
                        Gizmos.DrawLine(pPrev, p);
                    }
                }
            }
            // 绘制目标/区域
            Gizmos.color = targetColor;
            if (selected.target.hasTarget && selected.target.target != null)
            {
                Gizmos.DrawWireSphere(selected.target.target.transform.position, gizmoSphereRadius * 1.5f);
            }
            if (selected.target.hasCell)
            {
                var c = selected.target.cell;
                var p = CellToWorldResolver != null ? CellToWorldResolver(c) : new Vector3(c.x, c.y, 0f);
                Gizmos.DrawWireCube(p, Vector3.one * (gizmoSphereRadius * 2f));
            }
            if (selected.target.hasMultipleTargets && selected.target.targets != null)
            {
                foreach (var t in selected.target.targets)
                {
                    if (t == null) continue;
                    Gizmos.DrawWireSphere(t.transform.position, gizmoSphereRadius * 1.5f);
                }
            }
            if (selected.target.hasArea && selected.target.areaCells != null)
            {
                foreach (var ac in selected.target.areaCells)
                {
                    var p = CellToWorldResolver != null ? CellToWorldResolver(ac) : new Vector3(ac.x, ac.y, 0f);
                    Gizmos.DrawWireCube(p, Vector3.one * (gizmoSphereRadius * 2f));
                }
            }
        }
    }
}
