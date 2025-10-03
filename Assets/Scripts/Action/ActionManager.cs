using Common;
using UnityEngine;

namespace Action
{
    public class ActionManager : MonoBehaviour
    {
        private Camera  _mainCamera;
        private GridCell _activeCell;

        private void Awake()
        {
            _mainCamera = Camera.main;
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                DetectGridCellClick();
            }

            if (Input.GetMouseButtonDown(1))
            {
                DetectGridCellClickCanceled();
            }
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
}