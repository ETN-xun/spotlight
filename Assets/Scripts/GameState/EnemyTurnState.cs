using Common;
using Enemy;
using UnityEngine;

/// <summary>
/// 敌人回合状态 - 敌人执行行动的阶段
/// </summary>
public class EnemyTurnState : GameStateBase
{
    private float _turnTimer;
    private const float MaxTurnTime = 5f; 

    public EnemyTurnState(GameManager gameManager) : base(gameManager)
    {
    }
    
    public override void Enter()
    {
        base.Enter();
        
        _turnTimer = 0f;
        MessageCenter.Publish(Defines.EnemyTurnStartEvent);
        EnemyManager.Instance.StartEnemyTurn();
        // 如果是第一次敌人回合，只展示意图，不进行攻击
        // 如果不是第一次敌人回合，执行上一回合的意图，然后展示新的意图
        if (EnemyManager.Instance.CurrentEnemyTurn == 1)
        {

            return;
        }
        
    }
    
    public override void Update()
    {
        // 更新计时器
        _turnTimer += Time.deltaTime;
        
        
        // 检查是否完成敌人回合
        if (_turnTimer >= MaxTurnTime || AreAllEnemyActionsComplete())
        {
            FinishEnemyTurn();
        }
    }
    
    public override void Exit()
    {
        base.Exit();
        
        Debug.Log("敌人回合结束");
        MessageCenter.Publish(Defines.EnemyTurnEndEvent);
        EnemyManager.Instance.EndEnemyTurn();
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
        gameManager.ChangeGameState(GameState.PlayerTurn);
    }
}