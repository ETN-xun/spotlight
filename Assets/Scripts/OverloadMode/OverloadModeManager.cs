using System.Collections.Generic;
using UnityEngine;
using Ally;
using Enemy;
using Common;
using Action;

/// <summary>
/// 过载模式管理器
/// 负责管理过载模式的激活、能量消耗和效果应用
/// </summary>
public class OverloadModeManager : MonoBehaviour
{
    #region 单例模式
    private static OverloadModeManager _instance;
    public static OverloadModeManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<OverloadModeManager>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("OverloadModeManager");
                    _instance = go.AddComponent<OverloadModeManager>();
                }
            }
            return _instance;
        }
    }
    #endregion

    [Header("过载模式配置")]
    [Tooltip("激活过载模式所需的能量")]
    public int overloadEnergyCost = 5;
    
    [Tooltip("过载状态持续回合数")]
    public int overloadDuration = 3;

    [Header("过载效果配置")]
    [Tooltip("代码刺客过载效果：攻击力增加")]
    public int codeAssassinDamageBonus = 2;
    
    [Tooltip("协议卫士过载效果：移动范围增加")]
    public int protocolGuardianMoveBonus = 1;
    
    [Tooltip("指针操控者过载效果：技能冷却减少")]
    public int pointerManipulatorCooldownReduction = 1;

    /// <summary>
    /// 当前是否处于过载模式
    /// </summary>
    public bool IsOverloadModeActive { get; private set; } = false;

    /// <summary>
    /// 过载模式剩余回合数
    /// </summary>
    public int OverloadRemainingTurns { get; private set; } = 0;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
    }

    private void OnEnable()
    {
        MessageCenter.Subscribe(Defines.PlayerTurnEndEvent, OnPlayerTurnEnd);
    }

    private void OnDisable()
    {
        MessageCenter.Unsubscribe(Defines.PlayerTurnEndEvent, OnPlayerTurnEnd);
    }

    /// <summary>
    /// 尝试激活过载模式
    /// </summary>
    /// <returns>是否成功激活</returns>
    public bool TryActivateOverloadMode()
    {
        // 检查能量是否足够
        var energySystem = ActionManager.EnergySystem;
        if (energySystem == null)
        {
            Debug.LogError("未找到能量系统！");
            return false;
        }

        if (!energySystem.TrySpendEnergy(overloadEnergyCost))
        {
            Debug.Log($"能量不足！需要 {overloadEnergyCost} 点能量，当前只有 {energySystem.GetCurrentEnergy()} 点");
            return false;
        }

        // 激活过载模式
        ActivateOverloadMode();
        return true;
    }

    /// <summary>
    /// 激活过载模式
    /// </summary>
    private void ActivateOverloadMode()
    {
        Debug.Log("激活过载模式！");
        
        IsOverloadModeActive = true;
        OverloadRemainingTurns = overloadDuration;

        // 清除所有单位的debuff
        ClearAllUnitsDebuffs();

        // 为所有友方单位应用过载效果
        ApplyOverloadEffectsToAllies();

        // 发布过载模式激活事件
        MessageCenter.Publish(Defines.OverloadModeActivatedEvent);
    }

    /// <summary>
    /// 清除所有单位的debuff状态
    /// </summary>
    private void ClearAllUnitsDebuffs()
    {
        Debug.Log("清除所有单位的debuff状态...");

        // 清除友方单位的debuff
        if (AllyManager.Instance != null)
        {
            List<Unit> allies = AllyManager.Instance.GetAliveAllies();
            foreach (Unit ally in allies)
            {
                if (ally.StatusEffectManager != null)
                {
                    ally.StatusEffectManager.ClearAllEffects();
                    Debug.Log($"清除 {ally.data.unitName} 的所有状态效果");
                }
            }
        }

        // 清除敌方单位的debuff（如果需要的话）
        if (EnemyManager.Instance != null)
        {
            List<Unit> enemies = EnemyManager.Instance.GetAliveEnemies();
            foreach (Unit enemy in enemies)
            {
                if (enemy.StatusEffectManager != null)
                {
                    enemy.StatusEffectManager.ClearAllEffects();
                    Debug.Log($"清除 {enemy.data.unitName} 的所有状态效果");
                }
            }
        }
    }

    /// <summary>
    /// 为所有友方单位应用过载效果
    /// </summary>
    private void ApplyOverloadEffectsToAllies()
    {
        Debug.Log("为友方单位应用过载效果...");

        if (AllyManager.Instance == null) return;

        List<Unit> allies = AllyManager.Instance.GetAliveAllies();
        foreach (Unit ally in allies)
        {
            ApplyOverloadEffectToUnit(ally);
        }
    }

    /// <summary>
    /// 为指定单位应用过载效果
    /// </summary>
    /// <param name="unit">目标单位</param>
    private void ApplyOverloadEffectToUnit(Unit unit)
    {
        if (unit == null || unit.data == null) return;

        string unitName = unit.data.unitName;
        Debug.Log($"为 {unitName} 应用过载效果");

        // 根据单位类型应用不同的过载效果
        switch (unitName)
        {
            case "代码刺客":
            case "Code Assassin":
                ApplyCodeAssassinOverload(unit);
                break;
                
            case "协议卫士":
            case "Protocol Guardian":
                ApplyProtocolGuardianOverload(unit);
                break;
                
            case "指针操控者":
            case "Pointer Manipulator":
                ApplyPointerManipulatorOverload(unit);
                break;
                
            default:
                // 为其他单位应用通用过载效果
                ApplyGenericOverload(unit);
                break;
        }
    }

    /// <summary>
    /// 应用代码刺客过载效果：攻击力增加
    /// </summary>
    private void ApplyCodeAssassinOverload(Unit unit)
    {
        // 添加攻击力增加的状态效果
        if (unit.StatusEffectManager != null)
        {
            Debug.Log($"{unit.data.unitName} 进入过载状态：攻击力增加 {codeAssassinDamageBonus} 点，持续 {overloadDuration} 回合");
            unit.StatusEffectManager.AddStatusEffect(StatusAbnormalType.OverloadDamageBonus, overloadDuration, codeAssassinDamageBonus);
        }
    }

    /// <summary>
    /// 应用协议卫士过载效果：移动范围增加
    /// </summary>
    private void ApplyProtocolGuardianOverload(Unit unit)
    {
        Debug.Log($"{unit.data.unitName} 进入过载状态：移动范围增加 {protocolGuardianMoveBonus} 点，持续 {overloadDuration} 回合");
        unit.StatusEffectManager.AddStatusEffect(StatusAbnormalType.OverloadMoveBonus, overloadDuration, protocolGuardianMoveBonus);
    }

    /// <summary>
    /// 应用指针操控者过载效果：技能冷却减少
    /// </summary>
    private void ApplyPointerManipulatorOverload(Unit unit)
    {
        Debug.Log($"{unit.data.unitName} 进入过载状态：技能冷却减少 {pointerManipulatorCooldownReduction} 回合，持续 {overloadDuration} 回合");
        unit.StatusEffectManager.AddStatusEffect(StatusAbnormalType.OverloadCooldownReduction, overloadDuration, pointerManipulatorCooldownReduction);
    }

    /// <summary>
    /// 应用通用过载效果
    /// </summary>
    private void ApplyGenericOverload(Unit unit)
    {
        Debug.Log($"{unit.data.unitName} 进入过载状态：获得通用过载效果，持续 {overloadDuration} 回合");
        unit.StatusEffectManager.AddStatusEffect(StatusAbnormalType.OverloadGeneric, overloadDuration, 1.0f);
    }

    /// <summary>
    /// 玩家回合结束时调用，更新过载模式状态
    /// </summary>
    private void OnPlayerTurnEnd(object[] args)
    {
        if (IsOverloadModeActive)
        {
            OverloadRemainingTurns--;
            Debug.Log($"过载模式剩余回合数: {OverloadRemainingTurns}");

            if (OverloadRemainingTurns <= 0)
            {
                DeactivateOverloadMode();
            }
        }
    }

    /// <summary>
    /// 停用过载模式
    /// </summary>
    private void DeactivateOverloadMode()
    {
        Debug.Log("过载模式结束");
        
        IsOverloadModeActive = false;
        OverloadRemainingTurns = 0;

        // 移除所有过载效果
        RemoveOverloadEffectsFromAllies();

        // 发布过载模式结束事件
        MessageCenter.Publish(Defines.OverloadModeDeactivatedEvent);
    }

    /// <summary>
    /// 移除所有友方单位的过载效果
    /// </summary>
    private void RemoveOverloadEffectsFromAllies()
    {
        Debug.Log("移除所有过载效果...");

        if (AllyManager.Instance == null) return;

        List<Unit> allies = AllyManager.Instance.GetAliveAllies();
        foreach (Unit ally in allies)
        {
            if (ally.StatusEffectManager != null)
            {
                ally.StatusEffectManager.RemoveStatusEffect(StatusAbnormalType.OverloadDamageBonus);
                ally.StatusEffectManager.RemoveStatusEffect(StatusAbnormalType.OverloadMoveBonus);
                ally.StatusEffectManager.RemoveStatusEffect(StatusAbnormalType.OverloadCooldownReduction);
                ally.StatusEffectManager.RemoveStatusEffect(StatusAbnormalType.OverloadGeneric);
                
                Debug.Log($"移除 {ally.data.unitName} 的过载效果");
            }
        }
    }

    /// <summary>
    /// 检查是否可以激活过载模式
    /// </summary>
    /// <returns>是否可以激活</returns>
    public bool CanActivateOverloadMode()
    {
        if (IsOverloadModeActive)
        {
            return false; // 已经处于过载模式
        }

        var energySystem = ActionManager.EnergySystem;
        if (energySystem == null)
        {
            return false;
        }

        return energySystem.GetCurrentEnergy() >= overloadEnergyCost;
    }

    /// <summary>
    /// 获取过载模式状态信息
    /// </summary>
    /// <returns>状态信息字符串</returns>
    public string GetOverloadModeStatusInfo()
    {
        if (IsOverloadModeActive)
        {
            return $"过载模式激活中 (剩余 {OverloadRemainingTurns} 回合)";
        }
        else
        {
            var energySystem = ActionManager.EnergySystem;
            if (energySystem != null)
            {
                return $"过载模式可用 (需要 {overloadEnergyCost} 能量，当前 {energySystem.GetCurrentEnergy()})";
            }
            return "过载模式可用";
        }
    }
}