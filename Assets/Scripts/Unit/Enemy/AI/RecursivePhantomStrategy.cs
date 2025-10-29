using System.Collections.Generic;
using System.Linq;
using Ally;

namespace Enemy.AI
{
    public class RecursivePhantomStrategy : IEnemyStrategy
    {
        public Unit FindBestAttackTarget(Unit enemy, List<Unit> candidates)
        {
            if (candidates == null || candidates.Count == 0)
                return null;
            
            // 优先攻击闪回复制
            var flashbackCopies = candidates.Where(u => u.GetComponent<FlashbackCopyTag>() != null).ToList();
            if (flashbackCopies.Count > 0)
            {
                // 在闪回复制中选择最佳目标
                Unit bestFlashbackTarget = null;
                var bestFlashbackScore = float.MinValue;
                
                foreach (var unit in flashbackCopies)
                {
                    var score = EvaluateAttackTarget(enemy, unit);
                    if (score > bestFlashbackScore)
                    {
                        bestFlashbackScore = score;
                        bestFlashbackTarget = unit;
                    }
                }
                
                if (bestFlashbackTarget != null)
                    return bestFlashbackTarget;
            }
            
            // 如果没有闪回复制，按原逻辑选择目标
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

            if (allyUnits == null || allyUnits.Count == 0)
            {
                var candidates = moveRange
                    .Where(c => c != enemy.CurrentCell && (c.TerrainData == null || c.TerrainData.terrainType != TerrainType.CorrosionTile))
                    .ToList();
                if (candidates.Count > 0)
                {
                    int idx = UnityEngine.Random.Range(0, candidates.Count);
                    return candidates[idx];
                }
                return bestCell;
            }

            int atkRange = enemy.data.attackRange;

            foreach (var cell in moveRange)
            {
                bool avoidCorrosion = cell.TerrainData != null && cell.TerrainData.terrainType == TerrainType.CorrosionTile;

                var inRangeTargets = allyUnits
                    .Where(u => GridManager.Instance.GetDistance(cell, u.CurrentCell) > 0 &&
                                GridManager.Instance.GetDistance(cell, u.CurrentCell) <= atkRange)
                    .ToList();

                float score;
                if (inRangeTargets.Count > 0)
                {
                    var targetScore = inRangeTargets.Max(t => EvaluateAttackTarget(enemy, t));
                    score = targetScore + 5f;
                }
                else
                {
                    float minDist = allyUnits.Min(u => GridManager.Instance.GetDistance(cell, u.CurrentCell));
                    // 越接近进入攻击范围越好
                    float closeness = 1f / (1 + System.MathF.Abs(minDist - atkRange));
                    score = closeness;
                }

                if (avoidCorrosion)
                    score -= 100f;

                if (score <= bestScore) continue;
                bestScore = score;
                bestCell = cell;
            }

            return bestCell;
        }

        private float EvaluateAttackTarget(Unit enemy, Unit target)
        {
            if (enemy.attackedUnits.ContainsKey(target))
                return float.MinValue;
            float dist = GridManager.Instance.GetDistance(enemy.CurrentCell, target.CurrentCell);
            return 1f / (1 + dist);
        }
    }
}