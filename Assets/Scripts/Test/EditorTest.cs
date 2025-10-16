using UnityEngine;
using UnityEditor;

public class EditorTest
{
    [MenuItem("Test/Run Simple Test")]
    public static void RunSimpleTest()
    {
        Debug.Log("EditorTest: Menu item executed successfully!");
        
        // 测试控制台输出
        Debug.Log("Testing console output...");
        Debug.LogWarning("This is a warning message");
        Debug.LogError("This is an error message");
    }
}