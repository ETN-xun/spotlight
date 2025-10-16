using Action;
using Common;
using UnityEngine;
using View;

public class SelectPlayerUnitState : BaseInputState     // TODO：逻辑还得再理理
{
    private Unit CurrentSelectedUnit => InputManager.Instance.GetSelectedUnit();
    private GridCell CurrentSelectedCell => InputManager.Instance.GetSelectedCell();
    private Unit LastSelectedUnit { get; set; }
    private GridCell LastSelectedCell { get; set; }
    
    private bool _isPreparingSkill;
    
    private Skill _pendingSkill;
    
    public SelectPlayerUnitState(InputStateMachine stateMachine) : base(stateMachine)
    {
    }
    
    public override void Enter()
    {
        MessageCenter.Subscribe(Defines.ClickSkillViewEvent, OnClickSkillView);
        
        // 移动范围高亮，打开单位信息面板
        if (CurrentSelectedUnit is null) return;
        LastSelectedUnit = CurrentSelectedUnit;
        GridManager.Instance.Highlight(true, CurrentSelectedUnit.CurrentCell.Coordinate);
        var moveRange = CurrentSelectedUnit.GetMoveRange();
        foreach (var cell in moveRange)
        {
            GridManager.Instance.Highlight(true, cell.Coordinate);
        }
        ViewManager.Instance.OpenView(ViewType.UnitInfoView, "", CurrentSelectedUnit);
    }
    
    public override void Exit()
    {
        GridManager.Instance.Highlight(false, LastSelectedUnit.CurrentCell.Coordinate);
        var moveRange = LastSelectedUnit.GetMoveRange();
        foreach (var cell in moveRange)
        {
            GridManager.Instance.Highlight(false, cell.Coordinate);
        }
        ViewManager.Instance.CloseView(ViewType.UnitInfoView);
        
        MessageCenter.Unsubscribe(Defines.ClickSkillViewEvent, OnClickSkillView);
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
                HandleClickEnemyUnit();
                break;
            case InputType.ClickNoUnit:
                HandleNoUnitClick();
                break;
        }
    }
    
    private void HandleClickEnemyUnit()
    {
        if (_isPreparingSkill)
        {
            
        }
        stateMachine.ChangeState(InputState.SelectEnemyUnitState);
    }
    
    private void HandleNoUnitClick()
    {
        Debug.Log("No unit selected");
        if (_isPreparingSkill)
        {
            // 判断技能范围
        }
        if (!CurrentSelectedUnit.hasMoved)
        {
            var moveRangeCells = LastSelectedUnit.GetMoveRange();
            if (moveRangeCells.Contains(CurrentSelectedCell))
            {
                // ActionManager.Instance.move_action;
                
                
                GridManager.Instance.Highlight(false, LastSelectedUnit.CurrentCell.Coordinate);
                var moveRange = LastSelectedUnit.GetMoveRange();
                foreach (var cell in moveRange)
                {
                    GridManager.Instance.Highlight(false, cell.Coordinate);
                }
                ViewManager.Instance.CloseView(ViewType.UnitInfoView);
                
                LastSelectedUnit.MoveTo(CurrentSelectedCell);
                LastSelectedUnit.hasMoved = true;
            }
        }
        
    }
    
    private void OnClickSkillView(object[] obj)
    {
        if (obj[0] is not Skill skill) return;
        _pendingSkill = skill;
        _isPreparingSkill = true;
    }
}