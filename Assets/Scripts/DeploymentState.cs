using UnityEngine;

/// <summary>
/// 部署状态 - 玩家放置单位的阶段
/// </summary>
public class DeploymentState : GameStateBase
{
    public DeploymentState(GameManager gameManager) : base(gameManager)
    {
    }
    
    public override void Enter()
    {
        base.Enter();
        
        // 显示部署UI
        ShowDeploymentUI();
        
        // 启用单位放置功能
        EnableUnitPlacement();
        
        Debug.Log("部署阶段开始 - 请放置你的单位");
    }
    
    public override void Update()
    {
        // 检查是否完成部署
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // 空格键确认部署完成
            FinishDeployment();
        }
        
        // 处理单位放置逻辑
        HandleUnitPlacement();
    }
    
    public override void Exit()
    {
        base.Exit();
        
        // 隐藏部署UI
        HideDeploymentUI();
        
        // 禁用单位放置功能
        DisableUnitPlacement();
        
        Debug.Log("部署阶段结束");
    }
    
    /// <summary>
    /// 显示部署UI
    /// </summary>
    private void ShowDeploymentUI()
    {
        // TODO: 在这里显示部署相关的UI
        // 例如：单位选择面板、放置指示器等
        Debug.Log("显示部署UI");
        
        // 示例：可以通过事件通知UI系统
        // UIManager.Instance?.ShowDeploymentPanel();
    }
    
    /// <summary>
    /// 隐藏部署UI
    /// </summary>
    private void HideDeploymentUI()
    {
        // TODO: 在这里隐藏部署相关的UI
        Debug.Log("隐藏部署UI");
        
        // 示例：可以通过事件通知UI系统
        // UIManager.Instance?.HideDeploymentPanel();
    }
    
    /// <summary>
    /// 启用单位放置功能
    /// </summary>
    private void EnableUnitPlacement()
    {
        // TODO: 启用单位放置相关的输入处理
        Debug.Log("启用单位放置功能");
    }
    
    /// <summary>
    /// 禁用单位放置功能
    /// </summary>
    private void DisableUnitPlacement()
    {
        // TODO: 禁用单位放置相关的输入处理
        Debug.Log("禁用单位放置功能");
    }
    
    /// <summary>
    /// 处理单位放置逻辑
    /// </summary>
    private void HandleUnitPlacement()
    {
        // TODO: 处理鼠标点击放置单位的逻辑
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePosition = Input.mousePosition;
            // 将屏幕坐标转换为世界坐标并放置单位
            Debug.Log($"尝试在位置放置单位: {mousePosition}");
        }
    }
    
    /// <summary>
    /// 完成部署
    /// </summary>
    private void FinishDeployment()
    {
        Debug.Log("部署完成，进入战斗阶段");
        gameManager.EndCurrentTurn();
    }
}