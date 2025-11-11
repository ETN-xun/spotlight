using UnityEngine;
using Enemy;
using Ally;

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
        int aliveEnemies = EnemyManager.Instance != null ? EnemyManager.Instance.GetAliveEnemies().Count : 0;
        int aliveAllies = AllyManager.Instance != null ? AllyManager.Instance.GetAliveAllies().Count : 0;

        // 第二关特殊胜利条件：零到达左上角（x最小，y最大）即胜利；其它关卡沿用原逻辑
        int currentLevelIndex = Level.LevelManager.Instance != null
            ? Level.LevelManager.Instance.GetCurrentLevelIndex()
            : -1;

        if (currentLevelIndex == 2)
        {
            bool zeroAtTopLeft = false;
            var allies = AllyManager.Instance != null ? AllyManager.Instance.GetAliveAllies() : null;
            if (allies != null)
            {
                var zero = allies.Find(a => a.data.unitType == UnitType.Zero);
                if (zero != null && zero.CurrentCell != null && GridManager.Instance != null)
                {
                    int targetX = GridManager.Instance.GetMinX();
                    int targetY = GridManager.Instance.GetMaxY();
                    var coord = zero.CurrentCell.Coordinate;
                    zeroAtTopLeft = coord.x == targetX && coord.y == targetY;
                }
            }
            isVictory = zeroAtTopLeft;
        }
        else
        {
            isVictory = aliveEnemies == 0 && aliveAllies > 0;
        }
        
        // 将结果汇报给 GameManager，用于后续剧情与解锁逻辑
        gameManager.ReportGameResult(isVictory);
        
        // 若胜利则触发收束剧情（用于解锁下一关与返回选关）
        if (isVictory)
        {
            int levelIndex = Level.LevelManager.Instance != null
                ? Level.LevelManager.Instance.GetCurrentLevelIndex()
                : 1;
            gameManager.PlayerCompletedLevel(levelIndex);
        }
        
        // 显示游戏结束UI
        ShowGameOverUI();
        
        // 停止游戏时间
        //Time.timeScale = 0f;
        
        //GameManager.Instance.PlayerCompletedLevel(1);
        
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
