using System.Collections.Generic;
using System.Linq;

namespace Enemy.AI
{
    public class NullPointerStrategy : IEnemyStrategy
    {
        public Unit AttackedUnit { get; set; }
        public Unit FindBestAttackTarget(Unit enemy, List<Unit> candidates)
        {
            Unit bestTarget = null;
            var bestScore = float.MinValue;

            foreach (var unit in candidates)
            {
                var score = EvaluateAttackTarget(enemy, unit);
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
            var bestScore = float.MinValue;

            foreach (var cell in moveRange)
            {
                var attackableCells = enemy.GetAttackRange(cell);
                var attackableTargets = allyUnits.Where(u => attackableCells.Contains(u.CurrentCell)).ToList();
                float score;
                if (attackableTargets.Count > 0)
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
            if (target == AttackedUnit)
            {
                return float.MinValue;
            }
            var distanceScore = 1f / (1 + GridManager.Instance.GetDistance(enemy.CurrentCell, target.CurrentCell));
            return distanceScore;
        }
    }
}