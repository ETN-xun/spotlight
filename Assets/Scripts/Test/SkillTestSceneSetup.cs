using UnityEngine;
using System.Collections;

/// <summary>
/// 技能测试场景设置脚本
/// 用于在场景中创建测试环境并验证技能功能
/// </summary>
public class SkillTestSceneSetup : MonoBehaviour
{
    [Header("技能数据配置")]
    [SerializeField] private SkillDataSO breakpointExecutionSkill;
    [SerializeField] private SkillDataSO forcedMigrationSkill;
    
    [Header("单位数据配置")]
    [SerializeField] private UnitDataSO pointerManipulatorZero;
    [SerializeField] private UnitDataSO testTargetUnit;
    
    [Header("测试配置")]
    [SerializeField] private bool autoStartTest = true;
    [SerializeField] private float testDelay = 2f;
    
    [Header("场景对象")]
    [SerializeField] private Transform testArea;
    [SerializeField] private GameObject unitPrefab;
    
    private Unit casterUnit;
    private Unit targetUnit;
    
    private void Start()
    {
        if (autoStartTest)
        {
            StartCoroutine(SetupTestEnvironment());
        }
    }
    
    private IEnumerator SetupTestEnvironment()
    {
        Debug.Log("=== 开始设置技能测试环境 ===");
        
        yield return new WaitForSeconds(testDelay);
        
        // 创建测试单位
        CreateTestUnits();
        yield return new WaitForSeconds(1f);
        
        // 验证技能数据
        ValidateSkillData();
        yield return new WaitForSeconds(1f);
        
        // 测试技能功能
        TestSkillFunctionality();
        
        Debug.Log("=== 技能测试环境设置完成 ===");
    }
    
    private void CreateTestUnits()
    {
        Debug.Log("--- 创建测试单位 ---");
        
        // 创建施法者单位
        if (pointerManipulatorZero != null)
        {
            GameObject casterObj = CreateUnitGameObject("Caster_PointerManipulatorZero", Vector3.left * 2f);
            casterUnit = casterObj.GetComponent<Unit>();
            if (casterUnit != null)
            {
                casterUnit.data = pointerManipulatorZero;
                Debug.Log($"✓ 创建施法者单位: {pointerManipulatorZero.unitName}");
            }
        }
        else
        {
            Debug.LogError("✗ Pointer Manipulator Zero单位数据未配置");
        }
        
        // 创建目标单位
        if (testTargetUnit != null)
        {
            GameObject targetObj = CreateUnitGameObject("Target_TestUnit", Vector3.right * 2f);
            targetUnit = targetObj.GetComponent<Unit>();
            if (targetUnit != null)
            {
                targetUnit.data = testTargetUnit;
                Debug.Log($"✓ 创建目标单位: {testTargetUnit.unitName}");
            }
        }
        else
        {
            Debug.LogError("✗ 测试目标单位数据未配置");
        }
    }
    
    private GameObject CreateUnitGameObject(string unitName, Vector3 position)
    {
        GameObject unitObj;
        
        if (unitPrefab != null)
        {
            unitObj = Instantiate(unitPrefab, position, Quaternion.identity);
            unitObj.name = unitName;
        }
        else
        {
            // 如果没有预制体，创建基础GameObject
            unitObj = new GameObject(unitName);
            unitObj.transform.position = position;
            
            // 添加必要的组件
            Unit unitComponent = unitObj.AddComponent<Unit>();
            
            // 添加视觉表示
            GameObject visual = GameObject.CreatePrimitive(PrimitiveType.Cube);
            visual.transform.SetParent(unitObj.transform);
            visual.transform.localPosition = Vector3.zero;
            visual.name = "Visual";
        }
        
        if (testArea != null)
        {
            unitObj.transform.SetParent(testArea);
        }
        
        return unitObj;
    }
    
    private void ValidateSkillData()
    {
        Debug.Log("--- 验证技能数据 ---");
        
        // 验证断点执行技能
        if (breakpointExecutionSkill != null)
        {
            Debug.Log($"✓ 断点执行技能数据: {breakpointExecutionSkill.skillName}");
            Debug.Log($"  - 技能ID: {breakpointExecutionSkill.skillID}");
            Debug.Log($"  - 技能类型: {breakpointExecutionSkill.skillType}");
            Debug.Log($"  - 基础伤害: {breakpointExecutionSkill.baseDamage}");
            Debug.Log($"  - 攻击范围: {breakpointExecutionSkill.range}");
        }
        else
        {
            Debug.LogError("✗ 断点执行技能数据未配置");
        }
        
        // 验证强制迁移技能
        if (forcedMigrationSkill != null)
        {
            Debug.Log($"✓ 强制迁移技能数据: {forcedMigrationSkill.skillName}");
            Debug.Log($"  - 技能ID: {forcedMigrationSkill.skillID}");
            Debug.Log($"  - 技能类型: {forcedMigrationSkill.skillType}");
            Debug.Log($"  - 位移距离: {forcedMigrationSkill.displacementDistance}");
            Debug.Log($"  - 位移方向: {forcedMigrationSkill.displacementDirection}");
        }
        else
        {
            Debug.LogError("✗ 强制迁移技能数据未配置");
        }
        
        // 验证单位配置
        if (pointerManipulatorZero != null)
        {
            Debug.Log($"✓ Pointer Manipulator Zero单位配置:");
            Debug.Log($"  - 单位ID: {pointerManipulatorZero.unitID}");
            Debug.Log($"  - 单位名称: {pointerManipulatorZero.unitName}");
            Debug.Log($"  - 最大生命值: {pointerManipulatorZero.maxHP}");
            
            if (pointerManipulatorZero.skills != null && pointerManipulatorZero.skills.Length > 0)
            {
                Debug.Log($"  - 配置技能数量: {pointerManipulatorZero.skills.Length}");
                for (int i = 0; i < pointerManipulatorZero.skills.Length; i++)
                {
                    if (pointerManipulatorZero.skills[i] != null)
                    {
                        Debug.Log($"    {i + 1}. {pointerManipulatorZero.skills[i].skillName}");
                    }
                }
            }
            else
            {
                Debug.LogWarning("  ! 单位没有配置技能");
            }
        }
    }
    
    private void TestSkillFunctionality()
    {
        Debug.Log("--- 测试技能功能 ---");
        
        if (casterUnit == null || targetUnit == null)
        {
            Debug.LogError("✗ 测试单位未正确创建，无法进行技能测试");
            return;
        }
        
        // 测试断点执行技能
        TestBreakpointExecutionSkill();
        
        // 测试强制迁移技能
        TestForcedMigrationSkill();
    }
    
    private void TestBreakpointExecutionSkill()
    {
        if (breakpointExecutionSkill == null)
        {
            Debug.LogError("✗ 断点执行技能数据为空，跳过测试");
            return;
        }
        
        try
        {
            Debug.Log("测试断点执行技能...");
            
            // 创建技能实例
            BreakpointExecutionSkill skill = new BreakpointExecutionSkill(breakpointExecutionSkill, casterUnit);
            
            // 记录目标初始生命值
            int initialHP = targetUnit.currentHP;
            Debug.Log($"目标初始生命值: {initialHP}");
            
            // 模拟技能使用（注意：这里只是测试技能创建，不实际执行）
            Debug.Log("✓ 断点执行技能实例创建成功");
            Debug.Log("  - 技能可以正常实例化");
            Debug.Log("  - 技能构造函数工作正常");
            
        }
        catch (System.Exception e)
        {
            Debug.LogError($"✗ 断点执行技能测试失败: {e.Message}");
            Debug.LogError($"堆栈跟踪: {e.StackTrace}");
        }
    }
    
    private void TestForcedMigrationSkill()
    {
        if (forcedMigrationSkill == null)
        {
            Debug.LogError("✗ 强制迁移技能数据为空，跳过测试");
            return;
        }
        
        try
        {
            Debug.Log("测试强制迁移技能...");
            
            // 创建技能实例
            ForcedMigrationSkill skill = new ForcedMigrationSkill(forcedMigrationSkill, casterUnit);
            
            // 记录目标初始位置
            Vector3 initialPosition = targetUnit.transform.position;
            Debug.Log($"目标初始位置: {initialPosition}");
            
            // 模拟技能使用（注意：这里只是测试技能创建，不实际执行）
            Debug.Log("✓ 强制迁移技能实例创建成功");
            Debug.Log("  - 技能可以正常实例化");
            Debug.Log("  - 技能构造函数工作正常");
            
        }
        catch (System.Exception e)
        {
            Debug.LogError($"✗ 强制迁移技能测试失败: {e.Message}");
            Debug.LogError($"堆栈跟踪: {e.StackTrace}");
        }
    }
    
    // 手动触发测试的方法
    [ContextMenu("手动运行技能测试")]
    public void RunTestManually()
    {
        StartCoroutine(SetupTestEnvironment());
    }
    
    // 清理测试环境
    [ContextMenu("清理测试环境")]
    public void CleanupTestEnvironment()
    {
        if (casterUnit != null)
        {
            DestroyImmediate(casterUnit.gameObject);
            casterUnit = null;
        }
        
        if (targetUnit != null)
        {
            DestroyImmediate(targetUnit.gameObject);
            targetUnit = null;
        }
        
        Debug.Log("测试环境已清理");
    }
}