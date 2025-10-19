using UnityEngine;
using View;
using View.GameViews;

public class SelectNoUnitState : BaseInputState
{
    private GridCell CurrentSelectedCell => InputManager.Instance.GetSelectedCell();
    public SelectNoUnitState(InputStateMachine stateMachine) : base(stateMachine)
    {
    }
    
    public override void Enter()
    {
        ViewManager.Instance.OpenView(ViewType.TerrainInfoView, "", CurrentSelectedCell);
        
    }
    
    public override void Exit()
    {
        ViewManager.Instance.CloseView(ViewType.TerrainInfoView);
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
            case InputType.CancelClick:
                stateMachine.ChangeState(InputState.IdleState);
                break;
        }
    }
}