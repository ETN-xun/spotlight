using Common;
using UnityEngine;
using UnityEngine.UI;
using View;
using View.Base;
using View.GameViews;

namespace Action
{
    public class ActionHandler : MonoBehaviour
    {
        
        
        private void OnEnable()
        {
            MessageCenter.Subscribe(Defines.SelectUnitActionEvent, OnSelectUnit);
            MessageCenter.Subscribe(Defines.DeselectUnitActionEvent, OnDeselectUnit);
        }
        
        private void OnDisable()
        {
            MessageCenter.Unsubscribe(Defines.SelectUnitActionEvent, OnSelectUnit);
            MessageCenter.Unsubscribe(Defines.DeselectUnitActionEvent, OnDeselectUnit);
        }

        private void Update()
        {
            
        }

        private void OnSelectUnit(object[] obj)
        {
            if (obj[0] is not GridCell cell) return;
            // if (cell.CurrentUnit is null) return;
            cell.GridCellController.Highlight(true);
            var moveRangeCells = cell.CurrentUnit.GetMoveRange();
            foreach (var gridCell in moveRangeCells)
            {
                gridCell.GridCellController.Highlight(true);
            }
            ViewManager.Instance.OpenView(ViewType.UnitInfoView, 0, cell.CurrentUnit);
        }
        
        private void OnDeselectUnit(object[] obj)
        {
            if (obj[0] is not GridCell cell) return;
            if (cell.CurrentUnit is null) return;

            cell.GridCellController.Highlight(false);
            var moveRangeCells = cell.CurrentUnit.GetMoveRange();
            foreach (var gridCell in moveRangeCells)
            {
                gridCell.GridCellController.Highlight(false);
            }
            ViewManager.Instance.CloseView(ViewType.UnitInfoView);
        }
        
        
    }
}