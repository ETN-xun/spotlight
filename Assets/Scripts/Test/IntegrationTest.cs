using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 集成测试脚本 - 验证所有组件是否正常工作
/// </summary>
public class IntegrationTest : MonoBehaviour
{
    [Header("测试配置")]
    public UnitDataSO pointerManipulatorZero;
    public SkillDataSO breakpointExecutionSkill;
    public SkillDataSO forcedMigrationSkill;
    
    [Header("测试结果")]
    public bool allTestsPassed = false;
    
    private List<string> testResults = new List<string>();
    
    void Start()
    {
        Debug.Log("=== 开始集成测试 ===");
        RunAllTests();
        DisplayResults();
    }
    
    private void RunAllTests()
    {
        testResults.Clear();
        
        // 测试1: 验证单位数据加载
        TestUnitDataLoading();
        
        // 测试2: 验证技能数据加载
        TestSkillDataLoading();
        
        // 测试3: 验证技能实例化
        TestSkillInstantiation();
        
        // 测试4: 验证单位技能关联
        TestUnitSkillAssociation();
        
        // 检查所有测试是否通过
        allTestsPassed = !testResults.Exists(result => result.StartsWith("✗"));
    }
    
    private void TestUnitDataLoading()
    {
        if (pointerManipulatorZero != null)
        {
            testResults.Add($"✓ 单位数据加载成功: {pointerManipulatorZero.unitName}");
            testResults.Add($"  - 单位ID: {pointerManipulatorZero.unitID}");
            testResults.Add($"  - 最大生命值: {pointerManipulatorZero.maxHP}");
            testResults.Add($"  - 移动范围: {pointerManipulatorZero.moveRange}");
            testResults.Add($"  - 基础伤害: {pointerManipulatorZero.baseDamage}");
            testResults.Add($"  - 技能数量: {pointerManipulatorZero.skills.Length}");
        }
        else
        {
            testResults.Add("✗ 单位数据加载失败: pointerManipulatorZero为空");
        }
    }
    
    private void TestSkillDataLoading()
    {
        // 测试断点执行技能
        if (breakpointExecutionSkill != null)
        {
            testResults.Add($"✓ 断点执行技能数据加载成功: {breakpointExecutionSkill.skillName}");
            testResults.Add($"  - 技能ID: {breakpointExecutionSkill.skillID}");
            testResults.Add($"  - 技能类型: {breakpointExecutionSkill.skillType}");
            testResults.Add($"  - 基础伤害: {breakpointExecutionSkill.baseDamage}");
        }
        else
        {
            testResults.Add("✗ 断点执行技能数据加载失败");
        }
        
        // 测试强制迁移技能
        if (forcedMigrationSkill != null)
        {
            testResults.Add($"✓ 强制迁移技能数据加载成功: {forcedMigrationSkill.skillName}");
            testResults.Add($"  - 技能ID: {forcedMigrationSkill.skillID}");
            testResults.Add($"  - 技能类型: {forcedMigrationSkill.skillType}");
            testResults.Add($"  - 位移距离: {forcedMigrationSkill.displacementDistance}");
        }
        else
        {
            testResults.Add("✗ 强制迁移技能数据加载失败");
        }
    }
    
    private void TestSkillInstantiation()
    {
        if (pointerManipulatorZero == null)
        {
            testResults.Add("✗ 技能实例化测试跳过: 单位数据为空");
            return;
        }
        
        try
        {
            // 创建测试单位
            GameObject testUnit = new GameObject("TestUnit");
            Unit unit = testUnit.AddComponent<Unit>();
            unit.data = pointerManipulatorZero;
            
            // 测试断点执行技能实例化
            if (breakpointExecutionSkill != null)
            {
                BreakpointExecutionSkill skill1 = new BreakpointExecutionSkill(breakpointExecutionSkill, unit);
                testResults.Add("✓ 断点执行技能实例化成功");
            }
            
            // 测试强制迁移技能实例化
            if (forcedMigrationSkill != null)
            {
                ForcedMigrationSkill skill2 = new ForcedMigrationSkill(forcedMigrationSkill, unit);
                testResults.Add("✓ 强制迁移技能实例化成功");
            }
            
            // 清理测试对象
            DestroyImmediate(testUnit);
        }
        catch (System.Exception e)
        {
            testResults.Add($"✗ 技能实例化失败: {e.Message}");
        }
    }
    
    private void TestUnitSkillAssociation()
    {
        if (pointerManipulatorZero == null || pointerManipulatorZero.skills == null)
        {
            testResults.Add("✗ 单位技能关联测试跳过: 数据为空");
            return;
        }
        
        testResults.Add($"✓ 单位技能关联验证: 共{pointerManipulatorZero.skills.Length}个技能");
        
        for (int i = 0; i < pointerManipulatorZero.skills.Length; i++)
        {
            var skill = pointerManipulatorZero.skills[i];
            if (skill != null)
            {
                testResults.Add($"  - 技能{i + 1}: {skill.skillName} (ID: {skill.skillID})");
            }
            else
            {
                testResults.Add($"  - 技能{i + 1}: 数据为空");
            }
        }
    }
    
    private void DisplayResults()
    {
        Debug.Log("=== 集成测试结果 ===");
        
        foreach (string result in testResults)
        {
            Debug.Log(result);
        }
        
        if (allTestsPassed)
        {
            Debug.Log("🎉 所有测试通过！系统集成成功！");
        }
        else
        {
            Debug.LogError("❌ 部分测试失败，请检查上述错误信息");
        }
        
        Debug.Log("=== 测试完成 ===");
    }
    
    // 在Inspector中显示测试结果
    void OnValidate()
    {
        if (Application.isPlaying && testResults != null && testResults.Count > 0)
        {
            // 可以在这里添加Inspector显示逻辑
        }
    }
}