using System;
using System.Collections.Generic;
using Common;
using UnityEngine;
using View;

namespace Action
{
    public class ActionManager : MonoBehaviour
    {
        // TODO: 跟 Input 集成， 负责处理所有的 Action 事件，外界调用 ActionManager 来注册和触发 InputAction
        // TODO: 优化代码结构

        public static ActionManager Instance;    
        
        private GridCell _activeCell;   // 当前选中的格子，不为空，则代表当前有选中的格子

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                Instance = this;
                DontDestroyOnLoad(this.gameObject);
            }
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                DetectGridCellClick();
            }

            if (Input.GetMouseButtonDown(1))
            {
                OnGridCellClickCanceled();
            }
        }

        private void DetectGridCellClick()      // 注意一下：点击可能会触及 UI 方面，尽量把所有点击都处理了
        {
            if (Camera.main is null)
            {
                Debug.LogError("Camera missing");
                return;
            }
            var worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            // var cellCoord = GridManager.Instance.WorldToCell(worldPoint);
            var cell = GridManager.Instance.WorldToCell(worldPoint);    
            OnGridCellClicked(cell);

        }

        private void OnGridCellClicked(GridCell cell)   // 考虑是否做成事件
        {
            if (cell is null) return;
            if (_activeCell is not null) 
                OnGridCellClickCanceled();
            
            _activeCell = cell;
            if (_activeCell.CurrentUnit is null) return;
            cell.GridCellController.Highlight(true);
            ViewManager.Instance.OpenView(ViewType.UnitView, cell.CurrentUnit);
        }
        
        private void OnGridCellClickCanceled()
        {
            if (_activeCell is null) return;
            if (_activeCell.CurrentUnit is not null) 
                ViewManager.Instance.CloseView(ViewType.UnitView);
            _activeCell.GridCellController.Highlight(false);
            _activeCell = null;
            
        }
    }
}