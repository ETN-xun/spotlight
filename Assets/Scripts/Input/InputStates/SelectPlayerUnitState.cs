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
    // 移形换影：双目标选择流程状态
    private bool _isPreparingPositionSwap;
    private GridCell _firstSwapCell;
    
    private SkillDataSO _pendingSkill;
    
    public SelectPlayerUnitState(InputStateMachine stateMachine) : base(stateMachine)
    {
    }
    
    public override void Enter()
    {
        MessageCenter.Subscribe(Defines.ClickSkillViewEvent, OnClickSkillView);
        _isPreparingSkill = false;
        _isPreparingPositionSwap = false;
        _firstSwapCell = null;
        if (CurrentSelectedUnit is null) return;
        LastSelectedUnit = CurrentSelectedUnit;
        GridManager.Instance.Highlight(true, CurrentSelectedUnit.CurrentCell.Coordinate);
        var moveRange = CurrentSelectedUnit.GetMoveRange();
        foreach (var cell in moveRange)
        {
            GridManager.Instance.Highlight(true, cell.Coordinate);
        }
        ViewManager.Instance.OpenView(ViewType.SkillSelectView, "", CurrentSelectedUnit);
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
        ViewManager.Instance.CloseView(ViewType.SkillSelectView);
        // ViewManager.Instance.CloseView(ViewType.TerrainInfoView);
        
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
                if (_isPreparingPositionSwap)
                {
                    HandleClickUnitForPositionSwap();
                }
                else if (_isPreparingSkill)
                {
                    // 技能准备中：支持对友方单位施放（如堆栈护盾）
                    if (LastSelectedUnit is null)
                    {
                        Debug.Log("No unit selected to use the skill.");
                        return;
                    }

                    var targetRange = LastSelectedUnit.GetSkillTargetRange(LastSelectedCell, _pendingSkill);
                    if (targetRange.Count == 0)
                    {
                        Debug.Log("No valid targets in range for the skill.");
                        _isPreparingSkill = false;
                        stateMachine.ChangeState(InputState.IdleState);
                        return;
                    }

                    if (targetRange.Contains(CurrentSelectedCell))
                    {
                        if (!ActionManager.EnergySystem.TrySpendEnergy(_pendingSkill.energyCost))
                        {
                            Debug.Log("Not enough energy to use the skill.");
                            _isPreparingSkill = false;
                            stateMachine.ChangeState(InputState.IdleState);
                            return;
                        }
                        SkillSystem.Instance.StartSkill(LastSelectedUnit, _pendingSkill);
                        SkillSystem.Instance.SelectTarget(CurrentSelectedCell);
                        var animationName = Utilities.SkillNameToAnimationName(_pendingSkill.skillName);
                        LastSelectedUnit.PlayAnimation(animationName, false);
                    }
                    else
                    {
                        Debug.Log("Target out of range for the skill.");
                    }
                    _isPreparingSkill = false;
                    stateMachine.ChangeState(InputState.IdleState);
                }
                else
                {
                    stateMachine.ChangeState(InputState.SelectPlayerUnitState);
                }
                break;
            case InputType.ClickEnemyUnit:
                if (_isPreparingPositionSwap)
                {
                    HandleClickUnitForPositionSwap();
                }
                else
                {
                    HandleClickEnemyUnit();
                }
                break;
            case InputType.ClickNoUnit:
                HandleNoUnitClick();
                break;
            case InputType.CancelClick:
                // 取消双选流程
                if (_isPreparingPositionSwap)
                {
                    GridManager.Instance.ClearAllHighlights();
                    _isPreparingPositionSwap = false;
                    _firstSwapCell = null;
                }
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
            var targetRange = LastSelectedUnit.GetSkillTargetRange(LastSelectedCell, _pendingSkill);
            if (targetRange.Count == 0)
            {
                Debug.Log("No valid targets in range for the skill.");
                return;
            }
            if (targetRange.Contains(CurrentSelectedCell))
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
                var animationName = Utilities.SkillNameToAnimationName(_pendingSkill.skillName);
                LastSelectedUnit.PlayAnimation(animationName, false);
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
            // 在技能准备中，支持对空格子的技能（如生成类技能）
            if (LastSelectedUnit is null)
            {
                Debug.Log("No unit selected to use the skill.");
                _isPreparingSkill = false;
                stateMachine.ChangeState(InputState.IdleState);
                return;
            }

            var targetRange = LastSelectedUnit.GetSkillTargetRange(LastSelectedCell, _pendingSkill);
            if (targetRange.Count == 0)
            {
                Debug.Log("No valid targets in range for the skill.");
                _isPreparingSkill = false;
                stateMachine.ChangeState(InputState.IdleState);
                return;
            }

            if (targetRange.Contains(CurrentSelectedCell))
            {
                if (!ActionManager.EnergySystem.TrySpendEnergy(_pendingSkill.energyCost))
                {
                    Debug.Log("Not enough energy to use the skill.");
                    _isPreparingSkill = false;
                    stateMachine.ChangeState(InputState.IdleState);
                    return;
                }
                SkillSystem.Instance.StartSkill(LastSelectedUnit, _pendingSkill);
                SkillSystem.Instance.SelectTarget(CurrentSelectedCell);
                var animationName = Utilities.SkillNameToAnimationName(_pendingSkill.skillName);
                LastSelectedUnit.PlayAnimation(animationName, false);
            }
            else
            {
                Debug.Log("Target out of range for the skill.");
            }

            _isPreparingSkill = false;
            stateMachine.ChangeState(InputState.IdleState);
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
        
        // 检查是否为闪回位移技能
        if (skill.skillName.Contains("闪回") || skill.skillName.Contains("Flashback"))
        {
            // 闪回位移技能直接执行，不需要选择目标
            ExecuteFlashbackSkill(skill);
            return;
        }
        
        // 检查是否为移形换影技能（双目标选择：先攻距内单位，再任意单位）
        if (skill.skillID == "position_swap_01" || skill.skillName.Contains("移形换影") || skill.skillName.Contains("PositionSwap"))
        {
            PreparePositionSwap(skill);
            return;
        }
        
        _isPreparingSkill = true;
        Debug.Log("Preparing to use skill: " + skill.skillName);
        // 显示技能范围高亮
        GridManager.Instance.ClearAllHighlights();
        var targetRange = CurrentSelectedUnit.GetSkillTargetRange(CurrentSelectedCell, skill);
        foreach (var cell in targetRange)
        {
            GridManager.Instance.Highlight(true, cell.Coordinate);
        }
        LastSelectedUnit = CurrentSelectedUnit;
        LastSelectedCell = CurrentSelectedCell;
    }

    /// <summary>
    /// 进入移形换影的第一阶段：选择攻距内的第一个单位
    /// </summary>
    private void PreparePositionSwap(SkillDataSO skill)
    {
        _isPreparingPositionSwap = true;
        _firstSwapCell = null;
        Debug.Log("准备使用移形换影技能（双目标选择）");

        GridManager.Instance.ClearAllHighlights();
        LastSelectedUnit = CurrentSelectedUnit;
        LastSelectedCell = CurrentSelectedCell;

        // 高亮：施法者AttackRange范围内所有“有单位”的格子（友敌皆可）
        var cellsInRange = GetCellsWithUnitInAttackRange(LastSelectedUnit, LastSelectedCell);
        // 禁止选择施法者本人
        cellsInRange.RemoveAll(cell => cell == LastSelectedUnit.CurrentCell);
        foreach (var cell in cellsInRange)
        {
            GridManager.Instance.Highlight(true, cell.Coordinate);
        }
    }

    /// <summary>
    /// 在移形换影流程中处理单位点击（两次选择）
    /// </summary>
    private void HandleClickUnitForPositionSwap()
    {
        var clickedCell = CurrentSelectedCell;
        if (clickedCell == null || clickedCell.CurrentUnit == null)
        {
            Debug.Log("请选择一个有单位的格子");
            return;
        }

        // 第一次选择：必须在施法者攻击范围内（曼哈顿距离）
        if (_firstSwapCell == null)
        {
            int range = LastSelectedUnit.data.attackRange;
            var dist = Mathf.Abs(clickedCell.Coordinate.x - LastSelectedCell.Coordinate.x) +
                       Mathf.Abs(clickedCell.Coordinate.y - LastSelectedCell.Coordinate.y);
            if (dist > range)
            {
                Debug.Log("第一个目标超出施法者攻击范围");
                return;
            }

            // 禁止选择施法者本人
            if (clickedCell.CurrentUnit == LastSelectedUnit)
            {
                Debug.Log("不能选择施法者本人作为第一个目标");
                return;
            }

            // 记录第一个目标并进入第二次选择
            _firstSwapCell = clickedCell;
            GridManager.Instance.ClearAllHighlights();

            // 高亮所有可作为第二目标的单位（距离不限，排除第一个目标）
            foreach (var kvp in GridManager.Instance._gridDict)
            {
                var cell = kvp.Value;
                if (cell.CurrentUnit != null && cell != _firstSwapCell && cell != LastSelectedUnit.CurrentCell)
                {
                    GridManager.Instance.Highlight(true, cell.Coordinate);
                }
            }

            Debug.Log("已选择第一个目标，请选择第二个任意单位");
            return;
        }

        // 第二次选择：任意单位，且与第一个不同
        if (clickedCell == _firstSwapCell)
        {
            Debug.Log("第二个目标不能与第一个相同");
            return;
        }

        // 禁止施法者作为第二个目标
        if (clickedCell.CurrentUnit == LastSelectedUnit)
        {
            Debug.Log("不能选择施法者本人作为第二个目标");
            return;
        }

        // 能量检测
        if (!ActionManager.EnergySystem.TrySpendEnergy(_pendingSkill.energyCost))
        {
            Debug.Log("能量不足，无法使用移形换影");
            _isPreparingPositionSwap = false;
            _firstSwapCell = null;
            GridManager.Instance.ClearAllHighlights();
            stateMachine.ChangeState(InputState.IdleState);
            return;
        }

        // 执行位置互换
        var swapSkill = new PositionSwapSkill(_pendingSkill, LastSelectedUnit);
        swapSkill.ExecuteSwap(_firstSwapCell, clickedCell, GridManager.Instance);

        // 播放施法者动画
        var animationName = Utilities.SkillNameToAnimationName(_pendingSkill.skillName);
        LastSelectedUnit.PlayAnimation(animationName, false);

        // 清理状态并返回空闲
        _isPreparingPositionSwap = false;
        _firstSwapCell = null;
        GridManager.Instance.ClearAllHighlights();
        stateMachine.ChangeState(InputState.IdleState);
    }

    /// <summary>
    /// 计算施法者AttackRange范围内所有“有单位”的格子（友敌皆可）
    /// </summary>
    private System.Collections.Generic.List<GridCell> GetCellsWithUnitInAttackRange(Unit unit, GridCell center)
    {
        var result = new System.Collections.Generic.List<GridCell>();
        int range = unit.data.attackRange;
        var centerCoord = center.Coordinate;
        for (int dx = -range; dx <= range; dx++)
        {
            for (int dy = -range; dy <= range; dy++)
            {
                int dist = Mathf.Abs(dx) + Mathf.Abs(dy);
                if (dist > range) continue;
                var pos = centerCoord + new Vector2Int(dx, dy);
                var cell = GridManager.Instance.GetCell(pos);
                if (cell != null && cell.CurrentUnit != null)
                {
                    result.Add(cell);
                }
            }
        }
        return result;
    }
    
    /// <summary>
    /// 执行闪回位移技能
    /// </summary>
    /// <param name="skill">闪回位移技能数据</param>
    private void ExecuteFlashbackSkill(SkillDataSO skill)
    {
        if (CurrentSelectedUnit == null)
        {
            Debug.Log("没有选中的单位");
            return;
        }
        
        // 先进行完整校验：存在记录、未过期、目标格未被占用
        if (!FlashbackDisplacementSkill.CanExecuteFlashback(CurrentSelectedUnit))
        {
            Debug.Log($"{CurrentSelectedUnit.data.unitName} 当前不可执行闪回（无记录/过期/目标被占用）");
            return;
        }

        // 校验通过后再检测能量是否充足（不在失败时扣能量）
        if (!ActionManager.EnergySystem.TrySpendEnergy(skill.energyCost))
        {
            Debug.Log("能量不足，无法使用闪回位移技能");
            return;
        }
        
        Debug.Log($"{CurrentSelectedUnit.data.unitName} 使用闪回位移技能");
        
        var beforeCell = CurrentSelectedUnit.CurrentCell;
        // 直接创建并执行闪回位移技能（不需要通过SkillSystem的目标验证）
        FlashbackDisplacementSkill flashbackSkill = new FlashbackDisplacementSkill(skill, CurrentSelectedUnit);
        flashbackSkill.Execute(CurrentSelectedUnit.CurrentCell, GridManager.Instance);
        
        // 如果执行后位置未发生变化，视为失败，返还能量
        if (CurrentSelectedUnit.CurrentCell == beforeCell)
        {
            ActionManager.EnergySystem.IncreaseEnergy(skill.energyCost);
            Debug.Log("闪回位移失败，已返还能量");
            stateMachine.ChangeState(InputState.IdleState);
            return;
        }
        
        // 播放动画
        var animationName = Utilities.SkillNameToAnimationName(skill.skillName);
        CurrentSelectedUnit.PlayAnimation(animationName, false);
        
        // 技能执行完毕，返回空闲状态
        stateMachine.ChangeState(InputState.IdleState);
    }
}
