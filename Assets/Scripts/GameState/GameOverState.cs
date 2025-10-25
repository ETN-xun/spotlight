using UnityEngine;

/// <summary>
/// 游戏结束状态 - 胜利或失败的阶段
/// </summary>
public class GameOverState : GameStateBase
{
    private bool isVictory;
    
    public GameOverState(GameManager gameManager) : base(gameManager)
    {
    }
    
    public override void Enter()
    {
        base.Enter();
        
        // 判断胜负
        // isVictory = gameManager.AllEnemiesDefeated && !gameManager.IsCoreDestroyed;
        
        // 显示游戏结束UI
        ShowGameOverUI();
        
        // 停止游戏时间
        Time.timeScale = 0f;
        
        Debug.Log($"游戏结束 - {(isVictory ? "胜利" : "失败")}");
    }
    
    public override void Update()
    {
        // 处理游戏结束后的输入
        HandleGameOverInput();
    }
    
    public override void Exit()
    {
        base.Exit();
        
        // 隐藏游戏结束UI
        HideGameOverUI();
        
        // 恢复游戏时间
        Time.timeScale = 1f;
        
        Debug.Log("退出游戏结束状态");
    }
    
    /// <summary>
    /// 显示游戏结束UI
    /// </summary>
    private void ShowGameOverUI()
    {
        // TODO: 显示游戏结束相关的UI
        // 例如：胜利/失败画面、重新开始按钮、返回主菜单按钮等
        Debug.Log($"显示游戏结束UI - {(isVictory ? "胜利" : "失败")}画面");
    }
    
    /// <summary>
    /// 隐藏游戏结束UI
    /// </summary>
    private void HideGameOverUI()
    {
        // TODO: 隐藏游戏结束相关的UI
        Debug.Log("隐藏游戏结束UI");
    }
    
    /// <summary>
    /// 处理游戏结束后的输入
    /// </summary>
    private void HandleGameOverInput()
    {
        // R键重新开始游戏
        if (Input.GetKeyDown(KeyCode.R))
        {
            RestartGame();
        }
        
        // ESC键退出游戏
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            QuitGame();
        }
    }
    
    /// <summary>
    /// 重新开始游戏
    /// </summary>
    private void RestartGame()
    {
        Debug.Log("重新开始游戏");
        gameManager.RestartGame();
    }
    
    /// <summary>
    /// 退出游戏
    /// </summary>
    private void QuitGame()
    {
        Debug.Log("退出游戏");
        
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}