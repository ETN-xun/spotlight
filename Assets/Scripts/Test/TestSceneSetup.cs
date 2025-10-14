using UnityEngine;

/// <summary>
/// 测试场景设置脚本 - 自动配置集成测试环境
/// </summary>
public class TestSceneSetup : MonoBehaviour
{
    [Header("自动设置")]
    public bool autoSetupOnStart = true;
    
    void Start()
    {
        if (autoSetupOnStart)
        {
            SetupTestEnvironment();
        }
    }
    
    [ContextMenu("设置测试环境")]
    public void SetupTestEnvironment()
    {
        Debug.Log("开始设置测试环境...");
        
        // 查找或创建集成测试对象
        IntegrationTest integrationTest = FindObjectOfType<IntegrationTest>();
        if (integrationTest == null)
        {
            GameObject testObject = new GameObject("IntegrationTest");
            integrationTest = testObject.AddComponent<IntegrationTest>();
            Debug.Log("创建了IntegrationTest对象");
        }
        
        // 尝试自动加载资源
        LoadTestResources(integrationTest);
        
        Debug.Log("测试环境设置完成");
    }
    
    private void LoadTestResources(IntegrationTest integrationTest)
    {
        // 尝试加载Pointer Manipulator Zero单位数据
        if (integrationTest.pointerManipulatorZero == null)
        {
            UnitDataSO unitData = Resources.Load<UnitDataSO>("Units/PointerManipulatorZero");
            if (unitData == null)
            {
                // 尝试从Assets文件夹加载
                string[] guids = UnityEditor.AssetDatabase.FindAssets("PointerManipulatorZero t:UnitDataSO");
                if (guids.Length > 0)
                {
                    string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[0]);
                    unitData = UnityEditor.AssetDatabase.LoadAssetAtPath<UnitDataSO>(path);
                }
            }
            
            if (unitData != null)
            {
                integrationTest.pointerManipulatorZero = unitData;
                Debug.Log($"成功加载单位数据: {unitData.unitName}");
            }
            else
            {
                Debug.LogWarning("未能找到PointerManipulatorZero单位数据");
            }
        }
        
        // 尝试加载断点执行技能数据
        if (integrationTest.breakpointExecutionSkill == null)
        {
            SkillDataSO skillData = Resources.Load<SkillDataSO>("Skills/BreakpointExecutionSkill");
            if (skillData == null)
            {
                string[] guids = UnityEditor.AssetDatabase.FindAssets("BreakpointExecutionSkill t:SkillDataSO");
                if (guids.Length > 0)
                {
                    string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[0]);
                    skillData = UnityEditor.AssetDatabase.LoadAssetAtPath<SkillDataSO>(path);
                }
            }
            
            if (skillData != null)
            {
                integrationTest.breakpointExecutionSkill = skillData;
                Debug.Log($"成功加载技能数据: {skillData.skillName}");
            }
            else
            {
                Debug.LogWarning("未能找到BreakpointExecutionSkill技能数据");
            }
        }
        
        // 尝试加载强制迁移技能数据
        if (integrationTest.forcedMigrationSkill == null)
        {
            SkillDataSO skillData = Resources.Load<SkillDataSO>("Skills/ForcedMigrationSkill");
            if (skillData == null)
            {
                string[] guids = UnityEditor.AssetDatabase.FindAssets("ForcedMigrationSkill t:SkillDataSO");
                if (guids.Length > 0)
                {
                    string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[0]);
                    skillData = UnityEditor.AssetDatabase.LoadAssetAtPath<SkillDataSO>(path);
                }
            }
            
            if (skillData != null)
            {
                integrationTest.forcedMigrationSkill = skillData;
                Debug.Log($"成功加载技能数据: {skillData.skillName}");
            }
            else
            {
                Debug.LogWarning("未能找到ForcedMigrationSkill技能数据");
            }
        }
    }
}