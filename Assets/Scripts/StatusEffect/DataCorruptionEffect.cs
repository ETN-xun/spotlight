using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 数据损坏状态异常效果
/// 影响单位的移动和攻击方向，使其随机化
/// </summary>
public class DataCorruptionEffect : StatusEffect
{
    /// <summary>
    /// 构造函数
    /// </summary>
    public DataCorruptionEffect(int duration, float intensity = 1.0f) 
        : base(StatusAbnormalType.DataCorruption, duration, intensity)
    {
    }



    /// <summary>
    /// 随机化移动方向
    /// 在MovementSystem中调用此方法来获取随机化的移动目标
    /// </summary>
    /// <param name="originalTarget">原始目标位置</param>
    /// <param name="gridManager">网格管理器</param>
    /// <returns>随机化后的目标位置</returns>
    public Vector2Int RandomizeMovementTarget(Vector2Int originalTarget, GridManager gridManager)
    {
        if (affectedUnit == null || gridManager == null) return originalTarget;

        Vector2Int currentPos = affectedUnit.CurrentCell.Coordinate;
        
        // 获取当前位置周围的所有可移动位置
        List<Vector2Int> validMovePositions = new List<Vector2Int>();
        
        // 检查8个方向
        Vector2Int[] directions = {
            new Vector2Int(0, 1),   // 上
            new Vector2Int(1, 1),   // 右上
            new Vector2Int(1, 0),   // 右
            new Vector2Int(1, -1),  // 右下
            new Vector2Int(0, -1),  // 下
            new Vector2Int(-1, -1), // 左下
            new Vector2Int(-1, 0),  // 左
            new Vector2Int(-1, 1)   // 左上
        };

        foreach (var direction in directions)
        {
            Vector2Int targetPos = currentPos + direction;
            
            // 检查位置是否有效且可移动
            if (gridManager.IsValidPosition(targetPos))
            {
                GridCell targetCell = gridManager.GetCell(targetPos);
                if (targetCell != null && CanMoveToCell(targetCell))
                {
                    validMovePositions.Add(targetPos);
                }
            }
        }

        // 如果没有有效的移动位置，返回原始目标
        if (validMovePositions.Count == 0)
        {
            Debug.Log($"{affectedUnit.data.unitName} 数据损坏：没有有效的随机移动位置");
            return originalTarget;
        }

        // 随机选择一个有效位置
        Vector2Int randomTarget = validMovePositions[Random.Range(0, validMovePositions.Count)];
        
        Debug.Log($"{affectedUnit.data.unitName} 数据损坏：移动目标从 {originalTarget} 随机化为 {randomTarget}");
        return randomTarget;
    }

    /// <summary>
    /// 随机化攻击目标
    /// 在攻击系统中调用此方法来获取随机化的攻击目标
    /// </summary>
    /// <param name="originalTarget">原始攻击目标</param>
    /// <param name="gridManager">网格管理器</param>
    /// <param name="attackRange">攻击范围</param>
    /// <returns>随机化后的攻击目标</returns>
    public GridCell RandomizeAttackTarget(GridCell originalTarget, GridManager gridManager, int attackRange)
    {
        if (affectedUnit == null || gridManager == null) return originalTarget;

        Vector2Int currentPos = affectedUnit.CurrentCell.Coordinate;
        List<GridCell> validTargets = new List<GridCell>();

        // 获取攻击范围内的所有有效目标
        for (int dx = -attackRange; dx <= attackRange; dx++)
        {
            for (int dy = -attackRange; dy <= attackRange; dy++)
            {
                if (dx == 0 && dy == 0) continue; // 跳过自己的位置

                Vector2Int targetPos = currentPos + new Vector2Int(dx, dy);
                
                if (gridManager.IsValidPosition(targetPos))
                {
                    GridCell targetCell = gridManager.GetCell(targetPos);
                    if (targetCell != null && CanAttackCell(targetCell))
                    {
                        validTargets.Add(targetCell);
                    }
                }
            }
        }

        // 如果没有有效的攻击目标，返回原始目标
        if (validTargets.Count == 0)
        {
            Debug.Log($"{affectedUnit.data.unitName} 数据损坏：没有有效的随机攻击目标");
            return originalTarget;
        }

        // 随机选择一个有效目标
        GridCell randomTarget = validTargets[Random.Range(0, validTargets.Count)];
        
        Debug.Log($"{affectedUnit.data.unitName} 数据损坏：攻击目标随机化");
        return randomTarget;
    }

    /// <summary>
    /// 检查是否可以移动到指定格子
    /// </summary>
    /// <param name="cell">目标格子</param>
    /// <returns>是否可以移动</returns>
    private bool CanMoveToCell(GridCell cell)
    {
        // 检查格子是否被占用
        if (cell.CurrentUnit != null) return false;
        
        // 检查格子是否有建筑
        if (cell.DestructibleObject != null && !cell.DestructibleObject.CanUnitPassThrough(affectedUnit)) return false;
        
        //检查格子是否有残影
        if (cell.ObjectOnCell != null && !cell.ObjectOnCell.CanUnitPassThrough(affectedUnit)) return false;
        
        // 检查地形是否可通行
        if (cell.TerrainData != null && !cell.TerrainData.isWalkable) return false;
        
        return true;
    }

    /// <summary>
    /// 检查是否可以攻击指定格子
    /// </summary>
    /// <param name="cell">目标格子</param>
    /// <returns>是否可以攻击</returns>
    private bool CanAttackCell(GridCell cell)
    {
        // 检查格子是否有单位
        if (cell.CurrentUnit != null)
        {
            // 检查是否为敌对单位
            bool isTargetEnemy = cell.CurrentUnit.data.isEnemy;
            bool isCasterEnemy = affectedUnit.data.isEnemy;
            return isTargetEnemy != isCasterEnemy; // 只能攻击敌对单位
        }
        
        // 检查格子是否有可攻击的建筑
        if (cell.DestructibleObject != null && cell.DestructibleObject.CanTakeDamage())
        {
            return true;
        }
        // 检查格子是否有可攻击的残影
        if (cell.ObjectOnCell != null && cell.ObjectOnCell.CanTakeDamage())
        {
            return true;
        }
        
        return false;
    }

    /// <summary>
    /// 获取效果的详细描述
    /// </summary>
    /// <returns>效果描述</returns>
    public override string GetEffectDescription()
    {
        string desc = $"数据损坏：移动和攻击方向随机化";
        desc += $" - 剩余 {remainingTurns} 回合";
        return desc;
    }
}