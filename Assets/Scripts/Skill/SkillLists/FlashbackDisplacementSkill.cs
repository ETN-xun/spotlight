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
        public bool hasAfterimage;
        
        public FlashbackData(Vector2Int pos, GridCell cell, int turn)
        {
            previousPosition = pos;
            previousCell = cell;
            turnUsed = turn;
            hasAfterimage = false;
        }
    }
    
    public FlashbackDisplacementSkill(SkillDataSO data, Unit caster) : base(data, caster) {}

    public override void Execute(GridCell targetCell, GridManager gridManager)
    {
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
        
        // 检查单位是否有闪回位移技能
        bool hasFlashbackSkill = false;
        if (unit.data.skills != null)
        {
            foreach (var skill in unit.data.skills)
            {
                if (skill.skillName.Contains("闪回") || skill.skillName.Contains("Flashback"))
                {
                    hasFlashbackSkill = true;
                    break;
                }
            }
        }
        
        if (!hasFlashbackSkill) return;
        
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
        CreateAfterimage(currentCell, gridManager);
        
        // 移动单位到闪回位置
        currentCell.CurrentUnit = null;
        targetCell.CurrentUnit = caster;
        caster.PlaceAt(targetCell);
        
        // 播放闪回效果
        PlayFlashbackEffect(currentCell, targetCell);
        
        Debug.Log($"{caster.data.unitName} 闪回到位置 ({targetCell.Coordinate.x}, {targetCell.Coordinate.y})");
        
        // 标记已使用闪回
        flashbackData.hasAfterimage = true;
        
        // 移除闪回记录（一次性使用）
        flashbackHistory.Remove(caster);
    }
    
    /// <summary>
    /// 在指定位置创建残影
    /// </summary>
    /// <param name="cell">残影位置</param>
    /// <param name="gridManager">网格管理器</param>
    private void CreateAfterimage(GridCell cell, GridManager gridManager)
    {
        // 创建残影对象
        GameObject afterimageObj = new GameObject($"Afterimage_{caster.data.unitName}");
        afterimageObj.transform.position = cell.GridCellController.transform.position;
        
        // 添加残影组件
        Afterimage afterimage = afterimageObj.AddComponent<Afterimage>();
        afterimage.Initialize(caster, data.spawnHits); // 使用技能数据中的持续时间
        
        // 设置残影到格子
        cell.ObjectOnCell = afterimage;
        
        Debug.Log($"在位置 ({cell.Coordinate.x}, {cell.Coordinate.y}) 创建了 {caster.data.unitName} 的残影");
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