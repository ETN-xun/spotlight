using UnityEngine;

/// <summary>
/// 游戏状态基类
/// 提供状态的生命周期方法：Enter、Update、Exit
/// </summary>
public abstract class GameStateBase
{
    protected GameManager gameManager;
    
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="gameManager">游戏管理器引用</param>
    public GameStateBase(GameManager gameManager)
    {
        this.gameManager = gameManager;
    }
    
    /// <summary>
    /// 进入状态时调用
    /// </summary>
    public virtual void Enter()
    {
        Debug.Log($"进入状态: {GetType().Name}");
    }
    
    /// <summary>
    /// 状态更新时调用（每帧）
    /// </summary>
    public virtual void Update()
    {
        // 默认为空，子类可以重写
    }
    
    /// <summary>
    /// 退出状态时调用
    /// </summary>
    public virtual void Exit()
    {
        Debug.Log($"退出状态: {GetType().Name}");
    }
}