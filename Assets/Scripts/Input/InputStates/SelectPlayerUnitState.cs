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
    
    private SkillDataSO _pendingSkill;
    
    public SelectPlayerUnitState(InputStateMachine stateMachine) : base(stateMachine)
    {
    }
    
    public override void Enter()
    {
        MessageCenter.Subscribe(Defines.ClickSkillViewEvent, OnClickSkillView);
        _isPreparingSkill = false;
        if (CurrentSelectedUnit is null) return;
        LastSelectedUnit = CurrentSelectedUnit;
        GridManager.Instance.Highlight(true, CurrentSelectedUnit.CurrentCell.Coordinate);
        var moveRange = CurrentSelectedUnit.GetMoveRange();
        foreach (var cell in moveRange)
        {
            GridManager.Instance.Highlight(true, cell.Coordinate);
        }
        ViewManager.Instance.OpenView(ViewType.UnitInfoView, "", CurrentSelectedUnit);
        ViewManager.Instance.OpenView(ViewType.TerrainInfoView, "", CurrentSelectedCell);
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
        ViewManager.Instance.CloseView(ViewType.TerrainInfoView);
        
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
            case InputType.CancelClick:
                stateMachine.ChangeState(InputState.IdleState);
                break;
        }
    }
    
    private void HandleClickEnemyUnit()
    {
        if (_isPreparingSkill)
        {
            if (LastSelectedUnit is null)
            {
                Debug.Log("No unit selected to use the skill.");
                return;
            }
            var attackRange = LastSelectedUnit.GetAttackRange(LastSelectedCell);
            if (attackRange.Count == 0)
            {
                Debug.Log("No valid targets in range for the skill.");
                return;
            }
            if (attackRange.Contains(CurrentSelectedCell))
            {
                // ActionManager.Instance.ExecuteSkillAction(LastSelectedUnit, _pendingSkill, CurrentSelectedCell);
                if (!ActionManager.EnergySystem.TrySpendEnergy(_pendingSkill.energyCost))
                {
                    Debug.Log("Not enough energy to use the skill.");
                    _isPreparingSkill = false;
                    stateMachine.ChangeState(InputState.IdleState);
                }
                SkillSystem.Instance.StartSkill(LastSelectedUnit, _pendingSkill);
                SkillSystem.Instance.SelectTarget(CurrentSelectedCell);
            }
            else
            {
                Debug.Log("Target out of range for the skill.");
            }
            _isPreparingSkill = false;
            stateMachine.ChangeState(InputState.IdleState);
            return;
        }
        stateMachine.ChangeState(InputState.SelectEnemyUnitState);
    }
    
    private void HandleNoUnitClick()
    {
        if (_isPreparingSkill)
        {
            // 判断技能范围
        }
        else
        {
            var moveRangeCells = LastSelectedUnit.GetMoveRange();
            if (moveRangeCells.Contains(CurrentSelectedCell))
            {
                GridManager.Instance.Highlight(false, LastSelectedUnit.CurrentCell.Coordinate);
                var moveRange = LastSelectedUnit.GetMoveRange();
                foreach (var cell in moveRange)
                {
                    GridManager.Instance.Highlight(false, cell.Coordinate);
                }
                // ViewManager.Instance.CloseView(ViewType.UnitInfoView);
                // LastSelectedUnit.MoveTo(CurrentSelectedCell);
                ActionManager.Instance.ExecuteMoveAction(LastSelectedUnit, CurrentSelectedCell);
            }
        }
        stateMachine.ChangeState(InputState.SelectNoUnitState);
    }
    
    private void OnClickSkillView(object[] obj)
    {
        if (obj[0] is not SkillDataSO skill) return;
        _pendingSkill = skill;
        _isPreparingSkill = true;
        Debug.Log("Preparing to use skill: " + skill.skillName);
        // 显示技能范围高亮
        GridManager.Instance.ClearAllHighlights();
        var attackRange = CurrentSelectedUnit.GetAttackRange(CurrentSelectedCell);
        foreach (var cell in attackRange)
        {
            GridManager.Instance.Highlight(true, cell.Coordinate);
        }
        LastSelectedUnit = CurrentSelectedUnit;
        LastSelectedCell = CurrentSelectedCell;
    }
}