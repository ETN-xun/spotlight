using System;
using Common;
using Enemy;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;

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
                }
            }
            return _instance;
        }
    }

    private GameState _currentGameState = GameState.Deployment;
    private GameStateBase _currentStateInstance;
    
    private DeploymentState _deploymentState;
    private EnemyTurnState _enemyTurnState;
    private PlayerTurnState _playerTurnState;
    private GameOverState _gameOverState;
    
    public GameState CurrentGameState => _currentGameState;
    
    public GameStateBase CurrentStateInstance => _currentStateInstance;

    [SerializeField] private int _currentTurn = 1;
    [SerializeField] private int _maxTurns = 10; // 最大回合数
    
    public int CurrentTurn => _currentTurn;
    

    public int MaxTurns => _maxTurns;
    
    public bool IsMaxTurnReached => _currentTurn >= _maxTurns;
    
    
    private void Awake()
    {
        // 确保单例唯一性
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        _instance = this;
        // 初始化游戏
        InitializeGame();
    }

    
    private void Update()
    {
        // 调用当前状态的Update方法
        _currentStateInstance?.Update();
    }
    private void Start()
    {
        // 开始游戏
        StartGame();
    }

    /// <summary>
    /// 初始化游戏
    /// </summary>
    private void InitializeGame()
    {
        _currentGameState = GameState.Deployment;
        _currentTurn = 1;
        
        // 初始化状态实例
        InitializeStates();
    }
    
    /// <summary>
    /// 初始化所有状态实例
    /// </summary>
    private void InitializeStates()
    {
        _deploymentState = new DeploymentState(this);
        _enemyTurnState = new EnemyTurnState(this);
        _playerTurnState = new PlayerTurnState(this);
        _gameOverState = new GameOverState(this);
        
        // 设置初始状态
        _currentStateInstance = _deploymentState;
        
    }
    
    private void StartGame()
    {
        GridManager.Instance.InitGrid();
        EnemyManager.Instance.InitEnemies();
        _currentStateInstance?.Enter();
    }


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

        // 退出当前状态
        _currentStateInstance?.Exit();
        
        // 更新状态
        _currentGameState = newState;
        _currentStateInstance = GetStateInstance(newState);
        
        Debug.Log($"GameManager: 状态转换 {oldState} -> {newState}");
        
        // 进入新状态
        _currentStateInstance?.Enter();
        
    }
    

    private GameStateBase GetStateInstance(GameState state)
    {
        switch (state)
        {
            case GameState.Deployment:
                return _deploymentState;
            case GameState.EnemyTurn:
                return _enemyTurnState;
            case GameState.PlayerTurn:
                return _playerTurnState;
            case GameState.GameOver:
                return _gameOverState;
            default:
                Debug.LogError($"GameManager: 未知状态 {state}");
                return null;
        }
    }
    

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
                break;
                
            case GameState.Deployment:
                // 部署阶段结束，进入敌人回合
                ChangeGameState(GameState.EnemyTurn);
                break;
        }
    }
    

    public void RestartGame()
    {
        Debug.Log("GameManager: 重新开始游戏");
        
        // 退出当前状态
        _currentStateInstance?.Exit();
        
        // 重新初始化游戏
        InitializeGame();
        StartGame();
    }
    
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
}