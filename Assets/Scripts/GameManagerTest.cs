using UnityEngine;

/// <summary>
/// GameManager测试脚本
/// 用于测试游戏状态机和事件系统的功能
/// </summary>
public class GameManagerTest : MonoBehaviour
{
    [Header("测试控制")]
    [SerializeField] private bool enableDebugKeys = true;
    
    private void Start()
    {
        // 订阅游戏事件
        GameManager.OnGameStateChanged += OnGameStateChanged;
        GameManager.OnTurnStarted += OnTurnStarted;
        GameManager.OnGameEnded += OnGameEnded;
        
        Debug.Log("GameManagerTest: 已订阅游戏事件");
    }
    
    private void OnDestroy()
    {
        // 取消订阅事件
        GameManager.OnGameStateChanged -= OnGameStateChanged;
        GameManager.OnTurnStarted -= OnTurnStarted;
        GameManager.OnGameEnded -= OnGameEnded;
    }
    
    private void Update()
    {
        if (!enableDebugKeys) return;
        
        // 测试按键
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.Log("测试: 切换到敌人回合");
            GameManager.Instance.ChangeGameState(GameState.EnemyTurn);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Debug.Log("测试: 切换到玩家回合");
            GameManager.Instance.ChangeGameState(GameState.PlayerTurn);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Debug.Log("测试: 结束当前回合");
            GameManager.Instance.EndCurrentTurn();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            Debug.Log("测试: 核心建筑被摧毁");
            GameManager.Instance.SetCoreDestroyed();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            Debug.Log("测试: 所有敌人被击败");
            GameManager.Instance.SetAllEnemiesDefeated();
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log("测试: 重新开始游戏");
            GameManager.Instance.RestartGame();
        }
    }
    
    #region 事件处理
    private void OnGameStateChanged(GameState oldState, GameState newState)
    {
        Debug.Log($"<color=yellow>事件: 游戏状态改变 {oldState} -> {newState}</color>");
    }
    
    private void OnTurnStarted(int turnNumber)
    {
        Debug.Log($"<color=green>事件: 第{turnNumber}回合开始</color>");
    }
    
    private void OnGameEnded(bool isVictory)
    {
        Debug.Log($"<color=red>事件: 游戏结束 - {(isVictory ? "胜利" : "失败")}</color>");
    }
    #endregion
    
    #region GUI显示
    private void OnGUI()
    {
        if (!enableDebugKeys) return;
        
        GUILayout.BeginArea(new Rect(10, 10, 300, 200));
        GUILayout.Label("GameManager 测试控制", GUI.skin.box);
        
        GUILayout.Label($"当前状态: {GameManager.Instance.CurrentGameState}");
        GUILayout.Label($"当前回合: {GameManager.Instance.CurrentTurn}/{GameManager.Instance.MaxTurns}");
        GUILayout.Label($"核心被摧毁: {GameManager.Instance.IsCoreDestroyed}");
        GUILayout.Label($"敌人全灭: {GameManager.Instance.AllEnemiesDefeated}");
        
        GUILayout.Space(10);
        GUILayout.Label("测试按键:");
        GUILayout.Label("1 - 敌人回合");
        GUILayout.Label("2 - 玩家回合");
        GUILayout.Label("3 - 结束回合");
        GUILayout.Label("4 - 核心被摧毁");
        GUILayout.Label("5 - 敌人全灭");
        GUILayout.Label("R - 重新开始");
        
        GUILayout.EndArea();
    }
    #endregion
}