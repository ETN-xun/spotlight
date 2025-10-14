using UnityEngine;
using System.Collections;

/// <summary>
/// 最终验证测试脚本
/// 验证Pointer Manipulator Zero单位及其技能配置是否正确
/// </summary>
public class FinalVerificationTest : MonoBehaviour
{
    [Header("测试配置")]
    public UnitDataSO pointerManipulatorZero;
    public SkillDataSO breakpointExecutionSkill;
    public SkillDataSO forcedMigrationSkill;
    
    void Start()
    {
        StartCoroutine(RunVerificationTest());
    }
    
    IEnumerator RunVerificationTest()
    {
        Debug.Log("=== 开始最终验证测试 ===");
        
        // 等待数据管理器初始化
        yield return new WaitForSeconds(1f);
        
        bool allTestsPassed = true;
        
        // 测试1: 验证单位数据加载
        if (TestUnitDataLoading())
        {
            Debug.Log("✓ 单位数据加载测试通过");
        }
        else
        {
            Debug.LogError("✗ 单位数据加载测试失败");
            allTestsPassed = false;
        }
        
        // 测试2: 验证技能数据加载
        if (TestSkillDataLoading())
        {
            Debug.Log("✓ 技能数据加载测试通过");
        }
        else
        {
            Debug.LogError("✗ 技能数据加载测试失败");
            allTestsPassed = false;
        }
        
        // 测试3: 验证单位技能关联
        if (TestUnitSkillAssociation())
        {
            Debug.Log("✓ 单位技能关联测试通过");
        }
        else
        {
            Debug.LogError("✗ 单位技能关联测试失败");
            allTestsPassed = false;
        }
        
        // 输出最终结果
        if (allTestsPassed)
        {
            Debug.Log("🎉 所有验证测试通过！Pointer Manipulator Zero配置正确！");
        }
        else
        {
            Debug.LogError("❌ 部分验证测试失败，请检查配置");
        }
        
        Debug.Log("=== 验证测试完成 ===");
    }
    
    bool TestUnitDataLoading()
    {
        // 从DataManager获取单位数据
        var unitData = DataManager.Instance?.GetUnitData("pointer_manipulator_zero");
        
        if (unitData == null)
        {
            Debug.LogError("无法从DataManager获取Pointer Manipulator Zero数据");
            return false;
        }
        
        Debug.Log($"单位名称: {unitData.unitName}");
        Debug.Log($"单位ID: {unitData.unitID}");
        Debug.Log($"生命值: {unitData.maxHP}");
        Debug.Log($"移动范围: {unitData.moveRange}");
        Debug.Log($"攻击范围: {unitData.attackRange}");
        Debug.Log($"技能数量: {unitData.skills?.Length ?? 0}");
        
        return true;
    }
    
    bool TestSkillDataLoading()
    {
        // 测试断点执行技能
        var breakpointSkill = DataManager.Instance?.GetSkillData("breakpoint_execution_01");
        if (breakpointSkill == null)
        {
            Debug.LogError("无法从DataManager获取断点执行技能数据");
            return false;
        }
        
        Debug.Log($"断点执行技能: {breakpointSkill.skillName}");
        Debug.Log($"技能类型: {breakpointSkill.skillType}");
        Debug.Log($"基础伤害: {breakpointSkill.baseDamage}");
        Debug.Log($"攻击范围: {breakpointSkill.range}");
        
        // 测试强制迁移技能
        var migrationSkill = DataManager.Instance?.GetSkillData("forced_migration_01");
        if (migrationSkill == null)
        {
            Debug.LogError("无法从DataManager获取强制迁移技能数据");
            return false;
        }
        
        Debug.Log($"强制迁移技能: {migrationSkill.skillName}");
        Debug.Log($"技能类型: {migrationSkill.skillType}");
        Debug.Log($"基础伤害: {migrationSkill.baseDamage}");
        Debug.Log($"攻击范围: {migrationSkill.range}");
        
        return true;
    }
    
    bool TestUnitSkillAssociation()
    {
        var unitData = DataManager.Instance?.GetUnitData("pointer_manipulator_zero");
        
        if (unitData?.skills == null || unitData.skills.Length == 0)
        {
            Debug.LogError("单位没有配置技能");
            return false;
        }
        
        Debug.Log($"单位配置了 {unitData.skills.Length} 个技能:");
        
        for (int i = 0; i < unitData.skills.Length; i++)
        {
            var skill = unitData.skills[i];
            if (skill != null)
            {
                Debug.Log($"  技能 {i + 1}: {skill.skillName} (ID: {skill.skillID})");
            }
            else
            {
                Debug.LogError($"  技能 {i + 1}: 数据为空");
                return false;
            }
        }
        
        return true;
    }
}