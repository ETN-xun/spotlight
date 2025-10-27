using System.Collections.Generic;
using System.Linq;
using Ally;

namespace Enemy.AI
{
    public class GarbledCrawlerStrategy : IEnemyStrategy
    {
        public Unit FindBestAttackTarget(Unit enemy, List<Unit> candidates)
        {
            if (candidates == null || candidates.Count == 0)
                return null;
            Unit bestTarget = null;
            var bestScore = float.MinValue;

            foreach (var unit in candidates)
            {
                var score = EvaluateTarget(enemy, unit);
                if (score <= bestScore) continue;
                bestScore = score;
                bestTarget = unit;
            }

            return bestTarget ?? candidates[0];
        }
        
        public GridCell FindBestMoveTarget(Unit enemy, List<Unit> allyUnits)
        {
            var moveRange = enemy.GetMoveRange();
            var bestCell = enemy.CurrentCell;
            var bestScore = float.MaxValue;

            // 如果没有友方单位，直接返回当前位置
            if (allyUnits == null || allyUnits.Count == 0)
            {
                return bestCell;
            }

            foreach (var cell in moveRange)
            {
                if (cell.CurrentUnit is not null) continue;
                // if (!allyUnits.Contains(cell.CurrentUnit)) continue;
                
                var minDistance = allyUnits.Min(unit => GridManager.Instance.GetDistance(cell, unit.CurrentCell));
                if (minDistance >= bestScore) continue;
                bestScore = minDistance;
                bestCell = cell;
            }

            return bestCell;
        }
        
        private float EvaluateTarget(Unit enemy, Unit target)
        {
            var distance = GridManager.Instance.GetDistance(enemy.CurrentCell, target.CurrentCell);
            var distanceScore = 1f / (1 + distance);
            UnityEngine.Debug.Log($"[GarbledCrawlerStrategy] EvaluateTarget: enemy={enemy?.name}, target={target?.name}, distance={distance}, score={distanceScore}");
            return distanceScore;
        }
    }
}