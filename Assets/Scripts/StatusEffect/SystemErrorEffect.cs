using UnityEngine;

/// <summary>
/// 系统错误状态异常效果
/// 增加技能冷却时间，影响技能使用频率
/// </summary>
public class SystemErrorEffect : StatusEffect
{
    /// <summary>
    /// 构造函数
    /// </summary>
    public SystemErrorEffect(int duration, float intensity = 1.0f) 
        : base(StatusAbnormalType.SystemError, duration, intensity)
    {
    }



    /// <summary>
    /// 计算修正后的技能冷却时间
    /// 在技能系统中调用此方法来获取增加后的冷却时间
    /// </summary>
    /// <param name="baseCooldown">基础冷却时间</param>
    /// <returns>修正后的冷却时间</returns>
    public int GetModifiedCooldown(int baseCooldown)
    {
        if (affectedUnit == null) return baseCooldown;

        // 每层系统错误增加1回合冷却时间，强度影响增加量
        int additionalCooldown = Mathf.RoundToInt(intensity * stackCount);
        int modifiedCooldown = baseCooldown + additionalCooldown;
        
        Debug.Log($"{affectedUnit.data.unitName} 系统错误：技能冷却时间从 {baseCooldown} 增加到 {modifiedCooldown} (增加 {additionalCooldown} 回合)");
        
        return modifiedCooldown;
    }

    /// <summary>
    /// 检查技能是否受到系统错误影响
    /// </summary>
    /// <param name="skillName">技能名称</param>
    /// <returns>是否受影响</returns>
    public bool IsSkillAffected(string skillName)
    {
        // 所有技能都受到系统错误影响
        // 可以在这里添加特定技能的免疫逻辑
        return true;
    }

    /// <summary>
    /// 获取系统错误对技能使用的影响描述
    /// </summary>
    /// <param name="skillName">技能名称</param>
    /// <param name="baseCooldown">基础冷却时间</param>
    /// <returns>影响描述</returns>
    public string GetSkillImpactDescription(string skillName, int baseCooldown)
    {
        if (!IsSkillAffected(skillName)) return "";
        
        int additionalCooldown = Mathf.RoundToInt(intensity * stackCount);
        return $"系统错误：{skillName} 冷却时间增加 {additionalCooldown} 回合";
    }

    /// <summary>
    /// 尝试触发系统错误的额外效果
    /// 有概率导致技能使用失败或产生意外效果
    /// </summary>
    /// <param name="skillName">使用的技能名称</param>
    /// <returns>是否触发额外效果</returns>
    public bool TryTriggerAdditionalEffect(string skillName)
    {
        if (affectedUnit == null) return false;

        // 根据叠加层数和强度计算触发概率
        float triggerChance = (stackCount * intensity * 0.1f); // 每层10%概率
        triggerChance = Mathf.Clamp01(triggerChance); // 限制在0-1之间

        if (Random.Range(0f, 1f) < triggerChance)
        {
            Debug.Log($"{affectedUnit.data.unitName} 系统错误：{skillName} 触发额外效果！");
            
            // 可以在这里添加各种额外效果
            // 例如：技能失效、伤害自己、随机目标等
            TriggerRandomMalfunction(skillName);
            return true;
        }

        return false;
    }

    /// <summary>
    /// 触发随机故障效果
    /// </summary>
    /// <param name="skillName">技能名称</param>
    private void TriggerRandomMalfunction(string skillName)
    {
        int malfunctionType = Random.Range(0, 3);
        
        switch (malfunctionType)
        {
            case 0:
                // 技能失效
                Debug.Log($"{affectedUnit.data.unitName} 系统错误：{skillName} 技能失效！");
                break;
                
            case 1:
                // 对自己造成少量伤害
                int selfDamage = Mathf.RoundToInt(stackCount * intensity);
                affectedUnit.TakeDamage(selfDamage);
                Debug.Log($"{affectedUnit.data.unitName} 系统错误：{skillName} 反噬，造成 {selfDamage} 点伤害！");
                break;
                
            case 2:
                // 增加额外的状态异常持续时间
                remainingTurns += 1;
                Debug.Log($"{affectedUnit.data.unitName} 系统错误：{skillName} 加重系统错误，持续时间增加1回合！");
                break;
        }
    }

    /// <summary>
    /// 获取效果的详细描述
    /// </summary>
    /// <returns>效果描述</returns>
    public override string GetEffectDescription()
    {
        string desc = $"系统错误：技能冷却时间增加 {Mathf.RoundToInt(intensity * stackCount)} 回合";
        if (canStack && stackCount > 1)
        {
            desc += $" (叠加 {stackCount} 层)";
        }
        desc += $" - 剩余 {remainingTurns} 回合";
        
        // 添加额外效果概率信息
        float triggerChance = (stackCount * intensity * 0.1f);
        if (triggerChance > 0)
        {
            desc += $"\n故障概率：{Mathf.RoundToInt(triggerChance * 100)}%";
        }
        
        return desc;
    }

    /// <summary>
    /// 获取系统错误的严重程度
    /// </summary>
    /// <returns>严重程度等级 (1-5)</returns>
    public int GetSeverityLevel()
    {
        float severity = stackCount * intensity;
        
        if (severity >= 5f) return 5; // 严重
        if (severity >= 4f) return 4; // 高
        if (severity >= 3f) return 3; // 中等
        if (severity >= 2f) return 2; // 轻微
        return 1; // 最轻
    }

    /// <summary>
    /// 获取系统错误的严重程度描述
    /// </summary>
    /// <returns>严重程度描述</returns>
    public string GetSeverityDescription()
    {
        int level = GetSeverityLevel();
        
        switch (level)
        {
            case 5: return "系统崩溃";
            case 4: return "严重错误";
            case 3: return "中等错误";
            case 2: return "轻微错误";
            case 1: return "系统警告";
            default: return "未知错误";
        }
    }
}