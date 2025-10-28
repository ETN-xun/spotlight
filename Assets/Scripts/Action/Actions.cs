using System.Collections;
using UnityEngine;

namespace Action
{
    public class MoveAction : IAction
    {
        public IEnumerator Execute(ActionContext context)
        {
            var unit = context.activeUnit;
            MovementSystem.Instance.TryMoveToCell(unit, context.targetCell);
            yield return null;
        }
        
        public void Undo(ActionContext context)
        {
            var unit = context.activeUnit;
        }
    }
}