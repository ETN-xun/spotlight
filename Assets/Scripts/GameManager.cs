using System;
using System.Collections.Generic;
using Common;
using Enemy;
using Action;
using Scene;
using UnityEngine;
using UnityEngine.SceneManagement;
using View;

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
    
    public DialogueTrigger demoStartTrigger;
    public DialogueTrigger level1StartTrigger;
    public DialogueTrigger level1EndTrigger;
    public DialogueTrigger level2StartTrigger;
    public DialogueTrigger level2EndTrigger;
    public DialogueTrigger level3StartTrigger;
    public DialogueTrigger level3EndTrigger;
    public GameObject sceneTransitionCover; 
    // 内部状态变量
    private Queue<DialogueTrigger> dialogueChainQueue = new Queue<DialogueTrigger>();
    private bool isChainedPlaybackActive = false; // 标记是否处于“剧情链自动播放”模式
    private int _pendingUnlockLevel = 0; // 当播放关卡收束剧情结束后需要解锁的关卡索引（无则为0）
    private bool _lastGameWasVictory = false; // 记录最近一次游戏结束是否为胜利
    
    
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
        // 根据选中的关卡启动对应的开场剧情
        int levelIndex = Level.LevelManager.Instance != null
            ? Level.LevelManager.Instance.GetCurrentLevelIndex()
            : 1;
        switch (levelIndex)
        {
            case 1:
                StartDialogueChain(demoStartTrigger, level1StartTrigger);
                break;
            case 2:
                StartDialogueChain(level2StartTrigger);
                break;
            case 3:
                StartDialogueChain(level3StartTrigger);
                break;
            default:
                StartDialogueChain(demoStartTrigger, level1StartTrigger);
                break;
        }
    }
    private void OnEnable()
    {
        DialoguePlayer.OnSectionEnd += HandleSectionEndEvent;
        DialoguePlayer.OnDialogueEnded += HandleDialogueEnded;
    }

    private void OnDisable()
    {
        DialoguePlayer.OnSectionEnd -= HandleSectionEndEvent;
        DialoguePlayer.OnDialogueEnded -= HandleDialogueEnded;
    }
    
    public void PlayerCompletedLevel(int levelIndex)
    {
        // 完成关卡后，仅播放该关卡的收束剧情；解锁与返回由剧情事件驱动
        // 同时记录在收束剧情播放完毕后需要解锁的下一关（如果存在）
        _pendingUnlockLevel = levelIndex < 3 ? levelIndex + 1 : 0;
        switch (levelIndex)
        {
            case 1:
                StartDialogueChain(level1EndTrigger);
                break;
            case 2:
                StartDialogueChain(level2EndTrigger);
                break;
            case 3:
                StartDialogueChain(level3EndTrigger);
                break;
            default:
                StartDialogueChain(level1EndTrigger);
                break;
        }
    }

    /// <summary>
    /// 准备并启动一个剧情链。
    /// </summary>
    /// <param name="triggers">要按顺序播放的剧情触发器数组。</param>
    private void StartDialogueChain(params DialogueTrigger[] triggers)
    {
        dialogueChainQueue.Clear();
        foreach (var trigger in triggers)
        {
            if (trigger != null) dialogueChainQueue.Enqueue(trigger);
        }
        PlayNextInChain();
    }


    /// <summary>
    /// 播放剧情链队列中的下一个剧情。
    /// </summary>
    private void PlayNextInChain()
    {
        if (dialogueChainQueue.Count > 0)
        {
            dialogueChainQueue.Dequeue().Trigger();
        }
    }

private void HandleSectionEndEvent(string eventName)
{
    Debug.Log(eventName);
    if (eventName.StartsWith("StartLevel"))
    {
        dialogueChainQueue.Clear();
        if (eventName == "StartLevel1" || eventName == "StartLevel2" || eventName == "StartLevel3")
        {
            StartGame();
        }
        DialoguePlayer.ResetSkipAll();
    }
    else if (eventName == "ReturnToMenu")
    {
        dialogueChainQueue.Clear();
        // 若存在待解锁关卡，优先解锁，避免切场导致 OnDialogueEnded 未触发
        UnlockPendingIfAny();
        if (sceneTransitionCover != null)
        {
            sceneTransitionCover.SetActive(true);
        }
        SceneLoadManager.Instance.LoadScene(SceneType.MainMenu);
        DialoguePlayer.ResetSkipAll();
    }
    else if (eventName == "ReturnToLevelSelect" || eventName == "GoToLevelSelect" || eventName == "BackToLevelSelect")
    {
        dialogueChainQueue.Clear();
        // 若存在待解锁关卡，优先解锁，避免切场导致 OnDialogueEnded 未触发
        UnlockPendingIfAny();
        // 保险：基于当前关卡直接计算下一关并写入解锁，避免事件顺序导致未解锁
        TryUnlockNextLevelByCurrent();
        SceneLoadManager.Instance.LoadScene(SceneType.LevelSelect);
        DialoguePlayer.ResetSkipAll();
    }
    else if (eventName.StartsWith("UnlockLevel"))
    {
        int target = 0;
        for (int i = 0; i < eventName.Length; i++)
        {
            if (char.IsDigit(eventName[i]))
            {
                target = eventName[i] - '0';
                break;
            }
        }
        if (target > 0)
        {
            int current = PlayerPrefs.GetInt("UnlockedLevels", 1);
            if (target > current)
            {
                PlayerPrefs.SetInt("UnlockedLevels", target);
                PlayerPrefs.Save();
            }
        }
        SceneLoadManager.Instance.LoadScene(SceneType.LevelSelect);
        DialoguePlayer.ResetSkipAll();
    }
    else
    {
        PlayNextInChain();
    }
}

    /// <summary>
    /// 若存在待解锁关卡，则立即写入解锁并清空标记。
    /// </summary>
    private void UnlockPendingIfAny()
    {
        if (_pendingUnlockLevel > 0)
        {
            int current = PlayerPrefs.GetInt("UnlockedLevels", 1);
            if (_pendingUnlockLevel > current)
            {
                PlayerPrefs.SetInt("UnlockedLevels", _pendingUnlockLevel);
                PlayerPrefs.Save();
            }
            _pendingUnlockLevel = 0;
        }
    }

    /// <summary>
    /// 基于当前关卡尝试解锁下一关（与待解锁标记互不冲突，取更大值）。
    /// </summary>
    private void TryUnlockNextLevelByCurrent()
    {
        // 仅在胜利的情况下才进行兜底解锁，避免失败时误解锁
        if (!_lastGameWasVictory)
        {
            return;
        }
        int currentLevelIndex = Level.LevelManager.Instance != null
            ? Level.LevelManager.Instance.GetCurrentLevelIndex()
            : 1;

        // 仅当存在下一关时才尝试解锁
        if (currentLevelIndex < 3)
        {
            int next = currentLevelIndex + 1;
            int unlocked = PlayerPrefs.GetInt("UnlockedLevels", 1);
            if (next > unlocked)
            {
                PlayerPrefs.SetInt("UnlockedLevels", next);
                PlayerPrefs.Save();
            }
        }
    }

    /// <summary>
    /// 由 GameOverState 汇报本局胜负结果，用于后续剧情与解锁逻辑。
    /// </summary>
    public void ReportGameResult(bool victory)
    {
        _lastGameWasVictory = victory;
    }

    /// <summary>
    /// 当某段剧情播放完全结束时的处理（不依赖 SectionEnd 节点）。
    /// 用于关卡收束剧情结束后自动解锁下一关并返回选关界面。
    /// </summary>
    private void HandleDialogueEnded()
    {
        // 若仍有后续剧情链，则继续播放
        if (dialogueChainQueue.Count > 0)
        {
            PlayNextInChain();
            return;
        }

        // 若标记了需要解锁的下一关，则在收束剧情完整结束时执行解锁并返回选关
        if (_pendingUnlockLevel > 0)
        {
            int current = PlayerPrefs.GetInt("UnlockedLevels", 1);
            if (_pendingUnlockLevel > current)
            {
                PlayerPrefs.SetInt("UnlockedLevels", _pendingUnlockLevel);
                PlayerPrefs.Save();
            }
            SceneLoadManager.Instance.LoadScene(SceneType.LevelSelect);
            _pendingUnlockLevel = 0;
            DialoguePlayer.ResetSkipAll();
        }
    }


    /// <summary>
    /// 初始化游戏
    /// </summary>
    private void InitializeGame()
    {
        _currentGameState = GameState.Deployment;
        _currentTurn = 1;
        // 重置能量系统，确保重新进入关卡时从初始能量开始
        ActionManager.EnergySystem.ResetForNewLevel();
        
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
        // 每次真正开始战斗前重置能量，防止继承上一局
        ActionManager.EnergySystem.ResetForNewLevel();
        GridManager.Instance.InitGrid();
        EnemyManager.Instance.InitEnemies();
        ViewManager.Instance.OpenView(ViewType.FightView);
        _currentStateInstance?.Enter();
        Debug.Log("000000Game Started");
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
        
        // 进入新状态（关卡收束剧情仅在 GameOverState 内部根据胜负判定触发）
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
        StartDialogueChain(demoStartTrigger, level1StartTrigger);
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
