using Common;
using UnityEngine;
using View;

/// <summary>
/// 玩家回合状态 - 玩家控制单位行动的阶段
/// </summary>
public class PlayerTurnState : GameStateBase
{
    
    private Camera  _mainCamera;
    private GridCell _activeCell;
    public PlayerTurnState(GameManager gameManager) : base(gameManager)
    {
    }
    
    public override void Enter()
    {
        base.Enter();
        _mainCamera = Camera.main;
        MessageCenter.Subscribe(Defines.ClickSkillEvent, OnClickSkill);
    }

    public override void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
            DetectGridCellClick();
        }

        if (Input.GetMouseButtonDown(1))
        {
            DetectGridCellClickCanceled();
        }
        
        // 检查是否结束回合
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            // 回车键结束回合
            FinishPlayerTurn();
        }
    }

    public override void Exit()
    {
        base.Exit();
        
    }


    private void FinishPlayerTurn()
    {
        gameManager.EndCurrentTurn();
    }

    private void DetectGridCellClick()     
    {
        // BUG: 当鼠标同时点击到 UI 和 GridCell 上时，也会触发这个事件
        var worldPoint = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
        var cell = GridManager.Instance.WorldToCell(worldPoint);
        if (cell is null) return;

        // 情况 1：当前没有激活格子
        if (_activeCell is null)
        {
            HandleSelectCell(cell);
            return;
        }

        // 情况 2：点击的是同一个格子 -> 取消
        if (_activeCell == cell)
        {
            DeselectUnit(_activeCell);
            _activeCell = null;
            return;
        }

        // 情况 3：激活格子有单位
        if (_activeCell.CurrentUnit is not null)
        {
            var moveRangeCells = _activeCell.CurrentUnit.GetMoveRange();

            if (moveRangeCells.Contains(cell))
            {
                // 移动
                DeselectUnit(_activeCell);
                _activeCell.CurrentUnit.MoveTo(cell);
                _activeCell = null;
            }
            // else if (cell.CurrentUnit is not null)
            // {
            //     MessageCenter.Publish(Defines.DeselectUnitActionEvent, _activeCell);
            //     HandleSelectCell(cell);
            // }
            else
            {
                // 切换选择
                DeselectUnit(_activeCell);
                HandleSelectCell(cell);
            }
            return;
        }

        // 情况 4：激活格子无单位
        HandleSelectCell(cell);
    }


    private void DetectGridCellClickCanceled()
    {
        if (_activeCell is null) return;
        DeselectUnit(_activeCell);
        _activeCell = null;
    }

    private void HandleSelectCell(GridCell cell)
    {
        if (cell.CurrentUnit is not null)
        {
            SelectUnit(cell);
        }
        else
        {
            
        }
        _activeCell = cell;
    }

    private void SelectUnit(GridCell cell)
    {
        // if (cell.CurrentUnit is null) return;
        // if (cell.GridCellController is null)
        // {
        //     Debug.Log("Error: GridCellController is null");
        //     return;
        // }
        // cell.GridCellController.Highlight(true);
        // var moveRangeCells = cell.CurrentUnit.GetMoveRange();
        // foreach (var gridCell in moveRangeCells)
        // {
        //     gridCell.GridCellController.Highlight(true);
        // }
        ViewManager.Instance.OpenView(ViewType.UnitInfoView, "", cell.CurrentUnit);
        ViewManager.Instance.OpenView(ViewType.SkillSelectView, "", cell.CurrentUnit);
    }

    private void DeselectUnit(GridCell cell)
    {
        if (cell.CurrentUnit is null) return;

        // cell.GridCellController.Highlight(false);
        // var moveRangeCells = cell.CurrentUnit.GetMoveRange();
        // foreach (var gridCell in moveRangeCells)
        // {
        //     gridCell.GridCellController.Highlight(false);
        // }
        ViewManager.Instance.CloseView(ViewType.UnitInfoView);
        ViewManager.Instance.CloseView(ViewType.SkillSelectView);
    }

    private void OnClickSkill(object[] obj)
    {
        if (obj[0] is not SkillDataSO skillData) return;
        
        
        ViewManager.Instance.CloseView(ViewType.SkillSelectView);
        ViewManager.Instance.CloseView(ViewType.UnitInfoView);
    }
}