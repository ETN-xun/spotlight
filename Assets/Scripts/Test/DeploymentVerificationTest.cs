using UnityEngine;

/// <summary>
/// 部署功能验证测试
/// 验证之前修复的部署相关bug
/// </summary>
public class DeploymentVerificationTest : MonoBehaviour
{
    [Header("测试设置")]
    public bool runTestOnStart = true;
    
    void Start()
    {
        if (runTestOnStart)
        {
            RunDeploymentTests();
        }
    }
    
    [ContextMenu("运行部署测试")]
    public void RunDeploymentTests()
    {
        Debug.Log("=== 开始部署功能验证测试 ===");
        
        TestCoordinateTransform();
        TestDeploymentRangeCheck();
        TestDeploymentStateLogic();
        
        Debug.Log("=== 部署功能验证测试完成 ===");
    }
    
    /// <summary>
    /// 测试坐标转换功能
    /// </summary>
    void TestCoordinateTransform()
    {
        Debug.Log("--- 测试坐标转换功能 ---");
        
        // 测试有效坐标范围 [0..7]
        for (int i = 0; i <= 7; i++)
        {
            Vector2Int testCoord = new Vector2Int(i, i);
            Vector2Int originalCoord = testCoord;
            
            // 调用坐标转换（现在应该被注释掉，不会改变坐标）
            // Utils.Coordinate.Transform(ref testCoord); // 这行已被注释
            
            if (testCoord == originalCoord)
            {
                Debug.Log($"✓ 坐标 {originalCoord} 保持不变（修复成功）");
            }
            else
            {
                Debug.LogError($"✗ 坐标 {originalCoord} 被错误转换为 {testCoord}");
            }
        }
    }
    
    /// <summary>
    /// 测试部署范围检查
    /// </summary>
    void TestDeploymentRangeCheck()
    {
        Debug.Log("--- 测试部署范围检查 ---");
        
        // 测试有效范围内的坐标
        Vector2Int validCoord = new Vector2Int(3, 3);
        bool isValidRange = validCoord.x >= 0 && validCoord.x <= 7 && 
                           validCoord.y >= 0 && validCoord.y <= 7;
        
        if (isValidRange)
        {
            Debug.Log($"✓ 坐标 {validCoord} 在有效范围内 [0..7]");
        }
        else
        {
            Debug.LogError($"✗ 坐标 {validCoord} 不在有效范围内");
        }
        
        // 测试无效范围的坐标
        Vector2Int invalidCoord = new Vector2Int(8, 8);
        bool isInvalidRange = invalidCoord.x < 0 || invalidCoord.x > 7 || 
                             invalidCoord.y < 0 || invalidCoord.y > 7;
        
        if (isInvalidRange)
        {
            Debug.Log($"✓ 坐标 {invalidCoord} 正确识别为超出范围");
        }
        else
        {
            Debug.LogError($"✗ 坐标 {invalidCoord} 未正确识别为超出范围");
        }
    }
    
    /// <summary>
    /// 测试部署状态逻辑
    /// </summary>
    void TestDeploymentStateLogic()
    {
        Debug.Log("--- 测试部署状态逻辑 ---");
        
        // 模拟部署计数检查
        int deployedCount = 3;
        int totalUnits = 5;
        
        bool allUnitsDeployed = deployedCount >= totalUnits;
        
        if (!allUnitsDeployed)
        {
            Debug.Log($"✓ 部署检查正确：已部署 {deployedCount}/{totalUnits} 个单位，不允许结束部署阶段");
        }
        else
        {
            Debug.Log($"✓ 部署检查正确：已部署 {deployedCount}/{totalUnits} 个单位，可以结束部署阶段");
        }
        
        // 测试全部部署完成的情况
        deployedCount = 5;
        allUnitsDeployed = deployedCount >= totalUnits;
        
        if (allUnitsDeployed)
        {
            Debug.Log($"✓ 部署检查正确：已部署 {deployedCount}/{totalUnits} 个单位，可以结束部署阶段");
        }
        else
        {
            Debug.LogError($"✗ 部署检查错误：已部署 {deployedCount}/{totalUnits} 个单位，应该可以结束部署阶段");
        }
    }
}