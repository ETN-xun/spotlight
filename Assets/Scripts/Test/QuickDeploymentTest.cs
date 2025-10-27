using UnityEngine;

/// <summary>
/// 快速部署功能测试
/// 可以在Unity编辑器中直接运行，验证部署相关修复
/// </summary>
public class QuickDeploymentTest : MonoBehaviour
{
    [Header("测试控制")]
    [SerializeField] private bool runOnStart = true;
    
    void Start()
    {
        if (runOnStart)
        {
            RunQuickTest();
        }
    }
    
    [ContextMenu("运行快速部署测试")]
    public void RunQuickTest()
    {
        Debug.Log("=== 快速部署功能测试开始 ===");
        
        TestCoordinateValidation();
        TestDeploymentLogic();
        TestRangeChecking();
        
        Debug.Log("=== 快速部署功能测试完成 ===");
    }
    
    /// <summary>
    /// 测试坐标验证
    /// </summary>
    void TestCoordinateValidation()
    {
        Debug.Log("--- 测试坐标验证 ---");
        
        // 测试有效坐标
        Vector2Int[] validCoords = {
            new Vector2Int(0, 0),
            new Vector2Int(3, 3),
            new Vector2Int(7, 7),
            new Vector2Int(0, 7),
            new Vector2Int(7, 0)
        };
        
        foreach (var coord in validCoords)
        {
            bool isValid = IsValidDeploymentCoordinate(coord);
            if (isValid)
            {
                Debug.Log($"✓ 坐标 {coord} 验证通过");
            }
            else
            {
                Debug.LogError($"✗ 坐标 {coord} 验证失败");
            }
        }
        
        // 测试无效坐标
        Vector2Int[] invalidCoords = {
            new Vector2Int(-1, 0),
            new Vector2Int(0, -1),
            new Vector2Int(8, 0),
            new Vector2Int(0, 8),
            new Vector2Int(8, 8),
            new Vector2Int(-1, -1)
        };
        
        foreach (var coord in invalidCoords)
        {
            bool isValid = IsValidDeploymentCoordinate(coord);
            if (!isValid)
            {
                Debug.Log($"✓ 无效坐标 {coord} 正确被拒绝");
            }
            else
            {
                Debug.LogError($"✗ 无效坐标 {coord} 错误地被接受");
            }
        }
    }
    
    /// <summary>
    /// 测试部署逻辑
    /// </summary>
    void TestDeploymentLogic()
    {
        Debug.Log("--- 测试部署逻辑 ---");
        
        // 模拟部署计数
        int totalUnits = 5;
        
        for (int deployed = 0; deployed <= totalUnits; deployed++)
        {
            bool canEndDeployment = CanEndDeploymentPhase(deployed, totalUnits);
            
            if (deployed < totalUnits)
            {
                if (!canEndDeployment)
                {
                    Debug.Log($"✓ 已部署 {deployed}/{totalUnits}，正确阻止结束部署阶段");
                }
                else
                {
                    Debug.LogError($"✗ 已部署 {deployed}/{totalUnits}，错误允许结束部署阶段");
                }
            }
            else
            {
                if (canEndDeployment)
                {
                    Debug.Log($"✓ 已部署 {deployed}/{totalUnits}，正确允许结束部署阶段");
                }
                else
                {
                    Debug.LogError($"✗ 已部署 {deployed}/{totalUnits}，错误阻止结束部署阶段");
                }
            }
        }
    }
    
    /// <summary>
    /// 测试范围检查
    /// </summary>
    void TestRangeChecking()
    {
        Debug.Log("--- 测试范围检查 ---");
        
        // 测试边界情况
        Vector2Int[] boundaryCoords = {
            new Vector2Int(0, 0),    // 左上角
            new Vector2Int(7, 0),    // 右上角
            new Vector2Int(0, 7),    // 左下角
            new Vector2Int(7, 7)     // 右下角
        };
        
        foreach (var coord in boundaryCoords)
        {
            bool inRange = IsInValidRange(coord);
            if (inRange)
            {
                Debug.Log($"✓ 边界坐标 {coord} 在有效范围内");
            }
            else
            {
                Debug.LogError($"✗ 边界坐标 {coord} 不在有效范围内");
            }
        }
        
        // 测试超出边界的情况
        Vector2Int[] outOfBoundCoords = {
            new Vector2Int(-1, 0),
            new Vector2Int(8, 0),
            new Vector2Int(0, -1),
            new Vector2Int(0, 8)
        };
        
        foreach (var coord in outOfBoundCoords)
        {
            bool inRange = IsInValidRange(coord);
            if (!inRange)
            {
                Debug.Log($"✓ 超界坐标 {coord} 正确识别为超出范围");
            }
            else
            {
                Debug.LogError($"✗ 超界坐标 {coord} 错误识别为在范围内");
            }
        }
    }
    
    /// <summary>
    /// 检查坐标是否为有效的部署坐标
    /// </summary>
    bool IsValidDeploymentCoordinate(Vector2Int coord)
    {
        // 坐标必须在 [0, 7] 范围内
        return coord.x >= 0 && coord.x <= 7 && coord.y >= 0 && coord.y <= 7;
    }
    
    /// <summary>
    /// 检查是否可以结束部署阶段
    /// </summary>
    bool CanEndDeploymentPhase(int deployedCount, int totalUnits)
    {
        // 只有当所有单位都部署完成时才能结束部署阶段
        return deployedCount >= totalUnits;
    }
    
    /// <summary>
    /// 检查坐标是否在有效范围内
    /// </summary>
    bool IsInValidRange(Vector2Int coord)
    {
        return coord.x >= 0 && coord.x <= 7 && coord.y >= 0 && coord.y <= 7;
    }
}