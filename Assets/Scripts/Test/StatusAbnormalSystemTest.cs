using UnityEngine;

public class StatusAbnormalSystemTest : MonoBehaviour
{
    [Header("测试配置")]
    public SkillDataSO dataCorruptionSkill;
    public SkillDataSO systemErrorSkill;
    public SkillDataSO virusInfectionSkill;
    
    [Header("测试单位")]
    public UnitDataSO testUnit1;
    public UnitDataSO testUnit2;
    
    private Unit testUnitInstance1;
    private Unit testUnitInstance2;
    
    void Start()
    {
        Debug.Log("=== 状态异常系统测试开始 ===");
        
        // 创建测试单位
        CreateTestUnits();
        
        // 测试状态异常效果
        TestStatusEffects();
        
        // 测试技能数据配置
        TestSkillConfigurations();
    }
    
    void CreateTestUnits()
    {
        Debug.Log("创建测试单位...");
        
        // 创建测试单位1
        GameObject unit1Obj = new GameObject("TestUnit1");
        testUnitInstance1 = unit1Obj.AddComponent<Unit>();
        testUnitInstance1.data = testUnit1;
        
        // 创建测试单位2
        GameObject unit2Obj = new GameObject("TestUnit2");
        testUnitInstance2 = unit2Obj.AddComponent<Unit>();
        testUnitInstance2.data = testUnit2;
        
        Debug.Log("测试单位创建完成");
    }
    
    void TestStatusEffects()
    {
        Debug.Log("\n=== 测试状态异常效果 ===");
        
        // 测试数据损坏效果
        TestDataCorruptionEffect();
        
        // 测试系统错误效果
        TestSystemErrorEffect();
    }
    
    void TestDataCorruptionEffect()
    {
        Debug.Log("\n--- 测试数据损坏效果 ---");
        
        if (testUnitInstance1.StatusEffectManager == null)
        {
            Debug.LogError("StatusEffectManager 未初始化！");
            return;
        }
        
        // 添加数据损坏状态
        testUnitInstance1.StatusEffectManager.AddStatusEffect(
            StatusAbnormalType.DataCorruption, 3, 1.0f);
        
        // 检查状态是否添加成功
        bool hasEffect = testUnitInstance1.StatusEffectManager.HasStatusEffect(
            StatusAbnormalType.DataCorruption);
        Debug.Log($"数据损坏状态添加成功: {hasEffect}");
        
        // 测试状态描述
        string description = testUnitInstance1.StatusEffectManager.GetStatusEffectDescription(
            StatusAbnormalType.DataCorruption);
        Debug.Log($"状态描述: {description}");
    }
    
    void TestSystemErrorEffect()
    {
        Debug.Log("\n--- 测试系统错误效果 ---");
        
        if (testUnitInstance2.StatusEffectManager == null)
        {
            Debug.LogError("StatusEffectManager 未初始化！");
            return;
        }
        
        // 添加系统错误状态
        testUnitInstance2.StatusEffectManager.AddStatusEffect(
            StatusAbnormalType.SystemError, 2, 1.5f);
        
        // 检查状态是否添加成功
        bool hasEffect = testUnitInstance2.StatusEffectManager.HasStatusEffect(
            StatusAbnormalType.SystemError);
        Debug.Log($"系统错误状态添加成功: {hasEffect}");
        
        // 测试冷却时间修改
        int originalCooldown = 2;
        int modifiedCooldown = testUnitInstance2.StatusEffectManager.GetModifiedCooldown(originalCooldown);
        Debug.Log($"原始冷却时间: {originalCooldown}, 修改后: {modifiedCooldown}");
    }
    
    void TestSkillConfigurations()
    {
        Debug.Log("\n=== 测试技能配置 ===");
        
        // 测试数据损坏技能配置
        if (dataCorruptionSkill != null)
        {
            Debug.Log($"数据损坏技能: {dataCorruptionSkill.skillName}");
            Debug.Log($"技能类型: {dataCorruptionSkill.skillType}");
            Debug.Log($"状态类型: {dataCorruptionSkill.statusAbnormalType}");
            Debug.Log($"持续时间: {dataCorruptionSkill.statusDuration}");
            Debug.Log($"强度: {dataCorruptionSkill.statusIntensity}");
            Debug.Log($"可叠加: {dataCorruptionSkill.statusCanStack}");
        }
        
        // 测试系统错误技能配置
        if (systemErrorSkill != null)
        {
            Debug.Log($"系统错误技能: {systemErrorSkill.skillName}");
            Debug.Log($"技能类型: {systemErrorSkill.skillType}");
            Debug.Log($"状态类型: {systemErrorSkill.statusAbnormalType}");
            Debug.Log($"持续时间: {systemErrorSkill.statusDuration}");
            Debug.Log($"强度: {systemErrorSkill.statusIntensity}");
            Debug.Log($"可叠加: {systemErrorSkill.statusCanStack}");
        }
        
        // 测试病毒感染技能配置
        if (virusInfectionSkill != null)
        {
            Debug.Log($"病毒感染技能: {virusInfectionSkill.skillName}");
            Debug.Log($"技能类型: {virusInfectionSkill.skillType}");
            Debug.Log($"状态类型: {virusInfectionSkill.statusAbnormalType}");
            Debug.Log($"持续时间: {virusInfectionSkill.statusDuration}");
            Debug.Log($"强度: {virusInfectionSkill.statusIntensity}");
            Debug.Log($"可叠加: {virusInfectionSkill.statusCanStack}");
            Debug.Log($"影响范围: {virusInfectionSkill.effectRadius}");
        }
    }
    
    void Update()
    {
        // 按空格键手动触发状态更新测试
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("\n=== 手动触发状态更新 ===");
            
            if (testUnitInstance1 != null && testUnitInstance1.StatusEffectManager != null)
            {
                testUnitInstance1.StatusEffectManager.UpdateEffects();
                Debug.Log("单位1状态更新完成");
            }
            
            if (testUnitInstance2 != null && testUnitInstance2.StatusEffectManager != null)
            {
                testUnitInstance2.StatusEffectManager.UpdateEffects();
                Debug.Log("单位2状态更新完成");
            }
        }
    }
}