using System.Collections.Generic;
using System.Linq;
using Action;
using Ally;
using UnityEngine;

namespace Enemy.AI
{
    public class EnemyIntentPlanner
    {
        public void BuildIntent(Unit enemy)
        {
            var allyUnits = AllyManager.Instance.GetAliveAllies();
            var moveRange = enemy.GetMoveRange();
            var attackRange = enemy.GetAttackRange(enemy.CurrentCell);
            var targets = allyUnits.Where(u => attackRange.Contains(u.CurrentCell)).ToList();
            if (targets.Count > 0)
            {
                var attackTarget = FindBestAttackTarget(enemy, allyUnits);
                ActionManager.Instance.ExecuteEnemyAttackAction(enemy, attackTarget.CurrentCell);     // 应该是生成意图
            }
            else
            {
                var moveTarget = FindBestMoveTarget(enemy, allyUnits);
                enemy.MoveTo(moveTarget);
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

            // // 根据敌人类型、位置、玩家单位位置等因素，规划敌人的意图
            // var intent = new EnemyIntent();
            //
            // // 简单示例：如果敌人距离玩家单位在攻击范围内，则规划攻击意图
            // var playerUnits = Player.PlayerManager.Instance.GetAllPlayerUnits();
            // foreach (var playerUnit in playerUnits)
            // {
            //     var distance = Vector2Int.Distance(enemy.GridPosition, playerUnit.GridPosition);
            //     if (distance <= enemy.AttackRange)
            //     {
            //         intent.attackTargetCell = GridManager.Instance.GetCellAtPosition(playerUnit.GridPosition);
            //         intent.plannedSkill = enemy.BasicAttackSkill;
            //         return intent;
            //     }
            // }
            //
            // // 否则，规划移动意图，向最近的玩家单位移动
            // if (playerUnits.Count > 0)
            // {
            //     var closestPlayer = playerUnits[0];
            //     var minDistance = Vector2Int.Distance(enemy.GridPosition, closestPlayer.GridPosition);
            //     foreach (var playerUnit in playerUnits)
            //     {
            //         var distance = Vector2Int.Distance(enemy.GridPosition, playerUnit.GridPosition);
            //         if (distance < minDistance)
            //         {
            //             minDistance = distance;
            //             closestPlayer = playerUnit;
            //         }
            //     }
            //     
            //     // 计算移动目标位置（简单示例，向玩家单位方向移动一格）
            //     var direction = (closestPlayer.GridPosition - enemy.GridPosition).normalized;
            //     var moveTargetPos = enemy.GridPosition + new Vector2Int(Mathf.RoundToInt(direction.x), Mathf.RoundToInt(direction.y));
            //     intent.moveTargetCell = GridManager.Instance.GetCellAtPosition(moveTargetPos);
            // }
            //
            // return intent;
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
                   Mathf.Abs(target.Coordinate.y - target.Coordinate.y);
        }

    }
}