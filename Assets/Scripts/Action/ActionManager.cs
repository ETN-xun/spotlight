using System;
using UnityEngine;
using View;

namespace Action
{
    public class ActionManager : MonoBehaviour
    {
        // TODO: 跟 Input 集成， 负责处理所有的 Action 事件，外界调用 ActionManager 来注册和触发 Action

        public static ActionManager Instance;
        
        private GridCell _activeCell;   // 不为空，则代表当前有选中的格子

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
            // BUG: 同时点击左键和右键会出问题
            if (Input.GetMouseButtonDown(0))
            {
                DetectGridCellClick();
            }

            if (Input.GetMouseButtonDown(1))
            {
                
            }
        }

        private void DetectGridCellClick()
        {
            if (Camera.main is null)
            {
                Debug.LogError("Camera missing");
                return;
            }
            var worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            // var cellCoord = GridManager.Instance.WorldToCell(worldPoint);
            var cell = GridManager.Instance.WorldToCell(worldPoint);    // 数组可能越界
            
            OnGridCellClicked(cell);

        }

        private void OnGridCellClicked(GridCell cell)   // 考虑是否做成事件
        {
            if (cell is null) return;
            _activeCell = cell;
            // TODO;
            // 高亮
            // 有单位，打开单位信息界面
            // if (_activeCell.CurrentUnit is not null)
            //     ViewManager.Instance.OpenView(ViewType.UnitView);
        }
        
        private void OnGridCellClickCanceled()
        {
            // 取消选中格子
            if (_activeCell is null) return;
            // TODO;
            // 取消高亮
            // 有单位，关闭单位信息界面
            // if (_activeCell.CurrentUnit is not null)
            //     ViewManager.Instance.CloseView(ViewType.UnitView);
            _activeCell = null;
            
        }
    }
}