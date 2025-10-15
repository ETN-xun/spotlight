public class SelectEnemyUnitState : BaseInputState
{
    public SelectEnemyUnitState(InputStateMachine stateMachine) : base(stateMachine)
    {
    }
    
    public override void Enter()
    {
        
    }
    
    public override void Exit()
    {
        
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