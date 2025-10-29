using Common;
using View;

public class SelectEnemyUnitState : BaseInputState
{
    private Unit CurrentSelectedUnit => InputManager.Instance.GetSelectedUnit();
    private GridCell CurrentSelectedCell => InputManager.Instance.GetSelectedCell();
    private Unit LastSelectedUnit { get; set; }
    private GridCell LastSelectedCell { get; set; }
    
    public SelectEnemyUnitState(InputStateMachine stateMachine) : base(stateMachine)
    {
    }
    
    public override void Enter()
    {
        if (CurrentSelectedUnit is null) return;
        LastSelectedUnit = CurrentSelectedUnit;
        GridManager.Instance.Highlight(true, CurrentSelectedUnit.CurrentCell.Coordinate);
        var moveRange = CurrentSelectedUnit.GetMoveRange();
        foreach (var cell in moveRange)
        {
            GridManager.Instance.Highlight(true, cell.Coordinate);
        }
        ViewManager.Instance.OpenView(ViewType.EnemyInfoView, "", CurrentSelectedUnit);
        // ViewManager.Instance.OpenView(ViewType.TerrainInfoView, "", CurrentSelectedCell);
    }
    
    public override void Exit()
    {
        GridManager.Instance.Highlight(false, LastSelectedUnit.CurrentCell.Coordinate);
        var moveRange = LastSelectedUnit.GetMoveRange();
        foreach (var cell in moveRange)
        {
            GridManager.Instance.Highlight(false, cell.Coordinate);
        }
        ViewManager.Instance.CloseView(ViewType.EnemyInfoView);
        // ViewManager.Instance.CloseView(ViewType.TerrainInfoView);
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