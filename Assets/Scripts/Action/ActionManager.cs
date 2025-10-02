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
            var worldPoint = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
            // var cellCoord = GridManager.Instance.WorldToCell(worldPoint);
            var cell = GridManager.Instance.WorldToCell(worldPoint);
            if (_activeCell is not null && _activeCell != cell)
                MessageCenter.Publish(Defines.GridCellClickCanceledActionEvent, _activeCell);
            _activeCell = cell;
            MessageCenter.Publish(Defines.GridCellClickActionEvent, _activeCell);
        }
        
        private void DetectGridCellClickCanceled()
        {
            if (_activeCell is null) return;
            MessageCenter.Publish(Defines.GridCellClickCanceledActionEvent, _activeCell);
            _activeCell = null;
        }
    }
}