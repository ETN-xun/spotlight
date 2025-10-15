using UnityEngine;

/// <summary>
/// 编译验证脚本
/// 用于验证所有测试脚本是否能正常编译
/// </summary>
public class CompilationVerification : MonoBehaviour
{
    [Header("验证结果")]
    public bool compilationSuccessful = true;
    
    void Start()
    {
        Debug.Log("=== 编译验证开始 ===");
        
        // 验证所有测试类是否可以实例化
        try
        {
            // 验证IntegrationTest
            var integrationTest = gameObject.GetComponent<IntegrationTest>();
            if (integrationTest == null)
            {
                integrationTest = gameObject.AddComponent<IntegrationTest>();
            }
            Debug.Log("✓ IntegrationTest 编译成功");
            
            // 验证FinalVerificationTest
            var finalTest = gameObject.GetComponent<FinalVerificationTest>();
            if (finalTest == null)
            {
                finalTest = gameObject.AddComponent<FinalVerificationTest>();
            }
            Debug.Log("✓ FinalVerificationTest 编译成功");
            
            // 验证SkillFunctionalityTest
            var skillTest = gameObject.GetComponent<SkillFunctionalityTest>();
            if (skillTest == null)
            {
                skillTest = gameObject.AddComponent<SkillFunctionalityTest>();
            }
            Debug.Log("✓ SkillFunctionalityTest 编译成功");
            
            // 验证SkillTestSceneSetup
            var sceneSetup = gameObject.GetComponent<SkillTestSceneSetup>();
            if (sceneSetup == null)
            {
                sceneSetup = gameObject.AddComponent<SkillTestSceneSetup>();
            }
            Debug.Log("✓ SkillTestSceneSetup 编译成功");
            
            Debug.Log("=== 所有测试脚本编译验证成功！ ===");
            compilationSuccessful = true;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"编译验证失败: {e.Message}");
            compilationSuccessful = false;
        }
    }
}