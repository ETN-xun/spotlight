using System.Collections.Generic;
using System.Linq;
using Action;
using Ally;
using UnityEngine;

namespace Enemy.AI
{
    public class EnemyIntentPlanner
    {
        // public readonly List<EnemyIntent> plannedIntents = new();
        private readonly Dictionary<Unit, EnemyIntent> _enemyIntents = new();
        
        public void BuildIntent(Unit enemy)
        {
            var allyUnits = AllyManager.Instance.GetAliveAllies();
            var moveRange = enemy.GetMoveRange();
            var attackRange = enemy.GetAttackRange(enemy.CurrentCell);
            var targets = allyUnits.Where(u => attackRange.Contains(u.CurrentCell)).ToList();
            if (targets.Count > 0)
            {
                var attackTarget = FindBestAttackTarget(enemy, allyUnits);
                var attackIntent = new EnemyIntent
                {
                    type = EnemyIntentType.Attack,
                    attackTargetCell = attackTarget.CurrentCell,
                    priority = enemy.data.aiPriority
                };
                _enemyIntents[enemy] = attackIntent; }
            else
            {
                var moveTarget = FindBestMoveTarget(enemy, allyUnits);
                var moveIntent = new EnemyIntent
                {
                    type = EnemyIntentType.Move,
                    moveTargetCell = moveTarget,
                    priority = enemy.data.aiPriority,
                };
                _enemyIntents[enemy] = moveIntent;
            }
            
            // 如果当前敌方攻击范围内没有我方单位，就进行移动，朝着最有价值的目标移动，如果移动之后仍没有攻击目标，就暂停
            // 如果当前敌方攻击范围内有我方单位，就攻击建筑或我方单位
            
            foreach (var allyUnit in allyUnits)
            {
                if (moveRange.Contains(allyUnit.CurrentCell))
                {
                    
                }
                
                if (enemy.data.buildingTargetPriority > enemy.data.unitTargetPriority)
                {
                    
                }
                else
                {
                    
                }
            }
        }
        
        public Dictionary<Unit, EnemyIntent> GetOrderedEnemyIntents()
        {
            var ordered = _enemyIntents
                .Where(kv => kv.Value != null && kv.Value.IsValid())
                .OrderByDescending(kv => kv.Value.priority)
                .ToDictionary(kv => kv.Key, kv => kv.Value);

            return ordered;
        }
        
        public void ClearIntents()
        {
            _enemyIntents.Clear();
        }

        private Unit FindBestAttackTarget(Unit enemy, List<Unit> allyUnits)
        {
            Unit bestTarget = null;
            var bestScore = float.MinValue;
            
            foreach (var target in allyUnits)
            {
                var score = EvaluateTarget(enemy, target);

                if (!(score > bestScore)) continue;
                bestScore = score;
                bestTarget = target;
            }

            return bestTarget;
        }


        private GridCell FindBestMoveTarget(Unit enemy, List<Unit> allyUnits)
        {
            var moveRange = enemy.GetMoveRange();
            var bestCell = enemy.CurrentCell;
            var bestScore = float.MinValue;

            foreach (var cell in moveRange)
            {
                var attackableCells = enemy.GetAttackRange(cell);
                var attackableTargets = allyUnits.Where(u => attackableCells.Contains(u.CurrentCell)).ToList();

                var score = 0f;

                if (attackableTargets.Count > 0)
                {
                    // 如果能攻击到至少一个目标，就取其中最优的目标分数
                    score = attackableTargets.Max(t => EvaluateTarget(enemy, t)) + 5f; // 额外加分
                }
                else
                {
                    // 否则靠近最近的敌人
                    float minDist = allyUnits.Min(u => GetDistance(cell, u.CurrentCell));
                    score = 1f / (1 + minDist);
                }

                if (!(score > bestScore)) continue;
                bestScore = score;
                bestCell = cell;
            }

            return bestCell;
        }

        
        private float EvaluateTarget(Unit enemy, Unit target)
        {
            var hpScore = (target.data.maxHP - target.currentHP) / (float)target.data.maxHP; 
            var threatScore = target.data.baseDamage / 10f; 
            var distanceScore = 1f / (1 + GetDistance(enemy.CurrentCell, target.CurrentCell)); 

            return hpScore * 2f + threatScore + distanceScore;
        }

        private int GetDistance(GridCell cell, GridCell target)
        {
            return Mathf.Abs(target.Coordinate.x - cell.Coordinate.x) +
                   Mathf.Abs(target.Coordinate.y - cell.Coordinate.y);
        }

    }
}