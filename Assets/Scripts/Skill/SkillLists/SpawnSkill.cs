using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 地形投放技能
/// </summary>

public class SpawnSkill : Skill
{
    public SpawnSkill(SkillDataSO data, Unit caster) : base(data, caster) {}

    public override void Execute(GridCell targetCell, GridManager gridManager)
    {
        // 验证目标位置是否有效（允许任意格子，只要在攻击范围内由外层系统保证）
        if (!IsValidSpawnTarget(targetCell))
        {
            Debug.Log("无法在此位置使用地形投放技能：位置无效");
            return;
        }

        // 若方格上有敌方角色，则对其造成1点伤害
        if (targetCell.CurrentUnit != null)
        {
            var targetUnit = targetCell.CurrentUnit;
            bool isEnemyToCaster = targetUnit.data.isEnemy != caster.data.isEnemy;
            if (isEnemyToCaster)
            {
                targetUnit.TakeDamage(1);
                Debug.Log($"{caster.data.unitName} 对 {targetUnit.data.unitName} 造成了1点伤害");
                return;
            }
            else
            {
                // 有友方单位占用，不召唤虚影
                Debug.Log("目标格子被友方单位占用，无法召唤虚影");
                return;
            }
        }

        // 若方格上没有敌方角色（且无单位占用），则召唤一个自身的虚影（与“闪回位移”相同）
        var phantom = PhantomHelper.CreatePhantom(caster, targetCell, gridManager, 0.5f, 2);
        if (phantom == null)
        {
            Debug.LogError("虚影召唤失败");
            return;
        }
        Debug.Log($"{caster.data.unitName} 在 ({targetCell.Coordinate.x}, {targetCell.Coordinate.y}) 召唤了自身的虚影");
    }
    
    /// <summary>
    /// 检查是否为有效的生成目标位置
    /// </summary>
    /// <param name="targetCell">目标格子</param>
    /// <returns>是否为有效位置</returns>
    private bool IsValidSpawnTarget(GridCell targetCell)
    {
        // 地形投放技能可以在任何位置使用
        // 如果有单位或可摧毁对象，会先清除它们再生成新地形
        return true;
    }
}
