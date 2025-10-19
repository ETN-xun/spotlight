using System.Collections.Generic;
using System.Linq;
using Action;
using Ally;
using UnityEngine;

namespace Enemy.AI
{
    public class EnemyIntentPlanner
    {
        private readonly Dictionary<Unit, List<EnemyIntent>> _enemyIntents = new();

        public void BuildIntent(Unit enemy)
        {
            var allyUnits = AllyManager.Instance.GetAliveAllies();
            var moveRange = enemy.GetMoveRange();
            var attackRange = enemy.GetAttackRange(enemy.CurrentCell);
            var intents = new List<EnemyIntent>();
            
            var targetsInRange = allyUnits.Where(u => attackRange.Contains(u.CurrentCell)).ToList();
            if (targetsInRange.Count > 0)
            {
                var attackTarget = FindBestAttackTarget(enemy, targetsInRange);
                intents.Add(new EnemyIntent
                {
                    type = EnemyIntentType.Attack,
                    attackTargetCell = attackTarget.CurrentCell,
                    priority = enemy.data.aiPriority
                });
                _enemyIntents[enemy] = intents;
                return;
            }
            
            var bestMoveCell = FindBestMoveTarget(enemy, allyUnits);
            if (bestMoveCell != null && bestMoveCell != enemy.CurrentCell)
            {
                var moveIntent = new EnemyIntent
                {
                    type = EnemyIntentType.Move,
                    moveTargetCell = bestMoveCell,
                    movePath = MovementSystem.Instance.FindPath(enemy.CurrentCell, bestMoveCell),
                    priority = enemy.data.aiPriority,
                };
                intents.Add(moveIntent);

                var postMoveAttackRange = enemy.GetAttackRange(bestMoveCell);
                var postMoveTargets = allyUnits.Where(u => postMoveAttackRange.Contains(u.CurrentCell)).ToList();

                if (postMoveTargets.Count > 0)
                {
                    var attackTarget = FindBestAttackTarget(enemy, postMoveTargets);
                    var attackIntent = new EnemyIntent
                    {
                        type = EnemyIntentType.Attack,
                        attackTargetCell = attackTarget.CurrentCell,
                        priority = enemy.data.aiPriority
                    };
                    intents.Add(attackIntent);
                }
            }

            _enemyIntents[enemy] = intents;
        }

        public Dictionary<Unit, List<EnemyIntent>> GetOrderedEnemyIntents()
        {
            return _enemyIntents
                .Where(kv => kv.Value != null && kv.Value.Count > 0)
                .OrderByDescending(kv => kv.Value.First().priority)
                .ToDictionary(kv => kv.Key, kv => kv.Value);
        }

        public void ClearIntents()
        {
            _enemyIntents.Clear();
        }

        private Unit FindBestAttackTarget(Unit enemy, List<Unit> candidates)
        {
            Unit bestTarget = null;
            var bestScore = float.MinValue;

            foreach (var t in candidates)
            {
                var score = EvaluateTarget(enemy, t);
                if (score <= bestScore) continue;
                bestScore = score;
                bestTarget = t;
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

                float score;
                if (attackableTargets.Count > 0)
                {
                    score = attackableTargets.Max(t => EvaluateTarget(enemy, t)) + 5f;
                }
                else
                {
                    float minDist = allyUnits.Min(u => GetDistance(cell, u.CurrentCell));
                    score = 1f / (1 + minDist);
                }

                if (score > bestScore)
                {
                    bestScore = score;
                    bestCell = cell;
                }
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

        private int GetDistance(GridCell a, GridCell b)
        {
            return Mathf.Abs(a.Coordinate.x - b.Coordinate.x) +
                   Mathf.Abs(a.Coordinate.y - b.Coordinate.y);
        }
    }
}
