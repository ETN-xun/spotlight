using System.Collections;
using UnityEngine;

namespace Action
{
    public class MoveAction : IAction
    {
        public IEnumerator Execute(ActionContext context)
        {
            var unit = context.activeUnit;
            unit.MoveTo(context.targetCell);
            // yield return MovementSystem.Instance.MoveUnit(unit, Vector2Int.down, 2);
            yield return null;
        }
        
        public void Undo(ActionContext context)
        {
            var unit = context.activeUnit;
            // unit.MoveTo(context.OriginCell);
        }
    }
}