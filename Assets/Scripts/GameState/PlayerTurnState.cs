using System;
using Common;
using UnityEngine;
using View;



public class PlayerTurnState : GameStateBase
{
    private Camera _mainCamera;
    private Unit _selectedUnit;
    private readonly InputStateMachine _inputStateMachine;


    public PlayerTurnState(GameManager gameManager) : base(gameManager)
    {
        _inputStateMachine = new InputStateMachine();
    }

    public override void Enter()
    {
        base.Enter();
        _mainCamera = Camera.main;
        MessageCenter.Publish(Defines.PlayerTurnStartEvent);
        // 订阅能量变化事件：能量耗尽则自动结束我方回合
        MessageCenter.Subscribe(Defines.EnergyChangedEvent, OnEnergyChanged);
        
        // 进入玩家回合时若当前能量已为零，直接结束回合
        var currentEnergy = Action.ActionManager.EnergySystem.GetCurrentEnergy();
        if (currentEnergy <= 0)
        {
            GameManager.Instance.ChangeGameState(GameState.EnemyTurn);
        }
        
    }

    public override void Exit()
    {
        base.Exit();
        GridManager.Instance.ClearAllHighlights();
        MessageCenter.Publish(Defines.PlayerTurnEndEvent);
        // 取消订阅，避免在非玩家回合触发
        MessageCenter.Unsubscribe(Defines.EnergyChangedEvent, OnEnergyChanged);
    }
    
    public override void Update()
    {
        _inputStateMachine.Update();
    }
    


    private void FinishPlayerTurn()
    {
        
        gameManager.EndCurrentTurn();
    }

    /// <summary>
    /// 能量变化回调：在玩家回合期间当能量耗尽（<=0）时自动结束回合
    /// </summary>
    private void OnEnergyChanged(object[] args)
    {
        if (args == null || args.Length == 0) return;
        if (args[0] is not int energy) return;

        // 仅在玩家回合内处理自动结束
        if (energy <= 0 && gameManager.CurrentGameState == GameState.PlayerTurn)
        {
            GameManager.Instance.ChangeGameState(GameState.EnemyTurn);
        }
    }
}
