using Action;
using Common;
using UnityEngine;
using View;

public class SelectPlayerUnitState : BaseInputState
{
    private Unit CurrentSelectedUnit => InputManager.Instance.GetSelectedUnit();
    private Unit LastSelectedUnit { get; set; }
    
    private bool _hasMoved;
    private bool _isPreparingSkill;
    
    private Skill _pendingSkill;
    
    public SelectPlayerUnitState(InputStateMachine stateMachine) : base(stateMachine)
    {
    }
    
    public override void Enter()
    {
        MessageCenter.Subscribe(Defines.ClickSkillViewEvent, OnClickSkillView);
        
        // 移动范围高亮，打开单位信息面板
        if (CurrentSelectedUnit is null)
            return;
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
        if (_isPreparingSkill)
        {
            // 判断技能范围
        }
        if (!_hasMoved)
        {
            var moveRangeCells = LastSelectedUnit.GetMoveRange();
            if (moveRangeCells.Contains(CurrentSelectedUnit.CurrentCell))
            {
                // ActionManager.Instance.move_action;
                LastSelectedUnit.MoveTo(CurrentSelectedUnit.CurrentCell);
                _hasMoved = true;
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