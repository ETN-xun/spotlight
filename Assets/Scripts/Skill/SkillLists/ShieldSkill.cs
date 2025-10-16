using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 堆栈护盾技能
/// </summary>
public class ShieldSkill : Skill
{
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
