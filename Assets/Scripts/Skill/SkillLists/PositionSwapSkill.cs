using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 移形换影技能
/// 交换自己周围1格范围内的单位与任意单位的位置
/// </summary>
public class PositionSwapSkill : Skill
{
    public PositionSwapSkill(SkillDataSO data, Unit caster) : base(data, caster) {}

    public override void Execute(GridCell targetCell, GridManager gridManager)
    {
        if (targetCell == null || caster == null || caster.CurrentCell == null)
        {
            Debug.LogWarning("移形换影技能：目标格子或施法者无效");
            return;
        }

        Unit targetUnit = targetCell.CurrentUnit;
        if (targetUnit == null)
        {
            Debug.LogWarning("移形换影技能：目标位置没有单位");
            return;
        }

        // 检查目标单位是否在施法者周围1格范围内
        Vector2Int casterPos = caster.CurrentCell.Coordinate;
        Vector2Int targetPos = targetCell.Coordinate;
        
        if (!IsWithinRange(casterPos, targetPos, 1))
        {
            Debug.LogWarning("移形换影技能：目标单位不在1格范围内");
            return;
        }

        // 获取所有可以交换的单位（除了施法者和目标单位）
        List<Unit> availableUnits = GetAvailableSwapTargets(targetUnit);
        
        if (availableUnits.Count == 0)
        {
            Debug.LogWarning("移形换影技能：没有可用的交换目标");
            return;
        }

        // 让玩家选择要交换的单位（这里简化为随机选择）
        Unit swapTarget = availableUnits[Random.Range(0, availableUnits.Count)];
        
        // 执行位置交换
        ExecutePositionSwap(targetUnit, swapTarget, gridManager);
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
    /// 获取所有可用的交换目标
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
    /// 播放交换效果
    /// </summary>
    private void PlaySwapEffect(GridCell cell1, GridCell cell2)
    {
        // TODO: 添加视觉效果
        Debug.Log($"播放位置交换效果：({cell1.Coordinate.x}, {cell1.Coordinate.y}) <-> ({cell2.Coordinate.x}, {cell2.Coordinate.y})");
    }
}