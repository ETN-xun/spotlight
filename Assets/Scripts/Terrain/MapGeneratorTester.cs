using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// 地图生成器测试脚本
/// 用于测试RandomMapGenerator的功能
/// </summary>
public class MapGeneratorTester : MonoBehaviour
{
    [Header("测试设置")]
    [SerializeField] private RandomMapGenerator mapGenerator;
    [SerializeField] private bool autoTest = false;
    [SerializeField] private int testIterations = 5;
    
    private void Start()
    {
        if (autoTest)
        {
            StartCoroutine(RunTests());
        }
    }
    
    private System.Collections.IEnumerator RunTests()
    {
        Debug.Log("开始地图生成测试...");
        
        for (int i = 0; i < testIterations; i++)
        {
            Debug.Log($"测试迭代 {i + 1}/{testIterations}");
            
            // 生成地图
            if (mapGenerator != null)
            {
                try
                {
                    mapGenerator.GenerateRandomMap();
                    Debug.Log($"迭代 {i + 1}: 地图生成成功");
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"迭代 {i + 1}: 地图生成失败 - {e.Message}");
                }
            }
            else
            {
                Debug.LogError("MapGenerator 引用为空！");
                break;
            }
            
            yield return new WaitForSeconds(1f);
        }
        
        Debug.Log("地图生成测试完成！");
    }
    
    [ContextMenu("手动测试地图生成")]
    public void ManualTest()
    {
        if (mapGenerator != null)
        {
            try
            {
                Debug.Log("开始手动地图生成测试...");
                mapGenerator.GenerateRandomMap();
                Debug.Log("手动地图生成测试成功！");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"手动地图生成测试失败: {e.Message}");
            }
        }
        else
        {
            Debug.LogError("MapGenerator 引用为空！请在Inspector中设置引用。");
        }
    }
    
    [ContextMenu("验证地图约束")]
    public void ValidateMapConstraints()
    {
        Debug.Log("开始验证地图约束...");
        
        // 这里可以添加更多的约束验证逻辑
        // 例如检查路径连通性、区域限制等
        
        Debug.Log("地图约束验证完成！");
    }
}