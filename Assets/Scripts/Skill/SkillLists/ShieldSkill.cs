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
        // 检查目标格子是否有单位
        if (targetCell.CurrentUnit != null)
        {
            Unit target = targetCell.CurrentUnit;
            
            // 检查是否可以对该目标使用护盾技能
            if (CanTargetUnit(target))
            {
                target.data.Hits++;
                Debug.Log($"{caster.data.unitName} 为 {target.data.unitName} 增加了护盾");
            }
            else
            {
                Debug.Log($"无法对 {target.data.unitName} 使用护盾技能：只能对友方单位使用");
            }
        }
        // 检查目标格子是否有建筑对象
        else if (targetCell.DestructibleObject != null)
        {
            targetCell.DestructibleObject.data.Hits++;
            Debug.Log($"{caster.data.unitName} 为可摧毁对象增加了护盾");
        }
        else
        {
            Debug.Log("目标位置没有可以加护盾的对象");
        }
    }
    
    /// <summary>
    /// 检查是否可以对目标单位使用技能
    /// </summary>
    /// <param name="target">目标单位</param>
    /// <returns>是否可以使用</returns>
    private bool CanTargetUnit(Unit target)
    {
        bool isTargetEnemy = target.data.isEnemy;
        bool isCasterEnemy = caster.data.isEnemy;
        
        // 护盾技能只能对友方单位使用
        if (data.canTargetAllies && isTargetEnemy == isCasterEnemy)
        {
            return true;
        }
        
        return false;
    }
}
