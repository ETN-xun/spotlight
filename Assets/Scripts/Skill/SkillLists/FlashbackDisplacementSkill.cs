using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        // 创建单位复制对象
        GameObject unitCopyObj = new GameObject($"UnitCopy_{caster.data.unitName}");
        
        // 使用GridManager的CellToWorld方法获取世界坐标，避免空引用
        Vector3 worldPosition = gridManager.CellToWorld(cell.Coordinate);
        unitCopyObj.transform.position = worldPosition;
        
        // 添加单位复制组件
        UnitCopy unitCopy = unitCopyObj.AddComponent<UnitCopy>();
        unitCopy.Initialize(caster, 2); // 持续2回合
        
        // 设置复制的坐标
        unitCopy.coordinate = cell.Coordinate;
        
        // 设置复制到格子
        cell.ObjectOnCell = unitCopy;
        
        Debug.Log($"在位置 ({cell.Coordinate.x}, {cell.Coordinate.y}) 创建了 {caster.data.unitName} 的完全相同复制");
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