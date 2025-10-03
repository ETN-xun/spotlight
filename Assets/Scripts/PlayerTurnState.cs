using UnityEngine;

/// <summary>
/// 玩家回合状态 - 玩家控制单位行动的阶段
/// </summary>
public class PlayerTurnState : GameStateBase
{
    public PlayerTurnState(GameManager gameManager) : base(gameManager)
    {
    }
    
    public override void Enter()
    {
        base.Enter();
        
        // 显示玩家回合UI
        ShowPlayerTurnUI();
        
        // 启用玩家输入
        EnablePlayerInput();
        
        // 重置玩家行动点数
        ResetPlayerActionPoints();
        
        Debug.Log($"玩家回合开始 - 第{gameManager.CurrentTurn}回合");
    }
    
    public override void Update()
    {
        // 处理玩家输入
        HandlePlayerInput();
        
        // 检查是否结束回合
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            // 回车键结束回合
            FinishPlayerTurn();
        }
    }
    
    public override void Exit()
    {
        base.Exit();
        
        // 隐藏玩家回合UI
        HidePlayerTurnUI();
        
        // 禁用玩家输入
        DisablePlayerInput();
        
        Debug.Log("玩家回合结束");
    }
    
    /// <summary>
    /// 显示玩家回合UI
    /// </summary>
    private void ShowPlayerTurnUI()
    {
        // TODO: 显示玩家回合相关的UI
        // 例如：行动点数、技能面板、单位信息等
        Debug.Log("显示玩家回合UI");
    }
    
    /// <summary>
    /// 隐藏玩家回合UI
    /// </summary>
    private void HidePlayerTurnUI()
    {
        // TODO: 隐藏玩家回合相关的UI
        Debug.Log("隐藏玩家回合UI");
    }
    
    /// <summary>
    /// 启用玩家输入
    /// </summary>
    private void EnablePlayerInput()
    {
        // TODO: 启用玩家输入处理
        Debug.Log("启用玩家输入");
    }
    
    /// <summary>
    /// 禁用玩家输入
    /// </summary>
    /// <summary>
    /// 禁用玩家输入
    /// </summary>
    private void DisablePlayerInput()
    {
        // TODO: 禁用玩家输入处理
        Debug.Log("禁用玩家输入");
    }
    
    /// <summary>
    /// 重置玩家行动点数
    /// </summary>
    private void ResetPlayerActionPoints()
    {
        // TODO: 重置所有玩家单位的行动点数
        Debug.Log("重置玩家行动点数");
    }
    
    /// <summary>
    /// 处理玩家输入
    /// </summary>
    private void HandlePlayerInput()
    {
        // TODO: 处理玩家的各种输入
        // 例如：选择单位、移动、攻击、使用技能等
        
        // 示例：鼠标点击选择单位
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePosition = Input.mousePosition;
            Debug.Log($"玩家点击位置: {mousePosition}");
        }
    }
    
    /// <summary>
    /// 完成玩家回合
    /// </summary>
    private void FinishPlayerTurn()
    {
        Debug.Log("玩家回合完成，切换到敌人回合");
        gameManager.EndCurrentTurn();
    }
}