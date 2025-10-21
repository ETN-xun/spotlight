using System.Collections.Generic;
using System.Linq;
using Ally;

namespace Enemy.AI
{
    public class GarbledCrawlerStrategy : IEnemyStrategy
    {
        public Unit FindBestAttackTarget(Unit enemy, List<Unit> candidates)
        {
            Unit bestTarget = null;
            var bestScore = float.MinValue;

            foreach (var unit in candidates)
            {
                var score = EvaluateTarget(enemy, unit);
                if (score <= bestScore) continue;
                bestScore = score;
                bestTarget = unit;
            }

            return bestTarget;
        }
        
        public GridCell FindBestMoveTarget(Unit enemy, List<Unit> allyUnits)
        {
            var moveRange = enemy.GetMoveRange();
            var bestCell = enemy.CurrentCell;
            var bestScore = float.MaxValue;

            foreach (var cell in moveRange)
            {
                if (cell.CurrentUnit is null) continue;
                if (!allyUnits.Contains(cell.CurrentUnit)) continue;
                
                var minDistance = allyUnits.Min(unit => GridManager.Instance.GetDistance(cell, unit.CurrentCell));
                if (minDistance >= bestScore) continue;
                bestScore = minDistance;
                bestCell = cell;
            }

            return bestCell;
        }
        
        private float EvaluateTarget(Unit enemy, Unit target)
        {
            var distanceScore = 1f / (1 + GridManager.Instance.GetDistance(enemy.CurrentCell, target.CurrentCell));
            return distanceScore;
        }
    }
}