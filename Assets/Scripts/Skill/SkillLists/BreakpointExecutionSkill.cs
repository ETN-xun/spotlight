using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 断点斩杀技能
/// 代码刺客-影的特殊技能：若敌人处于异常状态，直接秒杀
/// </summary>
public class BreakpointExecutionSkill : Skill
{
    public BreakpointExecutionSkill(SkillDataSO data, Unit caster) : base(data, caster) {}

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
                    // 检查目标是否处于异常状态
                    if (HasAnyStatusAbnormal(target))
                    {
                        // 直接秒杀
                        ExecuteTarget(target);
                        Debug.Log($"{caster.data.unitName} 对 {target.data.unitName} 使用断点斩杀！目标处于异常状态，被直接秒杀！");
                    }
                    else
                    {
                        // 如果目标没有异常状态，造成普通伤害
                        int damage = data.baseDamage;
                        target.TakeDamage(damage);
                        Debug.Log($"{caster.data.unitName} 对 {target.data.unitName} 使用断点斩杀，但目标无异常状态，造成 {damage} 点伤害");
                    }
                }
            }
        }
        
        Debug.Log($"{caster.data.unitName} 使用了断点斩杀技能");
    }

    /// <summary>
    /// 检查目标是否有任何状态异常
    /// </summary>
    /// <param name="target">目标单位</param>
    /// <returns>是否有状态异常</returns>
    private bool HasAnyStatusAbnormal(Unit target)
    {
        if (target.StatusEffectManager == null) return false;
        
        // 检查所有已知的状态异常类型
        return target.StatusEffectManager.HasStatusEffect(StatusAbnormalType.DataCorruption) ||
               target.StatusEffectManager.HasStatusEffect(StatusAbnormalType.SystemError) ||
               target.StatusEffectManager.HasStatusEffect(StatusAbnormalType.MemoryLeak) ||
               target.StatusEffectManager.HasStatusEffect(StatusAbnormalType.CacheCorruption);
    }

    /// <summary>
    /// 直接秒杀目标
    /// </summary>
    /// <param name="target">目标单位</param>
    private void ExecuteTarget(Unit target)
    {
        // 直接将目标生命值设为0，触发死亡
        target.TakeDamage(target.currentHP);
        
        // 可以添加特殊的视觉效果或音效
        // TODO: 添加断点斩杀的特殊效果
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
}