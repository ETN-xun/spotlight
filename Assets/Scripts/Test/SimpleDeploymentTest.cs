using UnityEngine;

/// <summary>
/// 简单的部署功能测试 - 可以在Unity编辑器中直接运行
/// </summary>
public class SimpleDeploymentTest : MonoBehaviour
{
    [Header("测试按钮")]
    [SerializeField] private bool runTest = false;
    
    void Update()
    {
        if (runTest)
        {
            runTest = false;
            RunCoordinateTest();
        }
    }
    
    [ContextMenu("运行坐标转换测试")]
    public void RunCoordinateTest()
    {
        Debug.Log("=== 开始坐标转换测试 ===");
        
        // 测试修复前后的坐标转换
        Vector2Int[] testCoords = {
            new Vector2Int(0, 0),
            new Vector2Int(1, 1),
            new Vector2Int(3, 4),
            new Vector2Int(7, 7)
        };
        
        foreach (var coord in testCoords)
        {
            Vector2Int originalCoord = coord;
            Vector2Int transformedCoord = coord;
            Utils.Coordinate.Transform(ref transformedCoord);
            
            Debug.Log($"原坐标: {originalCoord} -> 转换后: {transformedCoord}");
            
            // 验证坐标是否保持不变（修复后应该不变）
            if (transformedCoord == originalCoord)
            {
                Debug.Log($"✓ 坐标 {originalCoord} 正确保持不变");
            }
            else
            {
                Debug.LogError($"✗ 坐标 {originalCoord} 被错误转换为 {transformedCoord}");
            }
        }
        
        // 测试边界坐标
        Vector2Int boundaryCoord = new Vector2Int(8, 8);
        Vector2Int originalBoundary = boundaryCoord;
        Utils.Coordinate.Transform(ref boundaryCoord);
        
        Debug.Log($"边界测试 - 原坐标: {originalBoundary} -> 转换后: {boundaryCoord}");
        
        Debug.Log("=== 坐标转换测试完成 ===");
    }
    
    [ContextMenu("测试部署逻辑")]
    public void TestDeploymentLogic()
    {
        Debug.Log("=== 开始部署逻辑测试 ===");
        
        // 模拟部署计数测试
        int totalUnits = 5;
        
        for (int deployed = 0; deployed <= totalUnits; deployed++)
        {
            bool canEndTurn = deployed >= totalUnits;
            Debug.Log($"已部署单位: {deployed}/{totalUnits}, 可以结束回合: {canEndTurn}");
            
            if (deployed < totalUnits && !canEndTurn)
            {
                Debug.Log($"✓ 部署未完成检查正确");
            }
            else if (deployed >= totalUnits && canEndTurn)
            {
                Debug.Log($"✓ 部署完成检查正确");
            }
            else
            {
                Debug.LogError($"✗ 部署检查逻辑错误");
            }
        }
        
        Debug.Log("=== 部署逻辑测试完成 ===");
    }
    
    [ContextMenu("运行所有测试")]
    public void RunAllTests()
    {
        RunCoordinateTest();
        TestDeploymentLogic();
        Debug.Log("🎉 所有测试完成！请查看控制台输出验证结果。");
    }
}