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
        
    }

    public override void Exit()
    {
        base.Exit();
        GridManager.Instance.ClearAllHighlights();
        MessageCenter.Publish(Defines.PlayerTurnEndEvent);
    }
    
    public override void Update()
    {
        _inputStateMachine.Update();
    }
    


    private void FinishPlayerTurn()
    {
        
        gameManager.EndCurrentTurn();
    }
}
