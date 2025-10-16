using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 状态效果管理器
/// 管理单位身上的所有状态异常效果
/// </summary>
public class StatusEffectManager : MonoBehaviour
{
    [Header("状态效果列表")]
    [SerializeField] private List<StatusEffect> activeEffects = new List<StatusEffect>();
    
    /// <summary>
    /// 关联的单位
    /// </summary>
    private Unit unit;
    
    private void Awake()
    {
        unit = GetComponent<Unit>();
    }
    
    /// <summary>
    /// 添加状态效果
    /// </summary>
    /// <param name="statusType">状态类型</param>
    /// <param name="duration">持续时间</param>
    /// <param name="intensity">效果强度</param>
    public void AddStatusEffect(StatusAbnormalType statusType, int duration, float intensity = 1.0f)
    {
        StatusEffect newEffect = CreateStatusEffect(statusType, duration, intensity);
        newEffect.affectedUnit = unit;
        
        // 检查是否可以叠加
        StatusEffect existingEffect = GetStatusEffect(statusType);
        if (existingEffect != null && existingEffect.TryStack(newEffect))
        {
            Debug.Log($"{unit.data.unitName} 的 {existingEffect.statusName} 效果叠加到 {existingEffect.stackCount} 层");
            return;
        }
        
        // 如果不能叠加，添加新效果
        activeEffects.Add(newEffect);
        Debug.Log($"{unit.data.unitName} 获得状态异常: {newEffect.statusName}");
        
        // 触发状态效果UI更新事件
        OnStatusEffectChanged();
    }
    
    /// <summary>
    /// 移除状态效果
    /// </summary>
    /// <param name="statusType">状态类型</param>
    public void RemoveStatusEffect(StatusAbnormalType statusType)
    {
        StatusEffect effect = GetStatusEffect(statusType);
        if (effect != null)
        {
            activeEffects.Remove(effect);
            Debug.Log($"{unit.data.unitName} 移除状态异常: {effect.statusName}");
            OnStatusEffectChanged();
        }
    }
    
    /// <summary>
    /// 获取指定类型的状态效果
    /// </summary>
    /// <param name="statusType">状态类型</param>
    /// <returns>状态效果，如果不存在返回null</returns>
    public StatusEffect GetStatusEffect(StatusAbnormalType statusType)
    {
        return activeEffects.FirstOrDefault(effect => effect.statusType == statusType);
    }
    
    /// <summary>
    /// 检查是否有指定状态效果
    /// </summary>
    /// <param name="statusType">状态类型</param>
    /// <returns>是否存在该状态效果</returns>
    public bool HasStatusEffect(StatusAbnormalType statusType)
    {
        return GetStatusEffect(statusType) != null;
    }
    
    /// <summary>
    /// 获取所有活跃的状态效果
    /// </summary>
    /// <returns>状态效果列表</returns>
    public List<StatusEffect> GetAllActiveEffects()
    {
        return new List<StatusEffect>(activeEffects);
    }
    
    /// <summary>
    /// 应用所有状态效果（回合开始时调用）
    /// </summary>
    public void ApplyAllEffects()
    {
        foreach (var effect in activeEffects.ToList())
        {
            effect.ApplyEffect();
        }
    }
    
    /// <summary>
    /// 更新状态效果持续时间（回合结束时调用）
    /// </summary>
    public void UpdateEffectDurations()
    {
        List<StatusEffect> expiredEffects = new List<StatusEffect>();
        
        foreach (var effect in activeEffects)
        {
            if (effect.DecreaseDuration())
            {
                expiredEffects.Add(effect);
            }
        }
        
        // 移除过期的效果
        foreach (var expiredEffect in expiredEffects)
        {
            activeEffects.Remove(expiredEffect);
            Debug.Log($"{unit.data.unitName} 的 {expiredEffect.statusName} 效果已过期");
        }
        
        if (expiredEffects.Count > 0)
        {
            OnStatusEffectChanged();
        }
    }
    
    /// <summary>
    /// 清除所有状态效果
    /// </summary>
    public void ClearAllEffects()
    {
        if (activeEffects.Count > 0)
        {
            activeEffects.Clear();
            Debug.Log($"{unit.data.unitName} 的所有状态异常效果已清除");
            OnStatusEffectChanged();
        }
    }
    
    /// <summary>
    /// 获取攻击力修正值（考虑缓存污染和过载攻击增强效果）
    /// </summary>
    /// <param name="baseDamage">基础攻击力</param>
    /// <returns>修正后的攻击力</returns>
    public int GetModifiedDamage(int baseDamage)
    {
        float modifiedDamage = baseDamage;
        
        // 缓存污染效果 - 攻击力降低
        StatusEffect cacheCorruption = GetStatusEffect(StatusAbnormalType.CacheCorruption);
        if (cacheCorruption != null)
        {
            float reduction = cacheCorruption.intensity * cacheCorruption.stackCount * 0.2f; // 每层减少20%
            modifiedDamage *= (1f - reduction);
        }
        
        // 过载攻击增强效果 - 攻击力增加
        StatusEffect overloadDamageBonus = GetStatusEffect(StatusAbnormalType.OverloadDamageBonus);
        if (overloadDamageBonus != null)
        {
            modifiedDamage += overloadDamageBonus.intensity;
        }
        
        return Mathf.RoundToInt(modifiedDamage);
    }
    
    /// <summary>
    /// 检查是否受到数据损坏影响
    /// </summary>
    /// <returns>是否随机化行为</returns>
    public bool IsDataCorrupted()
    {
        return HasStatusEffect(StatusAbnormalType.DataCorruption);
    }
    
    /// <summary>
    /// 获取移动范围修正值（考虑过载移动增强效果）
    /// </summary>
    /// <param name="baseMoveRange">基础移动范围</param>
    /// <returns>修正后的移动范围</returns>
    public int GetModifiedMoveRange(int baseMoveRange)
    {
        int modifiedMoveRange = baseMoveRange;
        
        // 过载移动增强效果 - 移动范围增加
        StatusEffect overloadMoveBonus = GetStatusEffect(StatusAbnormalType.OverloadMoveBonus);
        if (overloadMoveBonus != null)
        {
            modifiedMoveRange += Mathf.RoundToInt(overloadMoveBonus.intensity);
        }
        
        return modifiedMoveRange;
    }
    
    /// <summary>
    /// 获取技能冷却时间修正值（考虑系统错误和过载冷却减少效果）
    /// </summary>
    /// <param name="baseCooldown">基础冷却时间</param>
    /// <returns>修正后的冷却时间</returns>
    public int GetModifiedCooldown(int baseCooldown)
    {
        int modifiedCooldown = baseCooldown;
        
        // 系统错误效果 - 冷却时间增加
        StatusEffect systemError = GetStatusEffect(StatusAbnormalType.SystemError);
        if (systemError != null)
        {
            int additionalCooldown = Mathf.RoundToInt(systemError.intensity * systemError.stackCount);
            modifiedCooldown += additionalCooldown;
        }
        
        // 过载冷却减少效果 - 冷却时间减少
        StatusEffect overloadCooldownReduction = GetStatusEffect(StatusAbnormalType.OverloadCooldownReduction);
        if (overloadCooldownReduction != null)
        {
            int cooldownReduction = Mathf.RoundToInt(overloadCooldownReduction.intensity);
            modifiedCooldown -= cooldownReduction;
        }
        
        // 确保冷却时间不会小于0
        return Mathf.Max(0, modifiedCooldown);
    }
    
    /// <summary>
    /// 状态效果变化事件
    /// </summary>
    private void OnStatusEffectChanged()
    {
        // 这里可以触发UI更新或其他系统的响应
        // 例如：EventManager.TriggerEvent("StatusEffectChanged", unit);
    }
    
    /// <summary>
    /// 获取状态效果的显示信息
    /// </summary>
    /// <returns>状态效果描述列表</returns>
    public List<string> GetStatusEffectDescriptions()
    {
        List<string> descriptions = new List<string>();
        foreach (var effect in activeEffects)
        {
            descriptions.Add(effect.GetEffectDescription());
        }
        return descriptions;
    }
    
    /// <summary>
    /// 创建状态效果实例的工厂方法
    /// </summary>
    /// <param name="statusType">状态类型</param>
    /// <param name="duration">持续时间</param>
    /// <param name="intensity">效果强度</param>
    /// <returns>对应的状态效果实例</returns>
    private StatusEffect CreateStatusEffect(StatusAbnormalType statusType, float duration, float intensity)
    {
        int intDuration = Mathf.RoundToInt(duration);
        switch (statusType)
        {
            case StatusAbnormalType.DataCorruption:
                return new DataCorruptionEffect(intDuration, intensity);
            case StatusAbnormalType.SystemError:
                return new SystemErrorEffect(intDuration, intensity);
            case StatusAbnormalType.MemoryLeak:
            case StatusAbnormalType.CacheCorruption:
            default:
                return new StatusEffect(statusType, intDuration, intensity);
        }
    }
    
    /// <summary>
    /// 获取指定状态效果的描述
    /// </summary>
    /// <param name="statusType">状态类型</param>
    /// <returns>状态效果描述，如果不存在则返回空字符串</returns>
    public string GetStatusEffectDescription(StatusAbnormalType statusType)
    {
        var effect = activeEffects.Find(e => e.statusType == statusType);
        return effect?.GetEffectDescription() ?? "";
    }
    
    /// <summary>
    /// 手动更新所有状态效果
    /// </summary>
    public void UpdateEffects()
    {
        UpdateEffectDurations();
    }
}