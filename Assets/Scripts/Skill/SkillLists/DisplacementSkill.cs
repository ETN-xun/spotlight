using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplacementSkill : Skill
{
    public DisplacementSkill(SkillDataSO data, Unit caster) : base(data, caster) {}

    public override void Execute(GridCell targetCell, GridManager gridManager)
    {
        Unit targetUnit = targetCell.CurrentUnit;
        if (targetUnit == null) return;
        
        Vector2Int direction = targetCell.Coordinate - caster.CurrentCell.Coordinate;
        direction = new Vector2Int(Mathf.Clamp(direction.x, -1, 1), Mathf.Clamp(direction.y, -1, 1));
        
        if (data.displacementDirection == DisplacementDirection.Pull)
        {
            direction = -direction;
        }
        
        MovementSystem.Instance.MoveUnit(targetUnit, direction, data.displacementDistance);
    }
}

