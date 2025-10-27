using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageSkill : Skill
{
    public DamageSkill(SkillDataSO data, Unit caster) : base(data, caster) {}

    public override void Execute(GridCell targetCell, GridManager gridManager)
    {
        // 检查目标格子是否有单位
        if (targetCell.CurrentUnit != null)
        {
            Unit target = targetCell.CurrentUnit;
            
            // 检查是否可以对该目标使用技能
            if (CanTargetUnit(target))
            {
                target.TakeDamage(data.baseDamage);
                Debug.Log($"{caster.data.unitName} 对 {target.data.unitName} 造成了 {data.baseDamage} 点伤害");
            }
            else
            {
                Debug.Log($"无法对 {target.data.unitName} 使用伤害技能：目标类型不匹配");
            }
        }
        // 检查目标格子是否有可摧毁对象
        else if (targetCell.DestructibleObject != null)
        {
            targetCell.DestructibleObject.TakeHits();
            Debug.Log($"{caster.data.unitName} 攻击了可摧毁对象");
        }
        else
        {
            Debug.Log("目标位置没有可攻击的对象");
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
        
        // 如果可以对敌军使用且目标是敌军
        if (data.canTargetEnemies && isTargetEnemy != isCasterEnemy)
        {
            return true;
        }
        
        // 如果可以对友军使用且目标是友军
        if (data.canTargetAllies && isTargetEnemy == isCasterEnemy)
        {
            return true;
        }
        
        return false;
    }
}
