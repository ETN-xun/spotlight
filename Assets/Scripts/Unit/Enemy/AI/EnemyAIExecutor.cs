using System.Collections;

namespace Enemy.AI
{
    public class EnemyAIExecutor
    {
        public IEnumerator ExecuteIntent(EnemyIntent intent)
        {
            // Execute the planned intent (move, attack, spawn, etc.)
            yield return null;
        }
        
        public IEnumerator ExecuteMove(GridCell targetCell)
        {
            // Execute move action to the target cell
            yield return null;
        }

        public IEnumerator ExecuteSpawn(GridCell spawnCell)
        {
            yield return null;
        }
        
        public IEnumerator ExecuteAttack(Skill skill, GridCell targetCell)
        {
            yield return null;
        }

        public IEnumerator ExecuteSkill(Skill skill, GridCell targetCell)
        {
            yield return null;
        }
    }
}