using System;

/// <summary>
/// 游戏状态枚举
/// 定义游戏的四个主要阶段
/// </summary>
public enum GameState
{
    /// <summary>
    /// 部署阶段 - 玩家放置单位
    /// </summary>
    Deployment,
    
    /// <summary>
    /// 敌人回合 - 敌人执行行动
    /// </summary>
    EnemyTurn,
    
    /// <summary>
    /// 玩家回合 - 玩家控制单位行动
    /// </summary>
    PlayerTurn,
    
    /// <summary>
    /// 游戏结束 - 胜利或失败
    /// </summary>
    GameOver
}