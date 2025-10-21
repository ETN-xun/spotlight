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
        EnemyManager.Instance.StartEnemyTurnFlow();
    }
    
    public override void Update()
    {
        // 更新计时器
        _turnTimer += Time.deltaTime;

        if (EnemyManager.Instance.CurrentEnemyTurn == 1)
        {
            if (EnemyManager.Instance.EnemyIntentsShowFinished)
            {
                gameManager.ChangeGameState(GameState.PlayerTurn);
                return;
            }
        }
        if (EnemyManager.Instance.EnemyIntentsExecuteFinished && EnemyManager.Instance.EnemyIntentsShowFinished)
            gameManager.ChangeGameState(GameState.PlayerTurn);
    }
    
    public override void Exit()
    {
        base.Exit();
        
        Debug.Log("敌人回合结束");
        GridManager.Instance.ClearAllHighlights();
        EnemyManager.Instance.RemoveNullPointerAttackedUnits();
        MessageCenter.Publish(Defines.EnemyTurnEndEvent);
    }
}