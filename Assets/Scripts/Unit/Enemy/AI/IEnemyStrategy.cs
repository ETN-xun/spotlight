using System.Collections.Generic;
using System.Linq;
using Ally;

namespace Enemy.AI
{
    public interface IEnemyStrategy
    {
        public List<EnemyIntent> BuildIntent(Unit enemy)
        {
            // 先判断当前是否有可攻击目标，有则直接攻击，没有则移动到最佳位置，再判断是否有可攻击目标，有则攻击，没有则结束
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
                var moveIntent = new EnemyIntent
                {
                    type = EnemyIntentType.Move,
                    moveTargetCell = bestMoveCell,
                    movePath = MovementSystem.Instance.FindPathForEnemy(enemy.CurrentCell, bestMoveCell),
                    priority = enemy.data.aiPriority,
                };
                enemyIntents.Add(moveIntent);

                var postMoveAttackRange = enemy.GetAttackRange(bestMoveCell);
                var postMoveTargets = allyUnits.Where(u => postMoveAttackRange.Contains(u.CurrentCell)).ToList();

                if (postMoveTargets.Count <= 0) return enemyIntents;
                var attackTarget = FindBestAttackTarget(enemy, postMoveTargets);
                var attackIntent = new EnemyIntent
                {
                    type = EnemyIntentType.Attack,
                    attackTargetCell = attackTarget.CurrentCell,
                    priority = enemy.data.aiPriority
                };
                enemyIntents.Add(attackIntent);
            }

            return enemyIntents;
        }
        public GridCell FindBestMoveTarget(Unit enemy, List<Unit> allyUnits);
        public Unit FindBestAttackTarget(Unit enemy, List<Unit> candidates);
    }
}