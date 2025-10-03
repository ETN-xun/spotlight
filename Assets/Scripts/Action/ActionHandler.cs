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
            switch (GameManager.Instance.CurrentGameState)
            {
                case GameState.Deployment:
                    HandleDeploymentState();
                    break;
                case GameState.EnemyTurn:
                    HandleEnemyTurnState();
                    break;
                case GameState.PlayerTurn:
                    HandlePlayerTurnState();
                    break;
                case GameState.GameOver:
                    HandleGameOverState();
                    break;
                default: 
                    break;
            }
        }
        
        private void HandleDeploymentState()
        {
            
        }
        
        private void HandleEnemyTurnState()
        {
            
        }

        private void HandlePlayerTurnState()
        {
            
        }
        
        private void HandleGameOverState()
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
            ViewManager.Instance.OpenView(ViewType.UnitView, cell.CurrentUnit);
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
            ViewManager.Instance.CloseView(ViewType.UnitView);
        }
    }
    
}