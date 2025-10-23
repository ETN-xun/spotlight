

using System.Collections.Generic;

namespace Enemy.AI
{
    public enum EnemyIntentType { None, Move, Attack, Spawn }

    public class EnemyIntent
    {
        public EnemyIntentType type;
        public float priority;
        public GridCell attackTargetCell;
        public GridCell moveTargetCell;
        public List<GridCell> movePath = new();
        
        public EnemyIntent()
        {
            type = EnemyIntentType.None;
            priority = 0;
        }
    }
}