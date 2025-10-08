using System.Collections;
using UnityEngine;

namespace Action
{
    public class MoveAction : IAction
    {
        public IEnumerator Execute(ActionContext context)
        {
            var unit = context.ActiveUnit;
            unit.MoveTo(context.TargetCell);
            // yield return MovementSystem.Instance.MoveUnit(unit, Vector2Int.down, 2);
            yield return null;
        }
    }
}