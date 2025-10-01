# GameManager 使用说明

## 概述
GameManager是游戏的核心管理器，采用单例模式，负责游戏状态机管理和回合流程控制。

## 主要功能

### 1. 游戏状态管理
- **Deployment**: 部署阶段，玩家放置单位
- **EnemyTurn**: 敌人回合，敌人执行行动
- **PlayerTurn**: 玩家回合，玩家控制单位行动
- **GameOver**: 游戏结束，胜利或失败

### 2. 回合流程控制
- 自动管理回合转换
- 跟踪当前回合数
- 支持最大回合数限制

### 3. 游戏结束条件
- 核心建筑被摧毁（失败）
- 所有敌人被击败（胜利）
- 达到最大回合数（失败）

### 4. 事件系统
- `OnGameStateChanged`: 游戏状态改变事件
- `OnTurnStarted`: 回合开始事件
- `OnGameEnded`: 游戏结束事件

## 使用方法

### 基本使用
```csharp
// 获取GameManager实例
GameManager gameManager = GameManager.Instance;

// 获取当前游戏状态
GameState currentState = gameManager.CurrentGameState;

// 切换游戏状态
gameManager.ChangeGameState(GameState.PlayerTurn);

// 结束当前回合
gameManager.EndCurrentTurn();
```

### 事件订阅
```csharp
private void Start()
{
    // 订阅事件
    GameManager.OnGameStateChanged += OnGameStateChanged;
    GameManager.OnTurnStarted += OnTurnStarted;
    GameManager.OnGameEnded += OnGameEnded;
}

private void OnDestroy()
{
    // 取消订阅
    GameManager.OnGameStateChanged -= OnGameStateChanged;
    GameManager.OnTurnStarted -= OnTurnStarted;
    GameManager.OnGameEnded -= OnGameEnded;
}

private void OnGameStateChanged(GameState oldState, GameState newState)
{
    Debug.Log($"状态改变: {oldState} -> {newState}");
}
```

### 游戏结束条件设置
```csharp
// 设置核心建筑被摧毁
GameManager.Instance.SetCoreDestroyed();

// 设置所有敌人被击败
GameManager.Instance.SetAllEnemiesDefeated();
```

## 状态转换规则
- Deployment -> EnemyTurn 或 GameOver
- EnemyTurn -> PlayerTurn 或 GameOver
- PlayerTurn -> EnemyTurn 或 GameOver
- GameOver -> 无法转换到其他状态

## 测试
使用 `GameManagerTest` 脚本进行功能测试：
- 按键1-5: 测试不同功能
- 按键R: 重新开始游戏
- GUI显示当前状态信息

## 注意事项
1. GameManager使用单例模式，确保全局唯一
2. 状态转换有严格的规则验证
3. 游戏结束后需要重新开始才能继续
4. 所有状态改变都会触发相应事件