using UnityEngine;
using Action;

/// <summary>
/// 过载模式测试脚本
/// 用于测试过载模式的各项功能
/// </summary>
public class OverloadModeTest : MonoBehaviour
{
    [Header("测试设置")]
    public KeyCode activateOverloadKey = KeyCode.O;
    public KeyCode showStatusKey = KeyCode.I;
    
    private void Update()
    {
        // 按O键激活过载模式
        if (Input.GetKeyDown(activateOverloadKey))
        {
            TestActivateOverloadMode();
        }
        
        // 按I键显示状态信息
        if (Input.GetKeyDown(showStatusKey))
        {
            ShowOverloadModeStatus();
        }
    }
    
    private void TestActivateOverloadMode()
    {
        var overloadManager = OverloadModeManager.Instance;
        if (overloadManager != null)
        {
            Debug.Log("=== 过载模式激活测试 ===");
            Debug.Log($"当前状态: {overloadManager.GetOverloadModeStatusInfo()}");
            
            if (overloadManager.TryActivateOverloadMode())
            {
                Debug.Log("✓ 过载模式激活成功！");
                Debug.Log($"新状态: {overloadManager.GetOverloadModeStatusInfo()}");
            }
            else
            {
                Debug.Log("✗ 过载模式激活失败");
                Debug.Log($"原因: {overloadManager.GetOverloadModeStatusInfo()}");
            }
        }
        else
        {
            Debug.LogError("未找到OverloadModeManager实例！");
        }
    }
    
    private void ShowOverloadModeStatus()
    {
        var overloadManager = OverloadModeManager.Instance;
        if (overloadManager != null)
        {
            Debug.Log("=== 过载模式状态信息 ===");
            Debug.Log($"是否激活: {overloadManager.IsOverloadModeActive}");
            Debug.Log($"剩余回合: {overloadManager.OverloadRemainingTurns}");
            Debug.Log($"状态描述: {overloadManager.GetOverloadModeStatusInfo()}");
            Debug.Log($"是否可激活: {overloadManager.CanActivateOverloadMode()}");
            
            // 显示能量系统状态
            var energySystem = ActionManager.EnergySystem;
            if (energySystem != null)
            {
                Debug.Log($"当前能量: {energySystem.GetCurrentEnergy()}");
            }
        }
        else
        {
            Debug.LogError("未找到OverloadModeManager实例！");
        }
    }
    
    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 300, 200));
        GUILayout.Label("过载模式测试", GUI.skin.box);
        
        if (GUILayout.Button($"激活过载模式 ({activateOverloadKey})"))
        {
            TestActivateOverloadMode();
        }
        
        if (GUILayout.Button($"显示状态 ({showStatusKey})"))
        {
            ShowOverloadModeStatus();
        }
        
        var overloadManager = OverloadModeManager.Instance;
        if (overloadManager != null)
        {
            GUILayout.Label($"状态: {overloadManager.GetOverloadModeStatusInfo()}");
        }
        
        var energySystem = ActionManager.EnergySystem;
        if (energySystem != null)
        {
            GUILayout.Label($"能量: {energySystem.GetCurrentEnergy()}");
        }
        
        GUILayout.EndArea();
    }
}