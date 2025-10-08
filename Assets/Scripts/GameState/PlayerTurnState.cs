using Common;
using UnityEngine;

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
        Debug.Log($"玩家回合开始 - 第{gameManager.CurrentTurn}回合");
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
        
        // 禁用玩家输入
        DisablePlayerInput();
        
        Debug.Log("玩家回合结束");
    }
    
    /// <summary>
    /// 禁用玩家输入
    /// </summary>
    /// <summary>
    /// 禁用玩家输入
    /// </summary>
    private void DisablePlayerInput()
    {
        // TODO: 禁用玩家输入处理
        Debug.Log("禁用玩家输入");
    }
    
    /// <summary>
    /// 重置玩家行动点数
    /// </summary>
    private void ResetPlayerActionPoints()
    {
        // TODO: 重置所有玩家单位的行动点数
        Debug.Log("重置玩家行动点数");
    }
    
    
    /// <summary>
    /// 完成玩家回合
    /// </summary>
    private void FinishPlayerTurn()
    {
        Debug.Log("玩家回合完成，切换到敌人回合");
        gameManager.EndCurrentTurn();
    }
    
    private void DetectGridCellClick()     
    {
        // BUG: 当鼠标同时点击到 UI 和 GridCell 上时，也会触发这个事件
        // 我想要有这些状态：点击了一个 无单位格子，点击了一个 有单位格子，点击了空白处，
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
            MessageCenter.Publish(Defines.DeselectUnitActionEvent, _activeCell);
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
                MessageCenter.Publish(Defines.DeselectUnitActionEvent, _activeCell);
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
                MessageCenter.Publish(Defines.DeselectUnitActionEvent, _activeCell);
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
        MessageCenter.Publish(Defines.DeselectUnitActionEvent, _activeCell);
        _activeCell = null;
    }

    private void HandleSelectCell(GridCell cell)
    {
        if (cell.CurrentUnit is not null)
        {
            MessageCenter.Publish(Defines.SelectUnitActionEvent, cell);
        }
        else
        {
            
        }
        _activeCell = cell;
    }
}