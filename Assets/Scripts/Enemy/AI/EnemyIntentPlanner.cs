using System.Collections.Generic;
using UnityEngine;
using Action;

namespace Enemy.AI
{
    public static class EnemyIntentPlanner
    {
        public static EnemyIntent BuildForUnit(Unit unit)
        {
            if (unit == null || unit.data == null || unit.CurrentCell == null)
            {
                return null;
            }

            var intent = unit.GetComponent<EnemyIntent>();
            if (intent == null) intent = unit.gameObject.AddComponent<EnemyIntent>();

            // 绑定网格坐标转换用于调试
            EnemyIntent.CellToWorldResolver = (cell) => GridManager.Instance != null ? GridManager.Instance.CellToWorld(cell) : new Vector3(cell.x, cell.y, 0f);

            intent.actor = unit.gameObject;
            intent.actorId = unit.data.unitID;
            intent.createdTick = Time.frameCount;
            intent.ClearAll();
            intent.selectedPlan.Clear();

            // 1) 生成所有候选意图
            TryBuildBasicAttackIntents(unit, intent);
            TryBuildSkillIntents(unit, intent);
            TryBuildMoveIntents(unit, intent);
            intent.MarkEvaluated(Time.frameCount);
            intent.SelectBestByUtility();

            // 2) 生成多步计划
            // 2.1 能直接攻击/技能？
            var direct = FindBestAttackOrAbility(intent, unit.CurrentCell.Coordinate);
            if (direct != null)
            {
                intent.selectedPlan.Add(direct);
                intent.selected = direct;
                return intent;
            }
            // 2.2 能移动后攻击/技能？
            var moveAndAttack = FindMoveAndAttackPlan(unit, intent);
            if (moveAndAttack != null && moveAndAttack.Count > 0)
            {
                intent.selectedPlan.AddRange(moveAndAttack);
                intent.selected = moveAndAttack[moveAndAttack.Count-1];
                return intent;
            }
            // 2.3 只能移动
            var bestMove = FindBestMove(intent);
            if (bestMove != null)
            {
                intent.selectedPlan.Add(bestMove);
                intent.selected = bestMove;
            }
            return intent;
        }

        // 查找当前位置能直接攻击/技能的最佳意图
        private static EnemyIntent.IntentData FindBestAttackOrAbility(EnemyIntent intent, Vector2Int currentPos)
        {
            EnemyIntent.IntentData best = null;
            float bestUtil = float.NegativeInfinity;
            foreach (var c in intent.candidates)
            {
                if ((c.action == ActionType.Attack || c.action == ActionType.Ability)
                    && c.target.hasCell && c.path.cells == null)
                {
                    // 只考虑当前位置能打到的
                    if (best == null || c.utility > bestUtil)
                    {
                        best = c;
                        bestUtil = c.utility;
                    }
                }
            }
            return best;
        }

        // 查找移动后能攻击/技能的多步计划 [Move,Attack/Ability]
        private static List<EnemyIntent.IntentData> FindMoveAndAttackPlan(Unit unit, EnemyIntent intent)
        {
            var moves = new List<EnemyIntent.IntentData>();
            foreach (var move in intent.candidates)
            {
                if (move.action != ActionType.Move) continue;
                var afterMovePos = move.target.cell;
                // 检查该位置能否攻击/技能
                foreach (var c in intent.candidates)
                {
                    if ((c.action == ActionType.Attack || c.action == ActionType.Ability)
                        && c.target.hasCell)
                    {
                        // 攻击/技能的起点要等于 afterMovePos
                        if (c.path.cells == null && unit.CurrentCell.Coordinate != afterMovePos) // 只考虑移动后能打到的
                        {
                            // 这里简化：只要目标格与移动目标一致就认为可行
                            // 实际可根据技能/攻击范围再做更精细判断
                            var plan = new List<EnemyIntent.IntentData> { move, c };
                            return plan;
                        }
                    }
                }
            }
            return null;
        }

        // 找到最佳移动意图
        private static EnemyIntent.IntentData FindBestMove(EnemyIntent intent)
        {
            EnemyIntent.IntentData best = null;
            float bestUtil = float.NegativeInfinity;
            foreach (var c in intent.candidates)
            {
                if (c.action == ActionType.Move)
                {
                    if (best == null || c.utility > bestUtil)
                    {
                        best = c;
                        bestUtil = c.utility;
                    }
                }
            }
            return best;
        }

        private static void TryBuildBasicAttackIntents(Unit unit, EnemyIntent intent)
        {
            int range = unit.data.attackRange;
            if (range <= 0) return;
            var origin = unit.CurrentCell.Coordinate;
            for (int dx = -range; dx <= range; dx++)
            {
                for (int dy = -range; dy <= range; dy++)
                {
                    int dist = Mathf.Abs(dx) + Mathf.Abs(dy);
                    if (dist == 0 || dist > range) continue;
                    var pos = origin + new Vector2Int(dx, dy);
                    if (!GridManager.Instance.IsValidPosition(pos)) continue;
                    var cell = GridManager.Instance.GetCell(pos);
                    var target = cell.CurrentUnit;
                    if (target == null) continue;
                    // 仅对立阵营
                    if (target.data != null && target.data.isEnemy == unit.data.isEnemy) continue;

                    var estimate = new EnemyIntent.EffectEstimate
                    {
                        hitChance = 1f,
                        expectedDamage = unit.data.baseDamage,
                        killChance = target.currentHP <= unit.data.baseDamage ? 1f : 0f,
                        selfRisk = 0f,
                        ffRisk = 0f
                    };

                    var constraints = new EnemyIntent.Constraints
                    {
                        minRange = 1,
                        maxRange = unit.data.attackRange,
                        requiresLineOfSight = false,
                        cooldownRemaining = 0,
                        willBreakStealth = false,
                        budgetCost = unit.data.movementEnergyCost // 无独立攻能消耗，用移动能量代替占位
                    };

                    var id = intent.CreateAttackIntent(target.gameObject, pos, apCost: 1, estimate: estimate, utility: 0f, rationale: "Basic Attack", constraints: constraints);
                    // 计算 utility：伤害 + 击杀奖励
                    id.utility = estimate.expectedDamage + estimate.killChance * 5f;
                }
            }
        }

        private static void TryBuildSkillIntents(Unit unit, EnemyIntent intent)
        {
            var skills = unit.data.skills;
            if (skills == null || skills.Length == 0) return;
            var casterPos = unit.CurrentCell.Coordinate;

            foreach (var s in skills)
            {
                if (s == null || s.isPassive) continue;
                var targetCells = s.GetTargetableCells(casterPos, GridManager.Instance);
                if (targetCells == null) continue;
                foreach (var cellPos in targetCells)
                {
                    var cell = GridManager.Instance.GetCell(cellPos);
                    // 判断目标合法性
                    var hasUnit = cell.CurrentUnit != null;
                    var targetUnit = cell.CurrentUnit;
                    bool isEnemyTarget = targetUnit != null && targetUnit.data != null && (targetUnit.data.isEnemy != unit.data.isEnemy);
                    bool isAllyTarget = targetUnit != null && targetUnit.data != null && (targetUnit.data.isEnemy == unit.data.isEnemy);

                    if (s.requiresTarget)
                    {
                        // 必须有目标
                        if (!hasUnit) continue;
                        if (isEnemyTarget && !s.canTargetEnemies) continue;
                        if (isAllyTarget && !s.canTargetAllies) continue;
                    }

                    // AoE/多目标收集
                    var affectedCells = CollectAoECells(s, cellPos);
                    var affectedTargets = new List<GameObject>();
                    int enemiesHit = 0, alliesHit = 0;
                    foreach (var ac in affectedCells)
                    {
                        var acCell = GridManager.Instance.GetCell(ac);
                        if (acCell == null) continue;
                        var u = acCell.CurrentUnit;
                        if (u == null) continue;
                        affectedTargets.Add(u.gameObject);
                        if (u.data != null && u.data.isEnemy != unit.data.isEnemy) enemiesHit++;
                        else alliesHit++;
                    }

                    var estimate = new EnemyIntent.EffectEstimate
                    {
                        hitChance = 1f,
                        expectedDamage = s.baseDamage * Mathf.Max(1, enemiesHit),
                        killChance = 0f, // 粗略估计，详细可逐目标计算
                        selfRisk = 0f,
                        ffRisk = alliesHit > 0 ? s.baseDamage * alliesHit : 0f,
                        displacementValue = s.displacementDistance,
                        coverBonus = 0f,
                        terrainBonus = 0f
                    };

                    var constraints = new EnemyIntent.Constraints
                    {
                        minRange = 0,
                        maxRange = s.range,
                        requiresLineOfSight = false,
                        cooldownRemaining = 0, // 未集成冷却系统的运行时状态
                        willBreakStealth = false,
                        budgetCost = s.energyCost
                    };

                    EnemyIntent.IntentData id;
                    if (IsAoeSkill(s))
                    {
                        id = intent.CreateAoeIntent(s.skillID, affectedCells, affectedTargets, apCost: s.energyCost, estimate: estimate, utility: 0f, rationale: s.skillName, constraints: constraints);
                    }
                    else
                    {
                        id = intent.CreateAbilityIntent(s.skillID, target: targetUnit != null ? targetUnit.gameObject : null, targetCell: cellPos, apCost: s.energyCost, estimate: estimate, utility: 0f, rationale: s.skillName, constraints: constraints);
                    }

                    // utility：偏好命中敌人的期望伤害，惩罚友伤
                    id.utility = estimate.expectedDamage - estimate.ffRisk * 0.5f + estimate.displacementValue * 0.2f;
                    // 若存在明显击杀机会（粗略）：任一受击敌人 HP <= baseDamage
                    if (targetUnit != null && targetUnit.data != null && (targetUnit.currentHP <= s.baseDamage) && (targetUnit.data.isEnemy != unit.data.isEnemy))
                    {
                        id.utility += 5f;
                    }
                }
            }
        }

        private static void TryBuildMoveIntents(Unit unit, EnemyIntent intent)
        {
            var cells = unit.GetMoveRange();
            if (cells == null || cells.Count == 0) return;
            foreach (var c in cells)
            {
                var path = new List<Vector2Int> { unit.CurrentCell.Coordinate, c.Coordinate };
                var ap = unit.data.movementEnergyCost * Manhattan(unit.CurrentCell.Coordinate, c.Coordinate);
                var id = intent.CreateMoveIntent(c.Coordinate, path, ap, utility: 0.1f, rationale: "Advance");
                // 简单启发：靠近最近敌人
                var nearestEnemyDist = FindNearestEnemyDistance(c.Coordinate, unit.data.isEnemy);
                id.utility = 0.5f / (1 + nearestEnemyDist);
            }
        }

        private static int Manhattan(Vector2Int a, Vector2Int b)
        {
            return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
        }

        private static int FindNearestEnemyDistance(Vector2Int from, bool isEnemyCaster)
        {
            int best = int.MaxValue;
            // 遍历全图寻找非同阵营单位
            for (int x = 0; x < GridManager.Instance.cols; x++)
            {
                for (int y = 0; y < GridManager.Instance.rows; y++)
                {
                    var cell = GridManager.Instance.WorldToCell(GridManager.Instance.CellToWorld(new Vector2Int(x, y)));
                    if (cell == null || cell.CurrentUnit == null) continue;
                    var u = cell.CurrentUnit;
                    if (u.data != null && u.data.isEnemy != isEnemyCaster)
                    {
                        int d = Manhattan(from, cell.Coordinate);
                        if (d < best) best = d;
                    }
                }
            }
            return best == int.MaxValue ? 99 : best;
        }

        private static bool IsAoeSkill(SkillDataSO s)
        {
            return s != null && (s.effectRadius > 0 || (s.effectPattern != null && s.effectPattern.Count > 0));
        }

        private static List<Vector2Int> CollectAoECells(SkillDataSO s, Vector2Int center)
        {
            var result = new List<Vector2Int> { center };
            if (s == null) return result;
            if (s.effectRadius > 0)
            {
                for (int dx = -s.effectRadius; dx <= s.effectRadius; dx++)
                {
                    for (int dy = -s.effectRadius; dy <= s.effectRadius; dy++)
                    {
                        var p = center + new Vector2Int(dx, dy);
                        if (!GridManager.Instance.IsValidPosition(p)) continue;
                        result.Add(p);
                    }
                }
            }
            if (s.effectPattern != null)
            {
                foreach (var off in s.effectPattern)
                {
                    var p = center + off;
                    if (!GridManager.Instance.IsValidPosition(p)) continue;
                    if (!result.Contains(p)) result.Add(p);
                }
            }
            return result;
        }
    }
}
