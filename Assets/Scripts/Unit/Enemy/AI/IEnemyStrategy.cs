using System.Collections.Generic;
using System.Linq;
using Ally;
using UnityEngine;

namespace Enemy.AI
{
    public interface IEnemyStrategy
    {
        public List<EnemyIntent> BuildIntent(Unit enemy)
        {
            var allyUnits = AllyManager.Instance.GetAliveAllies();
            var attackRange = enemy.GetAttackRange(enemy.CurrentCell);
            var enemyIntents = new List<EnemyIntent>();

            var targetsInAttackRange = allyUnits.Where(u => attackRange.Contains(u.CurrentCell)).ToList();
            if (targetsInAttackRange.Count > 0)
            {
                var attackTarget = FindBestAttackTarget(enemy, targetsInAttackRange);
                enemyIntents.Add(new EnemyIntent
                {
                    type = EnemyIntentType.Attack,
                    attackTargetCell = attackTarget.CurrentCell,
                    priority = enemy.data.aiPriority
                });
                return enemyIntents;
            }

            var bestMoveCell = FindBestMoveTarget(enemy, allyUnits);
            if (bestMoveCell is not null && bestMoveCell != enemy.CurrentCell)
            {
                // 先尝试到达最优单元格
                var movePath = MovementSystem.Instance.FindPathForEnemy(enemy.CurrentCell, bestMoveCell);

                // 如果最优单元格不可达，回退到任一可达的空单元格
                if (movePath == null || movePath.Count < 2)
                {
                    var fallbackCell = enemy
                        .GetMoveRange()
                        .FirstOrDefault(c => c.CurrentUnit == null && MovementSystem.Instance.FindPathForEnemy(enemy.CurrentCell, c)?.Count >= 2);

                    if (fallbackCell != null)
                    {
                        bestMoveCell = fallbackCell;
                        movePath = MovementSystem.Instance.FindPathForEnemy(enemy.CurrentCell, bestMoveCell);
                    }
                }

                // 最终确认路径有效再添加移动意图
                if (movePath != null && movePath.Count >= 2)
                {
                    var moveIntent = new EnemyIntent
                    {
                        type = EnemyIntentType.Move,
                        moveTargetCell = bestMoveCell,
                        movePath = movePath,
                        priority = enemy.data.aiPriority,
                    };
                    enemyIntents.Add(moveIntent);

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
                        enemyIntents.Add(attackIntent);
                    }
                }
            }

            return enemyIntents;
        }
        public GridCell FindBestMoveTarget(Unit enemy, List<Unit> allyUnits);
        public Unit FindBestAttackTarget(Unit enemy, List<Unit> candidates);
    }
}