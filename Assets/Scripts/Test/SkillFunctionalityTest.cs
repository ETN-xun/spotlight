using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 技能功能测试脚本
/// 用于测试新创建的技能是否正常工作
/// </summary>
public class SkillFunctionalityTest : MonoBehaviour
{
    [Header("测试配置")]
    [SerializeField] private bool runTestsOnStart = true;
    [SerializeField] private bool enableDebugLogs = true;
    
    [Header("技能数据")]
    [SerializeField] private SkillDataSO breakpointExecutionSkill;
    [SerializeField] private SkillDataSO forcedMigrationSkill;
    
    [Header("单位数据")]
    [SerializeField] private UnitDataSO pointerManipulatorZero;
    [SerializeField] private UnitDataSO testTargetUnit;

    private void Start()
    {
        if (runTestsOnStart)
        {
            StartCoroutine(RunSkillTests());
        }
    }

    private System.Collections.IEnumerator RunSkillTests()
    {
        yield return new WaitForSeconds(1f); // 等待场景初始化完成
        
        LogMessage("=== 开始技能功能测试 ===");
        
        // 测试1: 验证技能数据加载
        TestSkillDataLoading();
        yield return new WaitForSeconds(0.5f);
        
        // 测试2: 验证单位配置
        TestUnitConfiguration();
        yield return new WaitForSeconds(0.5f);
        
        // 测试3: 测试断点执行技能
        TestBreakpointExecutionSkill();
        yield return new WaitForSeconds(0.5f);
        
        // 测试4: 测试强制迁移技能
        TestForcedMigrationSkill();
        yield return new WaitForSeconds(0.5f);
        
        LogMessage("=== 技能功能测试完成 ===");
    }

    private void TestSkillDataLoading()
    {
        LogMessage("--- 测试技能数据加载 ---");
        
        // 测试断点执行技能数据
        if (breakpointExecutionSkill != null)
        {
            LogMessage($"✓ 断点执行技能数据加载成功: {breakpointExecutionSkill.skillName}");
            LogMessage($"  - 技能类型: {breakpointExecutionSkill.skillType}");
            LogMessage($"  - 基础伤害: {breakpointExecutionSkill.baseDamage}");
            LogMessage($"  - 攻击范围: {breakpointExecutionSkill.range}");
        }
        else
        {
            LogError("✗ 断点执行技能数据未加载");
        }
        
        // 测试强制迁移技能数据
        if (forcedMigrationSkill != null)
        {
            LogMessage($"✓ 强制迁移技能数据加载成功: {forcedMigrationSkill.skillName}");
            LogMessage($"  - 技能类型: {forcedMigrationSkill.skillType}");
            LogMessage($"  - 位移距离: {forcedMigrationSkill.displacementDistance}");
            LogMessage($"  - 位移方向: {forcedMigrationSkill.displacementDirection}");
        }
        else
        {
            LogError("✗ 强制迁移技能数据未加载");
        }
    }

    private void TestUnitConfiguration()
    {
        LogMessage("--- 测试单位配置 ---");
        
        if (pointerManipulatorZero != null)
        {
            LogMessage($"✓ Pointer Manipulator Zero单位配置加载成功");
            LogMessage($"  - 单位ID: {pointerManipulatorZero.unitID}");
            LogMessage($"  - 单位名称: {pointerManipulatorZero.unitName}");
            LogMessage($"  - 最大生命值: {pointerManipulatorZero.maxHP}");
            LogMessage($"  - 移动范围: {pointerManipulatorZero.moveRange}");
            
            if (pointerManipulatorZero.skills != null && pointerManipulatorZero.skills.Length > 0)
            {
                LogMessage($"  - 技能数量: {pointerManipulatorZero.skills.Length}");
                for (int i = 0; i < pointerManipulatorZero.skills.Length; i++)
                {
                    if (pointerManipulatorZero.skills[i] != null)
                    {
                        LogMessage($"    {i + 1}. {pointerManipulatorZero.skills[i].skillName}");
                    }
                }
            }
            else
            {
                LogError("  ✗ 单位没有配置技能");
            }
        }
        else
        {
            LogError("✗ Pointer Manipulator Zero单位配置未加载");
        }
    }

    private void TestBreakpointExecutionSkill()
    {
        LogMessage("--- 测试断点执行技能 ---");
        
        if (breakpointExecutionSkill == null)
        {
            LogError("✗ 断点执行技能数据为空，无法测试");
            return;
        }
        
        try
        {
            // 创建模拟的施法者和目标
            GameObject casterObj = new GameObject("TestCaster");
            Unit caster = casterObj.AddComponent<Unit>();
            caster.data = pointerManipulatorZero;
            
            // 创建技能实例
            BreakpointExecutionSkill skill = new BreakpointExecutionSkill(breakpointExecutionSkill, caster);
            
            LogMessage("✓ 断点执行技能实例创建成功");
            LogMessage("  - 技能可以正常实例化");
            LogMessage("  - 技能构造函数工作正常");
            
            // 清理测试对象
            DestroyImmediate(casterObj);
        }
        catch (System.Exception e)
        {
            LogError($"✗ 断点执行技能测试失败: {e.Message}");
        }
    }

    private void TestForcedMigrationSkill()
    {
        LogMessage("--- 测试强制迁移技能 ---");
        
        if (forcedMigrationSkill == null)
        {
            LogError("✗ 强制迁移技能数据为空，无法测试");
            return;
        }
        
        try
        {
            // 创建模拟的施法者
            GameObject casterObj = new GameObject("TestCaster");
            Unit caster = casterObj.AddComponent<Unit>();
            caster.data = pointerManipulatorZero;
            
            // 创建技能实例
            ForcedMigrationSkill skill = new ForcedMigrationSkill(forcedMigrationSkill, caster);
            
            LogMessage("✓ 强制迁移技能实例创建成功");
            LogMessage("  - 技能可以正常实例化");
            LogMessage("  - 技能构造函数工作正常");
            
            // 清理测试对象
            DestroyImmediate(casterObj);
        }
        catch (System.Exception e)
        {
            LogError($"✗ 强制迁移技能测试失败: {e.Message}");
        }
    }

    private void LogMessage(string message)
    {
        if (enableDebugLogs)
        {
            Debug.Log($"[SkillTest] {message}");
        }
    }

    private void LogError(string message)
    {
        Debug.LogError($"[SkillTest] {message}");
    }

    // 在Inspector中手动触发测试的按钮
    [ContextMenu("运行技能测试")]
    public void RunTestsManually()
    {
        StartCoroutine(RunSkillTests());
    }
}