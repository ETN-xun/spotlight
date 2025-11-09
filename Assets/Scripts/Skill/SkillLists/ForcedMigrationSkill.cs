using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 强制迁移技能
/// 选择一个在施法者攻击范围内的敌方单位：施法者先向远离目标的方向后退一格，然后将该敌方单位移动到施法者的原位置。
/// </summary>
public class ForcedMigrationSkill : Skill
{
    public ForcedMigrationSkill(SkillDataSO data, Unit caster) : base(data, caster) {}

    public override void Execute(GridCell targetCell, GridManager gridManager)
    {
        if (targetCell == null || caster == null || caster.CurrentCell == null)
        {
            Debug.LogWarning("强制迁移技能：目标格子或施法者无效");
            return;
        }

        Unit target = targetCell.CurrentUnit;
        if (target == null)
        {
            Debug.LogWarning("强制迁移技能：目标位置没有单位");
            return;
        }

        // 验证目标是否为敌方（输入流程已做范围校验，这里做类型校验）
        if (!CanTargetUnit(target))
        {
            Debug.LogWarning("强制迁移技能：目标不合法（非敌方或不可选）");
            return;
        }

        Vector2Int casterPos = caster.CurrentCell.Coordinate;
        Vector2Int targetPos = targetCell.Coordinate;

        // 记录施法者原位置
        GridCell originalCasterCell = caster.CurrentCell;

        // 计算施法者后退方向（远离目标）
        Vector2Int awayDir = NormalizeDirection(casterPos - targetPos);
        Vector2Int backPos = casterPos + awayDir;

        // 检查后退位置有效且可进入
        if (!gridManager.IsValidPosition(backPos))
        {
            Debug.Log("强制迁移技能：后退失败，目标位置越界");
            return;
        }
        GridCell backCell = gridManager.GetCell(backPos);
        if (backCell == null || backCell.CurrentUnit != null || backCell.DestructibleObject != null)
        {
            Debug.Log("强制迁移技能：后退失败，目标格被占用或有建筑");
            return;
        }

        // 施法者后退一格
        caster.MoveTo(backCell);

        // 将敌方单位移动到施法者原位置
        if (originalCasterCell != null && originalCasterCell.CurrentUnit == null)
        {
            var oldTargetCell = target.CurrentCell;
            target.MoveTo(originalCasterCell);
            PlayForcedMigrationEffect(oldTargetCell, originalCasterCell);
            Debug.Log($"{caster.data.unitName} 后退到 ({backPos.x}, {backPos.y})，{target.data.unitName} 被迁移到施法者原位置 ({originalCasterCell.Coordinate.x}, {originalCasterCell.Coordinate.y})");
        }
        else
        {
            Debug.LogWarning("强制迁移技能：施法者原位置不可用，迁移失败");
        }

        // 如果技能配置中有额外伤害，造成伤害
        if (data.baseDamage > 0)
        {
            target.TakeDamage(data.baseDamage);
            Debug.Log($"强制迁移对 {target.data.unitName} 造成 {data.baseDamage} 点伤害");
        }
    }

    /// <summary>
    /// 执行强制迁移效果
    /// </summary>
    /// <param name="target">目标单位</param>
    /// <param name="gridManager">网格管理器</param>
    private void ExecuteForcedMigration(Unit target, GridManager gridManager)
    {
        if (target == null || caster == null) return;
        
        Vector2Int casterPos = caster.CurrentCell.Coordinate;
        Vector2Int targetPos = target.CurrentCell.Coordinate;
        
        // 计算从目标到施法者的方向
        Vector2Int direction = casterPos - targetPos;
        
        // 将方向标准化为单位向量（只能是8个基本方向之一）
        direction = NormalizeDirection(direction);
        
        // 计算目标位置（向施法者方向移动一格）
        Vector2Int newPos = targetPos + direction;
        
        // 检查新位置是否有效
        if (!gridManager.IsValidPosition(newPos))
        {
            Debug.Log($"强制迁移失败：目标位置 ({newPos.x}, {newPos.y}) 超出地图边界");
            return;
        }
        
        GridCell newCell = gridManager.GetCell(newPos);
        if (newCell == null)
        {
            Debug.Log($"强制迁移失败：无法获取目标格子 ({newPos.x}, {newPos.y})");
            return;
        }
        
        // 检查目标位置是否被占用
        if (newCell.CurrentUnit != null)
        {
            Debug.Log($"强制迁移失败：目标位置 ({newPos.x}, {newPos.y}) 被其他单位占用");
            return;
        }
        
        // 检查目标位置是否有不可通过的建筑
        if (newCell.DestructibleObject != null)
        {
            Debug.Log($"强制迁移失败：目标位置 ({newPos.x}, {newPos.y}) 有建筑阻挡");
            return;
        }
        
        // 执行移动
        GridCell oldCell = target.CurrentCell;
        oldCell.CurrentUnit = null;
        // newCell.CurrentUnit = target;
        // target.PlaceAt(newCell);
        target.MoveTo(newCell);
        
        // 播放强制迁移效果
        PlayForcedMigrationEffect(oldCell, newCell);
        
        Debug.Log($"{target.data.unitName} 被强制迁移从 ({targetPos.x}, {targetPos.y}) 到 ({newPos.x}, {newPos.y})");
        
        // 如果技能配置中有额外伤害，造成伤害
        if (data.baseDamage > 0)
        {
            target.TakeDamage(data.baseDamage);
            Debug.Log($"强制迁移对 {target.data.unitName} 造成 {data.baseDamage} 点伤害");
        }
    }

    /// <summary>
    /// 将方向向量标准化为8个基本方向之一
    /// </summary>
    /// <param name="direction">原始方向向量</param>
    /// <returns>标准化后的方向向量</returns>
    private Vector2Int NormalizeDirection(Vector2Int direction)
    {
        // 如果方向为零向量，返回零向量
        if (direction.x == 0 && direction.y == 0)
            return Vector2Int.zero;
        
        // 将每个分量限制在 -1, 0, 1 之间
        int x = Mathf.Clamp(direction.x, -1, 1);
        int y = Mathf.Clamp(direction.y, -1, 1);
        
        return new Vector2Int(x, y);
    }

    /// <summary>
    /// 播放强制迁移效果
    /// </summary>
    /// <param name="fromCell">起始位置</param>
    /// <param name="toCell">目标位置</param>
    private void PlayForcedMigrationEffect(GridCell fromCell, GridCell toCell)
    {
        // 这里可以添加视觉和音效
        Debug.Log($"播放强制迁移效果：从 ({fromCell.Coordinate.x}, {fromCell.Coordinate.y}) 到 ({toCell.Coordinate.x}, {toCell.Coordinate.y})");
        
        // 可以添加粒子效果、牵引线等
        // 例如：
        // ParticleSystem pullEffect = Instantiate(forcedMigrationParticlePrefab);
        // pullEffect.transform.position = fromCell.transform.position;
        // pullEffect.Play();
        
        // 可以添加从施法者到目标的牵引线效果
        // LineRenderer pullLine = Instantiate(pullLinePrefab);
        // pullLine.SetPosition(0, caster.transform.position);
        // pullLine.SetPosition(1, toCell.transform.position);
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
