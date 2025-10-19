using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Common;
using Level;
using UnityEngine;
using View;
using View.GameViews;

/// <summary>
/// 部署状态 - 玩家放置单位的阶段
/// </summary>
public class DeploymentState : GameStateBase
{
    private UnitDataSO _unitData;
    private bool _isClickDeployUnit;
    private int _deployedUnitCount;
    private List<Unit> _totalUnits = new ();

    public DeploymentState(GameManager gameManager) : base(gameManager)
    {
    }

    public override void Enter()
    {
        base.Enter();
        MessageCenter.Subscribe(Defines.ClickDeployUnitViewEvent, OnClickDeployUnit);
        _totalUnits = LevelManager.Instance.GetCurrentLevel().playerUnits;
        ViewManager.Instance.OpenView(ViewType.DeploymentView);
    }


    public override void Update()
    {
        if (_isClickDeployUnit)
        {
            if (Input.GetMouseButtonDown(0))
            {
                var worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                var cell = GridManager.Instance.WorldToCell(worldPoint);
                if (cell is null || cell.CurrentUnit is not null) return;
                _deployedUnitCount++;
                var unit = GetUnitById(_unitData.unitID);
                GridManager.Instance.PlaceUnit(cell.Coordinate, unit);
                var view = ViewManager.Instance.GetView<UnitDeploymentView>(ViewType.UnitDeploymentView, _unitData.unitID);
                view.DisableViewClick();
                _isClickDeployUnit = false;
            }
            if (Input.GetMouseButtonDown(1))
            {
                _isClickDeployUnit = false;
            }
        }


        if (_deployedUnitCount == LevelManager.Instance.GetCurrentLevel().playerUnits.Count)
        {
            gameManager.ChangeGameState(GameState.EnemyTurn);
        }
    }

    public override void Exit()
    {
        base.Exit();
        MessageCenter.Publish(Defines.DeploymentStateEndedEvent);
        MessageCenter.Unsubscribe(Defines.ClickDeployUnitViewEvent, OnClickDeployUnit);
        ViewManager.Instance.CloseView(ViewType.DeploymentView);
    }
    
    private void OnClickDeployUnit(object[] obj)
    {
        if (obj[0] is not UnitDataSO unitData) return;
        _unitData = unitData;
        _isClickDeployUnit = true;
    }
    
    private Unit GetUnitById(string unitId)
    {
        return _totalUnits.FirstOrDefault(unit => unit.data.unitID == unitId);
    }
}