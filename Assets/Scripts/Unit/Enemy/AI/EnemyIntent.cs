

namespace Enemy.AI
{
    public enum EnemyIntentType { None, Move, Attack, Spawn }

    public class EnemyIntent
    {
        public EnemyIntentType type;
        public GridCell attackTargetCell;
        public GridCell moveTargetCell;
        public GridCell spawnTargetCell;
        public Skill plannedSkill;
        public float priority; 

        public EnemyIntent()
        {
            type = EnemyIntentType.None;
            priority = 0;
        }

        public bool IsValid()
        {
            return type switch
            {
                EnemyIntentType.Attack => attackTargetCell != null,
                EnemyIntentType.Move => moveTargetCell != null,
                EnemyIntentType.Spawn => spawnTargetCell != null && plannedSkill != null,
                _ => true
            };
        }
    }
}