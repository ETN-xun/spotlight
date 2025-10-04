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
        
        Vector2Int newPos = targetCell.Coordinate + direction * data.displacementDistance;
        if (gridManager.IsValidPosition(newPos))
        {
            GridCell newCell = gridManager.GetCell(newPos);

            if (newCell.CurrentUnit == null) // 确保不会和其他单位重叠
            {
                targetUnit.MoveTo(newCell);
            }
            else
            {
            }
        }
    }
}

