using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldSkill : Skill
{
    // Start is called before the first frame update
    public ShieldSkill(SkillDataSO data, Unit caster) : base(data, caster) {}

    public override void Execute(GridCell targetCell, GridManager gridManager)
    {
        if (targetCell.CurrentUnit != null)
        {
            if (targetCell.CurrentUnit.data.isEnemy == false)
            {
                targetCell.CurrentUnit.data.Hits++;
            }
        }

        if (targetCell.DestructibleObject != null)
        {
            targetCell.DestructibleObject.data.Hits++;
        }
    }
}
