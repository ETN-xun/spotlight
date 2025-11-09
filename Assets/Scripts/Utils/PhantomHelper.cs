using UnityEngine;
using Spine.Unity;

/// <summary>
/// 虚影创建辅助类：复用“闪回位移”中的残影/复制体创建逻辑
/// </summary>
public static class PhantomHelper
{
    /// <summary>
    /// 在指定格子创建施法者的虚影（复制体），带透明度与持续回合设置
    /// </summary>
    /// <param name="caster">施法者单位</param>
    /// <param name="cell">目标格子</param>
    /// <param name="gridManager">网格管理器</param>
    /// <param name="alpha">透明度(0-1)</param>
    /// <param name="durationTurns">持续回合数</param>
    /// <returns>创建出的复制体Unit（若失败返回null）</returns>
    public static Unit CreatePhantom(Unit caster, GridCell cell, GridManager gridManager, float alpha = 0.5f, int durationTurns = 2)
    {
        if (caster == null || cell == null || gridManager == null)
        {
            Debug.LogError("CreatePhantom 参数无效");
            return null;
        }

        // 复制施法者的GameObject
        GameObject unitCopyObj = Object.Instantiate(caster.gameObject);
        unitCopyObj.transform.SetParent(gridManager.transform);
        unitCopyObj.name = $"{caster.data.unitName}_FlashbackCopy";

        // 放置到目标格子的世界坐标
        Vector3 worldPosition = gridManager.CellToWorld(cell.Coordinate);
        unitCopyObj.transform.position = worldPosition;

        // 获取复制体的Unit组件
        Unit copiedUnit = unitCopyObj.GetComponent<Unit>();
        if (copiedUnit == null)
        {
            Debug.LogError("复制的GameObject没有Unit组件");
            Object.Destroy(unitCopyObj);
            return null;
        }

        // 设置透明度
        SetTransparency(unitCopyObj, alpha);

        // 添加并初始化闪回复制标记组件
        FlashbackCopyTag copyTag = unitCopyObj.GetComponent<FlashbackCopyTag>();
        if (copyTag == null)
        {
            copyTag = unitCopyObj.AddComponent<FlashbackCopyTag>();
        }
        copyTag.Initialize(caster, durationTurns);

        // 使用PlaceAt正确占用格子与刷新引用
        copiedUnit.PlaceAt(cell);

        Debug.Log($"在位置 ({cell.Coordinate.x}, {cell.Coordinate.y}) 创建了 {caster.data.unitName} 的虚影（透明度 {alpha}，持续 {durationTurns} 回合）");
        return copiedUnit;
    }

    /// <summary>
    /// 设置复制体透明度（Spine与SpriteRenderer与CanvasRenderer）
    /// </summary>
    private static void SetTransparency(GameObject obj, float alpha)
    {
        // Spine动画透明度
        SkeletonAnimation skeletonAnimation = obj.GetComponentInChildren<SkeletonAnimation>();
        if (skeletonAnimation != null && skeletonAnimation.skeleton != null)
        {
            skeletonAnimation.skeleton.A = alpha;
        }

        // 所有SpriteRenderer透明度
        SpriteRenderer[] spriteRenderers = obj.GetComponentsInChildren<SpriteRenderer>();
        foreach (var renderer in spriteRenderers)
        {
            Color c = renderer.color;
            c.a = alpha;
            renderer.color = c;
        }

        // 所有CanvasRenderer透明度（UI）
        CanvasRenderer[] canvasRenderers = obj.GetComponentsInChildren<CanvasRenderer>();
        foreach (var cr in canvasRenderers)
        {
            cr.SetAlpha(alpha);
        }
    }
}

