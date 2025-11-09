using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 移形换影技能
/// 选择一个距离在施法者 AttackRange 内的单位（可友可敌），
/// 再选择任意一个单位（可友可敌，距离不限），使两者位置互换。
/// </summary>
public class PositionSwapSkill : Skill
{
    public PositionSwapSkill(SkillDataSO data, Unit caster) : base(data, caster) {}

    public override void Execute(GridCell targetCell, GridManager gridManager)
    {
        // 该技能需要两个明确的目标，请使用 ExecuteSwap(cellA, cellB, gridManager)
        Debug.LogWarning("移形换影技能：当前实现需要两个目标，请改用 ExecuteSwap 方法");
    }

    /// <summary>
    /// 检查两个位置是否在指定范围内
    /// </summary>
    private bool IsWithinRange(Vector2Int pos1, Vector2Int pos2, int range)
    {
        int distance = Mathf.Abs(pos1.x - pos2.x) + Mathf.Abs(pos1.y - pos2.y);
        return distance <= range;
    }

    /// <summary>
    /// 获取所有可用的交换目标（不包含施法者和排除的单位）
    /// </summary>
    private List<Unit> GetAvailableSwapTargets(Unit excludeUnit)
    {
        List<Unit> availableUnits = new List<Unit>();
        
        // 遍历所有网格，找到所有单位
        foreach (var kvp in GridManager.Instance._gridDict)
        {
            GridCell cell = kvp.Value;
            if (cell.CurrentUnit != null && 
                cell.CurrentUnit != caster && 
                cell.CurrentUnit != excludeUnit)
            {
                availableUnits.Add(cell.CurrentUnit);
            }
        }
        
        return availableUnits;
    }

    /// <summary>
    /// 执行位置交换
    /// </summary>
    private void ExecutePositionSwap(Unit unit1, Unit unit2, GridManager gridManager)
    {
        if (unit1 == null || unit2 == null || 
            unit1.CurrentCell == null || unit2.CurrentCell == null)
        {
            Debug.LogError("移形换影技能：交换单位或其位置无效");
            return;
        }

        GridCell cell1 = unit1.CurrentCell;
        GridCell cell2 = unit2.CurrentCell;
        
        // 临时清除单位引用
        cell1.CurrentUnit = null;
        cell2.CurrentUnit = null;
        
        // 交换位置
        unit1.PlaceAt(cell2);
        unit2.PlaceAt(cell1);
        
        Debug.Log($"移形换影技能：{unit1.data.unitName} 与 {unit2.data.unitName} 交换了位置");
        
        // 播放交换效果
        PlaySwapEffect(cell1, cell2);
    }

    /// <summary>
    /// 执行位置交换（公开方法，供两次选取后直接调用）
    /// </summary>
    public void ExecuteSwap(GridCell firstTargetCell, GridCell secondTargetCell, GridManager gridManager)
    {
        if (firstTargetCell == null || secondTargetCell == null)
        {
            Debug.LogWarning("移形换影技能：两个目标格子不能为空");
            return;
        }

        var unit1 = firstTargetCell.CurrentUnit;
        var unit2 = secondTargetCell.CurrentUnit;
        if (unit1 == null || unit2 == null)
        {
            Debug.LogWarning("移形换影技能：请选择两个都有单位的格子");
            return;
        }

        ExecutePositionSwap(unit1, unit2, gridManager);
    }

    /// <summary>
    /// 播放交换效果
    /// </summary>
    private void PlaySwapEffect(GridCell cell1, GridCell cell2)
    {
        // TODO: 添加视觉效果
        Debug.Log($"播放位置交换效果：({cell1.Coordinate.x}, {cell1.Coordinate.y}) <-> ({cell2.Coordinate.x}, {cell2.Coordinate.y})");
    }
}
