using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 镜像复制技能
/// 复制一个我方单位
/// </summary>
public class MirrorCopySkill : Skill
{
    public MirrorCopySkill(SkillDataSO data, Unit caster) : base(data, caster) {}

    public override void Execute(GridCell targetCell, GridManager gridManager)
    {
        if (targetCell == null || caster == null)
        {
            Debug.LogWarning("镜像复制技能：目标格子或施法者无效");
            return;
        }

        Unit targetUnit = targetCell.CurrentUnit;
        if (targetUnit == null)
        {
            Debug.LogWarning("镜像复制技能：目标位置没有单位");
            return;
        }

        // 检查目标单位是否为我方单位
        if (targetUnit.data.isEnemy)
        {
            Debug.LogWarning("镜像复制技能：只能复制我方单位");
            return;
        }

        // 查找空闲位置来放置复制的单位
        GridCell emptyCell = FindNearestEmptyCell(targetCell, gridManager);
        if (emptyCell == null)
        {
            Debug.LogWarning("镜像复制技能：没有空闲位置放置复制的单位");
            return;
        }

        // 创建复制的单位
        Unit copiedUnit = CreateCopiedUnit(targetUnit, emptyCell, gridManager);
        if (copiedUnit != null)
        {
            Debug.Log($"镜像复制技能：成功复制了 {targetUnit.data.unitName}");
            PlayCopyEffect(targetCell, emptyCell);
        }
    }

    /// <summary>
    /// 查找最近的空闲格子
    /// </summary>
    private GridCell FindNearestEmptyCell(GridCell centerCell, GridManager gridManager)
    {
        Vector2Int center = centerCell.Coordinate;
        
        // 从距离1开始，逐渐扩大搜索范围
        for (int range = 1; range <= 5; range++)
        {
            for (int dx = -range; dx <= range; dx++)
            {
                for (int dy = -range; dy <= range; dy++)
                {
                    // 只检查当前范围边界上的格子
                    if (Mathf.Abs(dx) != range && Mathf.Abs(dy) != range)
                        continue;

                    Vector2Int pos = center + new Vector2Int(dx, dy);
                    
                    if (gridManager.IsValidPosition(pos))
                    {
                        GridCell cell = gridManager.GetCell(pos);
                        if (cell != null && cell.CurrentUnit == null)
                        {
                            return cell;
                        }
                    }
                }
            }
        }
        
        return null;
    }

    /// <summary>
    /// 创建复制的单位
    /// </summary>
    private Unit CreateCopiedUnit(Unit originalUnit, GridCell targetCell, GridManager gridManager)
    {
        if (originalUnit == null || originalUnit.data == null || targetCell == null)
        {
            Debug.LogError("镜像复制技能：无法创建复制单位，参数无效");
            return null;
        }

        // 实例化单位预制体
        GameObject copiedObject;
        if (originalUnit.data.unitPrefab != null)
        {
            copiedObject = Object.Instantiate(originalUnit.data.unitPrefab);
        }
        else
        {
            // 如果没有预制体，复制原始单位的GameObject
            copiedObject = Object.Instantiate(originalUnit.gameObject);
        }

        // 获取Unit组件
        Unit copiedUnit = copiedObject.GetComponent<Unit>();
        if (copiedUnit == null)
        {
            copiedUnit = copiedObject.AddComponent<Unit>();
        }

        // 设置单位数据
        copiedUnit.data = originalUnit.data;
        copiedUnit.InitFromData();

        // 放置到目标位置
        copiedUnit.PlaceAt(targetCell);

        // 设置名称以区分复制品
        copiedObject.name = originalUnit.data.unitName + "_Copy";

        // 添加复制品标识（可选，用于特殊逻辑）
        MirrorCopyTag copyTag = copiedObject.GetComponent<MirrorCopyTag>();
        if (copyTag == null)
        {
            copyTag = copiedObject.AddComponent<MirrorCopyTag>();
        }
        copyTag.originalUnit = originalUnit;
        copyTag.creationTime = Time.time;

        return copiedUnit;
    }

    /// <summary>
    /// 播放复制效果
    /// </summary>
    private void PlayCopyEffect(GridCell originalCell, GridCell copyCell)
    {
        // TODO: 添加视觉效果
        Debug.Log($"播放镜像复制效果：从 ({originalCell.Coordinate.x}, {originalCell.Coordinate.y}) 复制到 ({copyCell.Coordinate.x}, {copyCell.Coordinate.y})");
    }
}

/// <summary>
/// 镜像复制标签组件
/// 用于标识复制的单位并存储相关信息
/// </summary>
public class MirrorCopyTag : MonoBehaviour
{
    [Header("复制信息")]
    public Unit originalUnit;           // 原始单位引用
    public float creationTime;          // 创建时间
    public bool isTemporary = false;    // 是否为临时复制品
    public float lifeTime = -1f;        // 生存时间（-1表示永久）

    private void Update()
    {
        // 如果设置了生存时间，检查是否需要销毁
        if (isTemporary && lifeTime > 0 && Time.time - creationTime >= lifeTime)
        {
            DestroyCopy();
        }
    }

    /// <summary>
    /// 销毁复制品
    /// </summary>
    public void DestroyCopy()
    {
        Unit unit = GetComponent<Unit>();
        if (unit != null && unit.CurrentCell != null)
        {
            unit.CurrentCell.CurrentUnit = null;
        }
        
        Debug.Log($"镜像复制品 {gameObject.name} 已消失");
        Destroy(gameObject);
    }

    /// <summary>
    /// 设置临时复制品
    /// </summary>
    /// <param name="duration">持续时间（秒）</param>
    public void SetTemporary(float duration)
    {
        isTemporary = true;
        lifeTime = duration;
    }
}