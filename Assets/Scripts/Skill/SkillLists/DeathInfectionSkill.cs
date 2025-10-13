using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 死亡感染技能
/// 当单位死亡时触发，对周围单位造成腐蚀伤害并施加状态异常
/// </summary>
public class DeathInfectionSkill : Skill
{
    public DeathInfectionSkill(SkillDataSO data, Unit caster) : base(data, caster) {}

    public override void Execute(GridCell targetCell, GridManager gridManager)
    {
        // 死亡感染技能在单位死亡时自动触发
        // targetCell 是死亡单位所在的格子
        ExecuteDeathInfection(targetCell, gridManager);
    }
    
    /// <summary>
    /// 执行死亡感染效果
    /// </summary>
    /// <param name="deathCell">死亡单位所在格子</param>
    /// <param name="gridManager">网格管理器</param>
    public void ExecuteDeathInfection(GridCell deathCell, GridManager gridManager)
    {
        Vector2Int deathPos = deathCell.Coordinate;
        List<GridCell> affectedCells = GetSurroundingCells(deathPos, gridManager);
        
        Debug.Log($"{caster.data.unitName} 死亡感染触发！影响范围: {affectedCells.Count} 个格子");
        
        foreach (var cell in affectedCells)
        {
            // 对格子内的单位造成腐蚀伤害和状态异常
            if (cell.CurrentUnit != null)
            {
                Unit target = cell.CurrentUnit;
                
                // 造成腐蚀伤害
                int corrosionDamage = data.baseDamage;
                target.TakeDamage(corrosionDamage);
                Debug.Log($"{target.data.unitName} 受到死亡感染腐蚀伤害: {corrosionDamage}");
                
                // 施加状态异常
                ApplyInfectionStatusEffects(target);
            }
            
            // 对格子内的建筑物造成伤害
            if (cell.ObjectOnCell != null)
            {
                cell.ObjectOnCell.TakeDamage(data.baseDamage);
                Debug.Log($"建筑物受到死亡感染腐蚀伤害: {data.baseDamage}");
            }
            
            // 在格子上留下腐蚀效果（可选）
            ApplyCorrosionEffect(cell, gridManager);
        }
    }
    
    /// <summary>
    /// 获取周围的格子
    /// </summary>
    /// <param name="centerPos">中心位置</param>
    /// <param name="gridManager">网格管理器</param>
    /// <returns>周围格子列表</returns>
    private List<GridCell> GetSurroundingCells(Vector2Int centerPos, GridManager gridManager)
    {
        List<GridCell> surroundingCells = new List<GridCell>();
        
        // 获取周围8个方向的格子
        Vector2Int[] directions = {
            new Vector2Int(-1, -1), new Vector2Int(0, -1), new Vector2Int(1, -1),
            new Vector2Int(-1,  0),                        new Vector2Int(1,  0),
            new Vector2Int(-1,  1), new Vector2Int(0,  1), new Vector2Int(1,  1)
        };
        
        foreach (var direction in directions)
        {
            Vector2Int targetPos = centerPos + direction;
            if (gridManager.IsValidPosition(targetPos))
            {
                GridCell cell = gridManager.GetCell(targetPos);
                if (cell != null)
                {
                    surroundingCells.Add(cell);
                }
            }
        }
        
        // 如果技能有额外的影响范围
        if (data.effectRadius > 1)
        {
            for (int dx = -data.effectRadius; dx <= data.effectRadius; dx++)
            {
                for (int dy = -data.effectRadius; dy <= data.effectRadius; dy++)
                {
                    if (dx == 0 && dy == 0) continue; // 跳过中心
                    
                    Vector2Int targetPos = centerPos + new Vector2Int(dx, dy);
                    if (gridManager.IsValidPosition(targetPos))
                    {
                        GridCell cell = gridManager.GetCell(targetPos);
                        if (cell != null && !surroundingCells.Contains(cell))
                        {
                            surroundingCells.Add(cell);
                        }
                    }
                }
            }
        }
        
        return surroundingCells;
    }
    
    /// <summary>
    /// 对目标施加感染状态异常
    /// </summary>
    /// <param name="target">目标单位</param>
    private void ApplyInfectionStatusEffects(Unit target)
    {
        if (target.StatusEffectManager == null) return;
        
        // 施加数据腐蚀状态异常
        target.StatusEffectManager.AddStatusEffect(
            StatusAbnormalType.DataCorruption, 
            duration: 3, // 持续3回合
            intensity: 1.0f
        );
        
        // 有概率施加系统错误
        if (Random.Range(0f, 1f) < 0.5f) // 50%概率
        {
            target.StatusEffectManager.AddStatusEffect(
                StatusAbnormalType.SystemError,
                duration: 2, // 持续2回合
                intensity: 1.0f
            );
        }
        
        Debug.Log($"{target.data.unitName} 被感染，获得状态异常效果");
    }
    
    /// <summary>
    /// 在格子上应用腐蚀效果
    /// </summary>
    /// <param name="cell">目标格子</param>
    /// <param name="gridManager">网格管理器</param>
    private void ApplyCorrosionEffect(GridCell cell, GridManager gridManager)
    {
        // 这里可以添加视觉效果，比如改变地形颜色或添加腐蚀贴图
        // 暂时只输出日志
        Debug.Log($"格子 ({cell.Coordinate.x}, {cell.Coordinate.y}) 被腐蚀感染");
        
        // 如果有腐蚀地形系统，可以在这里设置
        // 例如：将普通地形转换为腐蚀地形
    }
}