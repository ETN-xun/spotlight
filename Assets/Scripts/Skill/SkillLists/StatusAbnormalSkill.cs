using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 状态异常技能
/// 对目标施加状态异常效果
/// </summary>
public class StatusAbnormalSkill : Skill
{
    public StatusAbnormalSkill(SkillDataSO data, Unit caster) : base(data, caster) {}

    public override void Execute(GridCell targetCell, GridManager gridManager)
    {
        // 获取影响范围内的所有格子
        List<GridCell> affectedCells = GetAffectedCells(targetCell, gridManager);
        
        foreach (var cell in affectedCells)
        {
            if (cell.CurrentUnit != null)
            {
                Unit target = cell.CurrentUnit;
                
                // 检查是否可以对该目标使用
                if (CanTargetUnit(target))
                {
                    ApplyStatusEffects(target);
                    
                    // 如果技能也造成伤害
                    if (data.baseDamage > 0)
                    {
                        target.TakeDamage(data.baseDamage);
                    }
                }
            }
        }
        
        Debug.Log($"{caster.data.unitName} 使用了状态异常技能: {data.skillName}");
    }
    
    /// <summary>
    /// 获取受影响的格子列表
    /// </summary>
    /// <param name="targetCell">目标格子</param>
    /// <param name="gridManager">网格管理器</param>
    /// <returns>受影响的格子列表</returns>
    private List<GridCell> GetAffectedCells(GridCell targetCell, GridManager gridManager)
    {
        List<GridCell> affectedCells = new List<GridCell>();
        
        // 添加目标格子
        affectedCells.Add(targetCell);
        
        // 如果有范围效果
        if (data.effectRadius > 0)
        {
            Vector2Int center = targetCell.Coordinate;
            for (int dx = -data.effectRadius; dx <= data.effectRadius; dx++)
            {
                for (int dy = -data.effectRadius; dy <= data.effectRadius; dy++)
                {
                    if (dx == 0 && dy == 0) continue; // 跳过中心格子（已添加）
                    
                    // 使用曼哈顿距离而不是正方形区域，确保只包括非斜向格子
                    int manhattanDistance = Mathf.Abs(dx) + Mathf.Abs(dy);
                    if (manhattanDistance > data.effectRadius) continue;
                    
                    Vector2Int pos = center + new Vector2Int(dx, dy);
                    if (gridManager.IsValidPosition(pos))
                    {
                        GridCell cell = gridManager.GetCell(pos);
                        if (cell != null)
                        {
                            affectedCells.Add(cell);
                        }
                    }
                }
            }
        }
        
        // 如果有特定的效果模式
        if (data.effectPattern != null && data.effectPattern.Count > 0)
        {
            Vector2Int center = targetCell.Coordinate;
            foreach (var offset in data.effectPattern)
            {
                Vector2Int pos = center + offset;
                if (gridManager.IsValidPosition(pos))
                {
                    GridCell cell = gridManager.GetCell(pos);
                    if (cell != null && !affectedCells.Contains(cell))
                    {
                        affectedCells.Add(cell);
                    }
                }
            }
        }
        
        return affectedCells;
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
    
    /// <summary>
    /// 对目标施加状态异常效果
    /// </summary>
    /// <param name="target">目标单位</param>
    private void ApplyStatusEffects(Unit target)
    {
        if (target.StatusEffectManager == null) return;
        
        // 根据技能配置的效果列表施加状态异常
        foreach (var effectType in data.effects)
        {
            if (effectType == EffectType.StatusAbnormal)
            {
                // 使用技能配置中的状态异常参数
                // 将“状态异常效果”统一重做为：受到的伤害+1
                StatusAbnormalType statusType = StatusAbnormalType.DamageTakenIncrease;
                int duration = data.statusDuration;
                float intensity = data.statusIntensity;
                
                // 检查是否可以叠加
                if (data.statusCanStack && target.StatusEffectManager.HasStatusEffect(statusType))
                {
                    // 如果已有该状态且可以叠加，直接添加（StatusEffectManager会处理叠加逻辑）
                    target.StatusEffectManager.AddStatusEffect(statusType, duration, intensity);
                }
                else if (!target.StatusEffectManager.HasStatusEffect(statusType))
                {
                    // 如果没有该状态，直接添加
                    target.StatusEffectManager.AddStatusEffect(statusType, duration, intensity);
                }
                
                Debug.Log($"{target.data.unitName} 受到 {statusType} 状态异常影响，持续 {duration} 回合，强度 {intensity}");
            }
        }
    }
}
