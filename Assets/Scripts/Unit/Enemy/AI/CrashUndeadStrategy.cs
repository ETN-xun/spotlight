using System.Collections.Generic;
using System.Linq;
using Ally;

namespace Enemy.AI
{
    public class CrashUndeadStrategy : IEnemyStrategy
    {
        public Unit FindBestAttackTarget(Unit enemy, List<Unit> candidates)
        {
            if (candidates == null || candidates.Count == 0)
                return null;
            Unit bestTarget = null;
            var bestScore = float.MinValue;

            foreach (var unit in candidates)
            {
                var score = EvaluateAttackTarget(enemy, unit);
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
            var bestScore = float.MinValue;

            // 如果没有友方单位，优先寻找BugTile地形，否则进行随机移动
            if (allyUnits == null || allyUnits.Count == 0)
            {
                // 优先寻找BugTile地形
                var bugTileCells = moveRange.Where(cell => cell.CurrentUnit == null && 
                    cell.TerrainData != null && cell.TerrainData.terrainType == TerrainType.BugTile).ToList();
                if (bugTileCells.Count > 0)
                {
                    int randomIndex = UnityEngine.Random.Range(0, bugTileCells.Count);
                    return bugTileCells[randomIndex];
                }
                
                // 如果没有BugTile，随机移动
                var availableCells = moveRange.Where(cell => cell.CurrentUnit == null && cell != enemy.CurrentCell).ToList();
                if (availableCells.Count > 0)
                {
                    int randomIndex = UnityEngine.Random.Range(0, availableCells.Count);
                    return availableCells[randomIndex];
                }
                return bestCell;
            }

            foreach (var cell in moveRange)
            {
                var attackableCells = enemy.GetAttackRange(cell);
                var attackableTargets = allyUnits.Where(u => attackableCells.Contains(u.CurrentCell)).ToList();
                float score;
                if (cell.TerrainData is not null && cell.TerrainData.terrainType == TerrainType.BugTile)
                {
                    score = 10f;
                }
                else if (attackableTargets.Count > 0)
                {
                    score = attackableTargets.Max(t => EvaluateAttackTarget(enemy, t)) + 5f;
                }
                else
                {
                    float minDist = allyUnits.Min(u => GridManager.Instance.GetDistance(cell, u.CurrentCell));
                    score = 1f / (1 + minDist);
                }
                
                if (score <= bestScore) continue;
                bestScore = score;
                bestCell = cell;
            }

            return bestCell;
        }
        
        private float EvaluateAttackTarget(Unit enemy, Unit target)
        {
            var distanceScore = 1f / (1 + GridManager.Instance.GetDistance(enemy.CurrentCell, target.CurrentCell));
            return distanceScore;
        }
    }
}