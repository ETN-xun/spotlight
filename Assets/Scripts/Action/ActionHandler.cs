using System;
using Common;
using UnityEngine;
using View;

namespace Action
{
    public class ActionHandler : MonoBehaviour
    {
        private void OnEnable()
        {
            MessageCenter.Subscribe(Defines.GridCellClickActionEvent, OnGridCellClicked);
            MessageCenter.Subscribe(Defines.GridCellClickCanceledActionEvent, OnGridCellClickCanceled);
        }
        
        private void OnDisable()
        {
            MessageCenter.Unsubscribe(Defines.GridCellClickActionEvent, OnGridCellClicked);
            MessageCenter.Unsubscribe(Defines.GridCellClickCanceledActionEvent, OnGridCellClickCanceled);
        }

        private void OnGridCellClicked(object[] obj)
        {
            Debug.Log("Action : OnGridCellClicked");
            if (obj[0] is not GridCell cell) return;
            if (cell.CurrentUnit is null) return;
            
            cell.GridCellController.Highlight(true);
            ViewManager.Instance.OpenView(ViewType.UnitView, cell.CurrentUnit);
        }
        
        private void OnGridCellClickCanceled(object[] obj)
        {
            if (obj[0] is not GridCell cell) return;
            if (cell.CurrentUnit is null) return;

            cell.GridCellController.Highlight(false);
            ViewManager.Instance.CloseView(ViewType.UnitView);
        }
    }
    
}