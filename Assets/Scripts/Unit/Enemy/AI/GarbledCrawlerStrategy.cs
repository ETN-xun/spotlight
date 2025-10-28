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

            // 如果没有友方单位，进行随机移动而不是停留在原地
            if (allyUnits == null || allyUnits.Count == 0)
            {
                var availableCells = moveRange.Where(cell => cell.CurrentUnit == null && cell != enemy.CurrentCell).ToList();
                if (availableCells.Count > 0)
                {
                    // 随机选择一个可移动的位置
                    int randomIndex = UnityEngine.Random.Range(0, availableCells.Count);
                    return availableCells[randomIndex];
                }
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
            return distanceScore;
        }
    }
}