using UnityEngine;

/// <summary>
/// 编译测试脚本 - 验证OverloadModeManager是否能正确编译和运行
/// </summary>
public class CompileTest : MonoBehaviour
{
    void Start()
    {
        Debug.Log("=== 过载模式系统编译测试开始 ===");
        
        // 测试OverloadModeManager是否能正确实例化
        var overloadManager = FindObjectOfType<OverloadModeManager>();
        if (overloadManager != null)
        {
            Debug.Log("✓ OverloadModeManager 编译成功！");
            
            // 测试各种方法调用
            try
            {
                string status = overloadManager.GetOverloadModeStatusInfo();
                Debug.Log($"✓ 状态信息获取成功: {status}");
                
                bool canActivate = overloadManager.CanActivateOverloadMode();
                Debug.Log($"✓ 激活检查成功: {(canActivate ? "可以激活" : "无法激活")}");
                
                bool isActive = overloadManager.IsOverloadModeActive;
                Debug.Log($"✓ 状态检查成功: {(isActive ? "已激活" : "未激活")}");
                
                Debug.Log("✓ 所有方法调用成功！");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"✗ 方法调用失败: {e.Message}");
            }
        }
        else
        {
            Debug.Log("⚠ 未找到 OverloadModeManager 实例（这在测试场景中是正常的）");
        }
        
        Debug.Log("=== 过载模式系统编译测试完成 ===");
    }
}