using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageSkill : Skill
{
    public DamageSkill(SkillDataSO data, Unit caster) : base(data, caster) {}

    public override void Execute(GridCell targetCell, GridManager gridManager)
    {
        if (targetCell.CurrentUnit != null && data.canTargetEnemies)
        {
            targetCell.CurrentUnit.TakeDamage(data.baseDamage);
        }
        else
        {
           
        }
    }
}
