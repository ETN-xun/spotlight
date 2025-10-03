using UnityEngine;

/// <summary>
/// 敌人回合状态 - 敌人执行行动的阶段
/// </summary>
public class EnemyTurnState : GameStateBase
{
    private float turnTimer = 0f;
    private float maxTurnTime = 5f; // 敌人回合最大时间
    
    public EnemyTurnState(GameManager gameManager) : base(gameManager)
    {
    }
    
    public override void Enter()
    {
        base.Enter();
        
        // 重置计时器
        turnTimer = 0f;
        
        // 显示敌人回合UI
        ShowEnemyTurnUI();
        
        // 开始敌人AI行动
        StartEnemyActions();
        
        Debug.Log($"敌人回合开始 - 第{gameManager.CurrentTurn}回合");
    }
    
    public override void Update()
    {
        // 更新计时器
        turnTimer += Time.deltaTime;
        
        // 执行敌人AI逻辑
        UpdateEnemyActions();
        
        // 检查是否完成敌人回合
        if (turnTimer >= maxTurnTime || AreAllEnemyActionsComplete())
        {
            FinishEnemyTurn();
        }
    }
    
    public override void Exit()
    {
        base.Exit();
        
        // 隐藏敌人回合UI
        HideEnemyTurnUI();
        
        Debug.Log("敌人回合结束");
    }
    
    /// <summary>
    /// 显示敌人回合UI
    /// </summary>
    private void ShowEnemyTurnUI()
    {
        // TODO: 显示敌人回合相关的UI
        // 例如：回合指示器、敌人行动动画等
        Debug.Log("显示敌人回合UI");
    }
    
    /// <summary>
    /// 隐藏敌人回合UI
    /// </summary>
    private void HideEnemyTurnUI()
    {
        // TODO: 隐藏敌人回合相关的UI
        Debug.Log("隐藏敌人回合UI");
    }
    
    /// <summary>
    /// 开始敌人行动
    /// </summary>
    private void StartEnemyActions()
    {
        // TODO: 初始化敌人AI行动
        Debug.Log("开始敌人AI行动");
    }
    
    /// <summary>
    /// 更新敌人行动
    /// </summary>
    private void UpdateEnemyActions()
    {
        // TODO: 更新敌人AI逻辑
        // 例如：移动、攻击、使用技能等
    }
    
    /// <summary>
    /// 检查所有敌人行动是否完成
    /// </summary>
    /// <returns>是否完成</returns>
    private bool AreAllEnemyActionsComplete()
    {
        // TODO: 检查所有敌人是否完成行动
        // 这里暂时返回false，实际应该检查敌人状态
        return false;
    }
    
    /// <summary>
    /// 完成敌人回合
    /// </summary>
    private void FinishEnemyTurn()
    {
        Debug.Log("敌人回合完成，切换到玩家回合");
        gameManager.EndCurrentTurn();
    }
}