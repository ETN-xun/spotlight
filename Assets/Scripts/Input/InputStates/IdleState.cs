using UnityEngine;

public sealed class IdleState : BaseInputState
{
    public IdleState(InputStateMachine stateMachine) : base(stateMachine) { }
    
    
    public override void Enter()
    {
        Debug.Log("Entering Input Idle State");
    }
    
    public override void Exit()
    {
        Debug.Log("Exiting Input Idle State");
    }

    public override void Update()
    {
        var inputType = InputManager.Instance.DetectInputType();
        switch (inputType)
        {
            case InputType.NoClick:
                break;
            case InputType.ClickPlayerUnit:
                stateMachine.ChangeState(InputState.SelectPlayerUnitState);
                break;
            case InputType.ClickEnemyUnit:
                stateMachine.ChangeState(InputState.SelectEnemyUnitState);
                break;
            case InputType.ClickNoUnit:
                stateMachine.ChangeState(InputState.SelectNoUnitState);
                break;
        }
    }
}