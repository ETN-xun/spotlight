using System;
using UnityEngine;

/// <summary>
/// 游戏管理器 - 游戏运行总控，状态机管理
/// 单例模式，负责控制游戏状态转换和回合流程
/// </summary>
public class GameManager : MonoBehaviour
{
    #region 单例模式
    private static GameManager _instance;
    
    /// <summary>
    /// 游戏管理器单例实例
    /// </summary>
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GameManager>();
                if (_instance == null)
                {
                    GameObject gameManagerObject = new GameObject("GameManager");
                    _instance = gameManagerObject.AddComponent<GameManager>();
                    DontDestroyOnLoad(gameManagerObject);
                }
            }
            return _instance;
        }
    }
    #endregion

    #region 游戏状态
    [SerializeField] private GameState _currentGameState = GameState.Deployment;
    
    /// <summary>
    /// 当前游戏状态（只读）
    /// </summary>
    public GameState CurrentGameState => _currentGameState;
    #endregion

    #region 回合管理
    [SerializeField] private int _currentTurn = 1;
    [SerializeField] private int _maxTurns = 10; // 最大回合数
    
    /// <summary>
    /// 当前回合数
    /// </summary>
    public int CurrentTurn => _currentTurn;
    
    /// <summary>
    /// 最大回合数
    /// </summary>
    public int MaxTurns => _maxTurns;
    
    /// <summary>
    /// 是否达到最大回合数
    /// </summary>
    public bool IsMaxTurnReached => _currentTurn >= _maxTurns;
    #endregion

    #region 游戏结束条件
    [SerializeField] private bool _isCoreDestroyed = false;
    [SerializeField] private bool _allEnemiesDefeated = false;
    
    /// <summary>
    /// 核心建筑是否被摧毁
    /// </summary>
    public bool IsCoreDestroyed => _isCoreDestroyed;
    
    /// <summary>
    /// 所有敌人是否被击败
    /// </summary>
    public bool AllEnemiesDefeated => _allEnemiesDefeated;
    #endregion

    #region 事件系统
    /// <summary>
    /// 游戏状态改变事件 (旧状态, 新状态)
    /// </summary>
    public static event Action<GameState, GameState> OnGameStateChanged;
    
    /// <summary>
    /// 回合开始事件
    /// </summary>
    public static event Action<int> OnTurnStarted;
    
    /// <summary>
    /// 游戏结束事件 (是否胜利)
    /// </summary>
    public static event Action<bool> OnGameEnded;
    #endregion

    #region Unity生命周期
    private void Awake()
    {
        // 确保单例唯一性
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        _instance = this;
        DontDestroyOnLoad(gameObject);
        
        // 初始化游戏
        InitializeGame();
    }

    private void Start()
    {
        // 开始游戏
        StartGame();
    }
    #endregion

    #region 游戏初始化
    /// <summary>
    /// 初始化游戏
    /// </summary>
    private void InitializeGame()
    {
        _currentGameState = GameState.Deployment;
        _currentTurn = 1;
        _isCoreDestroyed = false;
        _allEnemiesDefeated = false;
        
        Debug.Log("GameManager: 游戏初始化完成");
    }
    
    /// <summary>
    /// 开始游戏
    /// </summary>
    private void StartGame()
    {
        Debug.Log("GameManager: 游戏开始 - 进入部署阶段");
        OnTurnStarted?.Invoke(_currentTurn);
    }
    #endregion

    #region 状态管理
    /// <summary>
    /// 改变游戏状态
    /// </summary>
    /// <param name="newState">新的游戏状态</param>
    public void ChangeGameState(GameState newState)
    {
        if (_currentGameState == newState)
        {
            Debug.LogWarning($"GameManager: 尝试切换到相同状态 {newState}");
            return;
        }

        GameState oldState = _currentGameState;
        
        // 验证状态转换是否合法
        if (!IsValidStateTransition(oldState, newState))
        {
            Debug.LogError($"GameManager: 非法状态转换 {oldState} -> {newState}");
            return;
        }

        _currentGameState = newState;
        
        Debug.Log($"GameManager: 状态转换 {oldState} -> {newState}");
        
        // 处理状态转换逻辑
        HandleStateTransition(oldState, newState);
        
        // 触发状态改变事件
        OnGameStateChanged?.Invoke(oldState, newState);
    }
    
    /// <summary>
    /// 验证状态转换是否合法
    /// </summary>
    /// <param name="from">源状态</param>
    /// <param name="to">目标状态</param>
    /// <returns>是否合法</returns>
    private bool IsValidStateTransition(GameState from, GameState to)
    {
        // 游戏结束后不能转换到其他状态
        if (from == GameState.GameOver)
            return false;
            
        // 定义合法的状态转换
        switch (from)
        {
            case GameState.Deployment:
                return to == GameState.EnemyTurn || to == GameState.GameOver;
                
            case GameState.EnemyTurn:
                return to == GameState.PlayerTurn || to == GameState.GameOver;
                
            case GameState.PlayerTurn:
                return to == GameState.EnemyTurn || to == GameState.GameOver;
                
            default:
                return false;
        }
    }
    
    /// <summary>
    /// 处理状态转换逻辑
    /// </summary>
    /// <param name="oldState">旧状态</param>
    /// <param name="newState">新状态</param>
    private void HandleStateTransition(GameState oldState, GameState newState)
    {
        switch (newState)
        {
            case GameState.EnemyTurn:
                HandleEnemyTurnStart();
                break;
                
            case GameState.PlayerTurn:
                HandlePlayerTurnStart();
                break;
                
            case GameState.GameOver:
                HandleGameOver();
                break;
        }
    }
    #endregion

    #region 回合流程控制
    /// <summary>
    /// 处理敌人回合开始
    /// </summary>
    private void HandleEnemyTurnStart()
    {
        Debug.Log($"GameManager: 敌人回合开始 - 第{_currentTurn}回合");
        // 这里将来会调用敌人AI系统
    }
    
    /// <summary>
    /// 处理玩家回合开始
    /// </summary>
    private void HandlePlayerTurnStart()
    {
        Debug.Log($"GameManager: 玩家回合开始 - 第{_currentTurn}回合");
        // 这里将来会重置玩家行动点数等
    }
    
    /// <summary>
    /// 结束当前回合，进入下一回合
    /// </summary>
    public void EndCurrentTurn()
    {
        switch (_currentGameState)
        {
            case GameState.EnemyTurn:
                // 敌人回合结束，进入玩家回合
                ChangeGameState(GameState.PlayerTurn);
                break;
                
            case GameState.PlayerTurn:
                // 玩家回合结束，增加回合数，进入敌人回合
                _currentTurn++;
                OnTurnStarted?.Invoke(_currentTurn);
                
                // 检查是否达到最大回合数
                if (CheckGameEndConditions())
                {
                    ChangeGameState(GameState.GameOver);
                }
                else
                {
                    ChangeGameState(GameState.EnemyTurn);
                }
                break;
                
            case GameState.Deployment:
                // 部署阶段结束，进入敌人回合
                ChangeGameState(GameState.EnemyTurn);
                break;
        }
    }
    #endregion

    #region 游戏结束条件检查
    /// <summary>
    /// 检查游戏结束条件
    /// </summary>
    /// <returns>是否应该结束游戏</returns>
    public bool CheckGameEndConditions()
    {
        // 核心建筑被摧毁 - 失败
        if (_isCoreDestroyed)
        {
            Debug.Log("GameManager: 核心建筑被摧毁，游戏失败");
            return true;
        }
        
        // 所有敌人被击败 - 胜利
        if (_allEnemiesDefeated)
        {
            Debug.Log("GameManager: 所有敌人被击败，游戏胜利");
            return true;
        }
        
        // 达到最大回合数 - 失败
        if (IsMaxTurnReached)
        {
            Debug.Log("GameManager: 达到最大回合数，游戏失败");
            return true;
        }
        
        return false;
    }
    
    /// <summary>
    /// 设置核心建筑被摧毁
    /// </summary>
    public void SetCoreDestroyed()
    {
        _isCoreDestroyed = true;
        Debug.Log("GameManager: 核心建筑被摧毁");
        
        if (_currentGameState != GameState.GameOver)
        {
            ChangeGameState(GameState.GameOver);
        }
    }
    
    /// <summary>
    /// 设置所有敌人被击败
    /// </summary>
    public void SetAllEnemiesDefeated()
    {
        _allEnemiesDefeated = true;
        Debug.Log("GameManager: 所有敌人被击败");
        
        if (_currentGameState != GameState.GameOver)
        {
            ChangeGameState(GameState.GameOver);
        }
    }
    
    /// <summary>
    /// 处理游戏结束
    /// </summary>
    private void HandleGameOver()
    {
        bool isVictory = _allEnemiesDefeated && !_isCoreDestroyed;
        Debug.Log($"GameManager: 游戏结束 - {(isVictory ? "胜利" : "失败")}");
        
        OnGameEnded?.Invoke(isVictory);
    }
    #endregion

    #region 公共接口
    /// <summary>
    /// 重新开始游戏
    /// </summary>
    public void RestartGame()
    {
        Debug.Log("GameManager: 重新开始游戏");
        InitializeGame();
        StartGame();
    }
    
    /// <summary>
    /// 暂停游戏
    /// </summary>
    public void PauseGame()
    {
        Time.timeScale = 0f;
        Debug.Log("GameManager: 游戏暂停");
    }
    
    /// <summary>
    /// 恢复游戏
    /// </summary>
    public void ResumeGame()
    {
        Time.timeScale = 1f;
        Debug.Log("GameManager: 游戏恢复");
    }
    #endregion
}