using System;
using UnityEngine;

/// <summary>
/// 状态异常效果基类
/// 管理状态异常的持续时间和效果
/// </summary>
[Serializable]
public class StatusEffect
{
    [Header("基础信息")]
    [Tooltip("状态异常类型")]
    public StatusAbnormalType statusType;
    
    [Tooltip("状态异常名称")]
    public string statusName;
    
    [Tooltip("状态异常描述")]
    public string description;
    
    [Tooltip("状态异常图标")]
    public Sprite statusIcon;
    
    [Header("持续时间")]
    [Tooltip("剩余持续回合数")]
    public int remainingTurns;
    
    [Tooltip("最大持续回合数")]
    public int maxTurns;
    
    [Header("效果强度")]
    [Tooltip("效果强度值")]
    public float intensity = 1.0f;
    
    [Tooltip("是否可叠加")]
    public bool canStack = false;
    
    [Tooltip("叠加层数")]
    public int stackCount = 1;
    
    /// <summary>
    /// 受影响的单位
    /// </summary>
    [NonSerialized]
    public Unit affectedUnit;
    
    /// <summary>
    /// 构造函数
    /// </summary>
    public StatusEffect(StatusAbnormalType type, int duration, float intensity = 1.0f)
    {
        this.statusType = type;
        this.remainingTurns = duration;
        this.maxTurns = duration;
        this.intensity = intensity;
        this.stackCount = 1;
        
        // 根据类型设置基础信息
        SetStatusInfo();
    }
    
    /// <summary>
    /// 根据状态类型设置基础信息
    /// </summary>
    private void SetStatusInfo()
    {
        switch (statusType)
        {
            case StatusAbnormalType.DataCorruption:
                statusName = "数据损坏";
                description = "移动和攻击方向随机化";
                canStack = false;
                break;
                
            case StatusAbnormalType.SystemError:
                statusName = "系统错误";
                description = "技能冷却时间增加";
                canStack = true;
                break;
                
            case StatusAbnormalType.MemoryLeak:
                statusName = "内存泄漏";
                description = "每回合损失能量";
                canStack = true;
                break;
                
            case StatusAbnormalType.CacheCorruption:
                statusName = "缓存污染";
                description = "攻击力降低";
                canStack = true;
                break;
        }
    }
    
    /// <summary>
    /// 应用状态效果（回合开始时调用）
    /// </summary>
    public virtual void ApplyEffect()
    {
        if (affectedUnit == null) return;
        
        switch (statusType)
        {
            case StatusAbnormalType.DataCorruption:
                ApplyDataCorruption();
                break;
                
            case StatusAbnormalType.SystemError:
                ApplySystemError();
                break;
                
            case StatusAbnormalType.MemoryLeak:
                ApplyMemoryLeak();
                break;
                
            case StatusAbnormalType.CacheCorruption:
                ApplyCacheCorruption();
                break;
        }
    }
    
    /// <summary>
    /// 数据损坏效果 - 移动和攻击随机化
    /// </summary>
    private void ApplyDataCorruption()
    {
        // 这个效果在移动和攻击时处理，这里只是标记
        Debug.Log($"{affectedUnit.data.unitName} 受到数据损坏影响，行为将随机化");
    }
    
    /// <summary>
    /// 系统错误效果 - 技能冷却时间增加
    /// </summary>
    private void ApplySystemError()
    {
        // 技能冷却时间增加在技能使用时处理
        Debug.Log($"{affectedUnit.data.unitName} 受到系统错误影响，技能冷却时间增加");
    }
    
    /// <summary>
    /// 内存泄漏效果 - 每回合损失能量
    /// </summary>
    private void ApplyMemoryLeak()
    {
        int energyLoss = Mathf.RoundToInt(intensity * stackCount);
        // 这里需要能量系统的支持，暂时用Debug输出
        Debug.Log($"{affectedUnit.data.unitName} 因内存泄漏损失 {energyLoss} 点能量");
    }
    
    /// <summary>
    /// 缓存污染效果 - 攻击力降低
    /// </summary>
    private void ApplyCacheCorruption()
    {
        // 攻击力降低效果在攻击计算时处理
        Debug.Log($"{affectedUnit.data.unitName} 受到缓存污染影响，攻击力降低");
    }
    
    /// <summary>
    /// 减少持续时间
    /// </summary>
    /// <returns>是否已过期</returns>
    public bool DecreaseDuration()
    {
        remainingTurns--;
        return remainingTurns <= 0;
    }
    
    /// <summary>
    /// 尝试叠加效果
    /// </summary>
    /// <param name="newEffect">新的状态效果</param>
    /// <returns>是否成功叠加</returns>
    public bool TryStack(StatusEffect newEffect)
    {
        if (!canStack || statusType != newEffect.statusType) return false;
        
        stackCount++;
        // 刷新持续时间为较长的那个
        remainingTurns = Mathf.Max(remainingTurns, newEffect.remainingTurns);
        
        return true;
    }
    
    /// <summary>
    /// 获取效果描述
    /// </summary>
    public virtual string GetEffectDescription()
    {
        string desc = description;
        if (canStack && stackCount > 1)
        {
            desc += $" (叠加 {stackCount} 层)";
        }
        desc += $" - 剩余 {remainingTurns} 回合";
        return desc;
    }
}