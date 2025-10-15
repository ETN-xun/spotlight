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
        
       
        
        ViewManager.Instance.OpenView(ViewType.FightView);
    }

    public override void Exit()
    {
        base.Exit();
        
        
        ViewManager.Instance.CloseView(ViewType.FightView);
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
