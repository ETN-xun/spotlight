using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

/// <summary>
/// 闪回位移技能
/// 允许单位撤销一次位移并在原位置留下残影
/// </summary>
public class FlashbackDisplacementSkill : Skill
{
    // 存储上一次位移的信息
    private static Dictionary<Unit, FlashbackData> flashbackHistory = new Dictionary<Unit, FlashbackData>();
    
    /// <summary>
    /// 闪回数据结构
    /// </summary>
    private class FlashbackData
    {
        public Vector2Int previousPosition;
        public GridCell previousCell;
        public int turnUsed;
        public bool hasUnitCopy;
        
        public FlashbackData(Vector2Int pos, GridCell cell, int turn)
        {
            previousPosition = pos;
            previousCell = cell;
            turnUsed = turn;
            hasUnitCopy = false;
        }
    }
    
    public FlashbackDisplacementSkill(SkillDataSO data, Unit caster) : base(data, caster) {}

    public override void Execute(GridCell targetCell, GridManager gridManager)
    {
        // 闪回位移技能是自我目标技能，targetCell应该是施法者当前位置
        // 检查是否有可用的闪回记录
        if (!flashbackHistory.ContainsKey(caster))
        {
            Debug.Log($"{caster.data.unitName} 没有可用的闪回记录");
            return;
        }
        
        FlashbackData flashbackData = flashbackHistory[caster];
        
        // 检查闪回是否仍然有效（通常限制在同一回合或下一回合使用）
        if (!IsFlashbackValid(flashbackData))
        {
            Debug.Log($"{caster.data.unitName} 的闪回记录已过期");
            flashbackHistory.Remove(caster);
            return;
        }
        
        // 执行闪回位移
        ExecuteFlashback(flashbackData, gridManager);
    }
    
    /// <summary>
    /// 记录单位的位移信息，用于后续闪回
    /// </summary>
    /// <param name="unit">移动的单位</param>
    /// <param name="fromCell">起始格子</param>
    /// <param name="currentTurn">当前回合数</param>
    public static void RecordMovement(Unit unit, GridCell fromCell, int currentTurn)
    {
        if (unit == null || fromCell == null) return;
        
        Debug.Log($"尝试为 {unit.data.unitName} 记录移动，从位置 ({fromCell.Coordinate.x}, {fromCell.Coordinate.y})");
        
        // 检查单位是否有闪回位移技能
        bool hasFlashbackSkill = false;
        if (unit.data.skills != null)
        {
            Debug.Log($"{unit.data.unitName} 拥有 {unit.data.skills.Length} 个技能:");
            foreach (var skill in unit.data.skills)
            {
                if (skill != null)
                {
                    Debug.Log($"  技能: {skill.skillName} (ID: {skill.skillID})");
                    // 检查技能ID或技能名称
                    if (skill.skillID == "flashback_displacement_01" || 
                        skill.skillName.Contains("闪回") || 
                        skill.skillName.Contains("Flashback"))
                    {
                        hasFlashbackSkill = true;
                        Debug.Log($"  找到闪回技能: {skill.skillName}");
                        break;
                    }
                }
                else
                {
                    Debug.Log("  技能为null");
                }
            }
        }
        else
        {
            Debug.Log($"{unit.data.unitName} 没有技能数据");
        }
        
        if (!hasFlashbackSkill) 
        {
            Debug.Log($"{unit.data.unitName} 没有闪回位移技能，不记录移动");
            return;
        }
        
        // 记录位移信息
        FlashbackData flashbackData = new FlashbackData(fromCell.Coordinate, fromCell, currentTurn);
        flashbackHistory[unit] = flashbackData;
        
        Debug.Log($"{unit.data.unitName} 的位移已记录，可以使用闪回位移");
    }
    
    /// <summary>
    /// 检查闪回是否仍然有效
    /// </summary>
    /// <param name="flashbackData">闪回数据</param>
    /// <returns>是否有效</returns>
    private bool IsFlashbackValid(FlashbackData flashbackData)
    {
        // 这里可以根据游戏规则调整有效期
        // 暂时设定为在记录后的2回合内有效
        int currentTurn = GetCurrentTurn(); // 需要实现获取当前回合数的方法
        return (currentTurn - flashbackData.turnUsed) <= 1;
    }
    
    /// <summary>
    /// 执行闪回位移
    /// </summary>
    /// <param name="flashbackData">闪回数据</param>
    /// <param name="gridManager">网格管理器</param>
/// <summary>
    /// 执行闪回位移
    /// </summary>
    /// <param name="flashbackData">闪回数据</param>
    /// <param name="gridManager">网格管理器</param>
    private void ExecuteFlashback(FlashbackData flashbackData, GridManager gridManager)
    {
        GridCell currentCell = caster.CurrentCell;
        GridCell targetCell = flashbackData.previousCell;
        
        // 检查目标位置是否可用
        if (targetCell.CurrentUnit != null)
        {
            Debug.Log($"闪回失败：目标位置 ({targetCell.Coordinate.x}, {targetCell.Coordinate.y}) 被占用");
            return;
        }
        
        // 在当前位置留下残影
        CreateUnitCopy(currentCell, gridManager);
        
        // 移动单位到闪回位置 - 修复位置更新问题
        // 先清理当前位置
        currentCell.CurrentUnit = null;
        
        // 使用PlaceAt方法正确更新单位的逻辑和视觉位置
        caster.PlaceAt(targetCell);
        
        // 播放闪回效果
        PlayFlashbackEffect(currentCell, targetCell);
        
        Debug.Log($"{caster.data.unitName} 闪回到位置 ({targetCell.Coordinate.x}, {targetCell.Coordinate.y})");
        
        // 标记已使用闪回
        flashbackData.hasUnitCopy = true;
    }
    
    /// <summary>
    /// 在指定位置创建残影
    /// </summary>
    /// <param name="cell">残影位置</param>
    /// <param name="gridManager">网格管理器</param>
/// <summary>
    /// 在指定位置创建单位复制
    /// </summary>
    /// <param name="cell">复制位置</param>
    /// <param name="gridManager">网格管理器</param>
    private void CreateUnitCopy(GridCell cell, GridManager gridManager)
    {
        // 直接复制原始单位的GameObject（相当于Ctrl+C、Ctrl+V）
        GameObject unitCopyObj = Object.Instantiate(caster.gameObject);
        
        // 设置GridManager为父对象
        unitCopyObj.transform.SetParent(gridManager.transform);
        
        // 设置名称以区分复制品
        unitCopyObj.name = $"{caster.data.unitName}_FlashbackCopy";
        
        // 使用GridManager的CellToWorld方法获取世界坐标
        Vector3 worldPosition = gridManager.CellToWorld(cell.Coordinate);
        unitCopyObj.transform.position = worldPosition;
        
        // 获取复制的Unit组件
        Unit copiedUnit = unitCopyObj.GetComponent<Unit>();
        if (copiedUnit != null)
        {
            // 设置复制体的透明度为50%
            SetCopyTransparency(unitCopyObj, 0.5f);
            
            // 保持复制体的isEnemy属性与施法者相同，这样敌人AI会将其识别为友军并攻击
            // 不需要修改isEnemy属性，因为复制体应该保持与原单位相同的阵营
            
            // 添加闪回复制标识组件
            FlashbackCopyTag copyTag = unitCopyObj.GetComponent<FlashbackCopyTag>();
            if (copyTag == null)
            {
                copyTag = unitCopyObj.AddComponent<FlashbackCopyTag>();
            }
            copyTag.Initialize(caster, 2); // 持续2回合
            
            // 设置复制体的坐标
            copiedUnit.PlaceAt(cell); // 使用PlaceAt方法正确设置位置
            
            Debug.Log($"在位置 ({cell.Coordinate.x}, {cell.Coordinate.y}) 创建了 {caster.data.unitName} 的完全相同复制（50%透明度）");
        }
        else
        {
            Debug.LogError("复制的GameObject没有Unit组件");
            Object.Destroy(unitCopyObj);
        }
    }
    
    /// <summary>
    /// 设置复制体的透明度
    /// </summary>
    /// <param name="copyObj">复制的GameObject</param>
    /// <param name="alpha">透明度值（0-1）</param>
    private void SetCopyTransparency(GameObject copyObj, float alpha)
    {
        // 设置Spine动画的透明度
        SkeletonAnimation skeletonAnimation = copyObj.GetComponentInChildren<SkeletonAnimation>();
        if (skeletonAnimation != null && skeletonAnimation.skeleton != null)
        {
            skeletonAnimation.skeleton.A = alpha;
        }
        
        // 设置所有SpriteRenderer的透明度
        SpriteRenderer[] spriteRenderers = copyObj.GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer renderer in spriteRenderers)
        {
            Color color = renderer.color;
            color.a = alpha;
            renderer.color = color;
        }
        
        // 设置所有CanvasRenderer的透明度（UI元素）
        CanvasRenderer[] canvasRenderers = copyObj.GetComponentsInChildren<CanvasRenderer>();
        foreach (CanvasRenderer renderer in canvasRenderers)
        {
            renderer.SetAlpha(alpha);
        }
        
        Debug.Log($"设置复制体透明度为 {alpha * 100}%");
    }
    
    /// <summary>
    /// 播放闪回效果
    /// </summary>
    /// <param name="fromCell">起始位置</param>
    /// <param name="toCell">目标位置</param>
    private void PlayFlashbackEffect(GridCell fromCell, GridCell toCell)
    {
        // 这里可以添加视觉和音效
        Debug.Log($"播放闪回效果：从 ({fromCell.Coordinate.x}, {fromCell.Coordinate.y}) 到 ({toCell.Coordinate.x}, {toCell.Coordinate.y})");
        
        // 可以添加粒子效果、闪光等
        // 例如：
        // ParticleSystem flashEffect = Instantiate(flashbackParticlePrefab);
        // flashEffect.transform.position = fromCell.transform.position;
        // flashEffect.Play();
    }
    
    /// <summary>
    /// 获取当前回合数
    /// </summary>
    /// <returns>当前回合数</returns>
    private int GetCurrentTurn()
    {
        return GameManager.Instance != null ? GameManager.Instance.CurrentTurn : 1;
    }
    
    /// <summary>
    /// 检查单位是否可以使用闪回位移
    /// </summary>
    /// <param name="unit">要检查的单位</param>
    /// <returns>是否可以使用</returns>
    public static bool CanUseFlashback(Unit unit)
    {
        return flashbackHistory.ContainsKey(unit);
    }

    /// <summary>
    /// 检查当前是否可以执行闪回（包含记录存在、未过期、目标格未被占用）
    /// </summary>
    /// <param name="unit">要检查的单位</param>
    /// <returns>是否可以执行闪回</returns>
    public static bool CanExecuteFlashback(Unit unit)
    {
        if (unit == null) return false;
        if (!flashbackHistory.ContainsKey(unit)) return false;

        var data = flashbackHistory[unit];
        if (data == null || data.previousCell == null) return false;

        int currentTurn = GameManager.Instance != null ? GameManager.Instance.CurrentTurn : 1;
        // 过期不可用，同时清理记录
        if ((currentTurn - data.turnUsed) > 1)
        {
            flashbackHistory.Remove(unit);
            return false;
        }

        // 目标格被占用不可用
        if (data.previousCell.CurrentUnit != null)
        {
            return false;
        }

        return true;
    }
    
    /// <summary>
    /// 清理过期的闪回记录
    /// </summary>
    public static void CleanupExpiredFlashbacks()
    {
        List<Unit> toRemove = new List<Unit>();
        int currentTurn = GameManager.Instance != null ? GameManager.Instance.CurrentTurn : 1;
        
        foreach (var kvp in flashbackHistory)
        {
            if ((currentTurn - kvp.Value.turnUsed) > 1)
            {
                toRemove.Add(kvp.Key);
            }
        }
        
        foreach (var unit in toRemove)
        {
            flashbackHistory.Remove(unit);
        }
    }
}
