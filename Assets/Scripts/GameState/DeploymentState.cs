using System.Collections;
using Common;
using UnityEngine;
using View;
using View.GameViews;

/// <summary>
/// 部署状态 - 玩家放置单位的阶段
/// </summary>
public class DeploymentState : GameStateBase
{
    private bool _isClickDeployUnit;
    private UnitDataSO _unitData;
    
    public DeploymentState(GameManager gameManager) : base(gameManager)
    {
    }

    public override void Enter()
    {
        base.Enter();
        MessageCenter.Subscribe(Defines.ClickDeployUnitEvent, OnClickDeployUnit);
        
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
                if (cell is null) return;
                var unit = _unitData.unitPrefab.GetComponent<Unit>();
                GridManager.Instance.PlaceUnit(cell.Coordinate, unit);
                var view = ViewManager.Instance.GetView<UnitDeploymentView>(ViewType.UnitDeploymentView, int.Parse(_unitData.unitID));
                // if (view is null) return;
                view.DisableViewClick();
                _isClickDeployUnit = false;
            }
            if (Input.GetMouseButtonDown(1))
            {
                _isClickDeployUnit = false;
            }
        }
        

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("部署阶段结束，进入敌人回合");
            gameManager.ChangeGameState(GameState.EnemyTurn);
        }
    }

    public override void Exit()
    {
        base.Exit();
        MessageCenter.Unsubscribe(Defines.ClickDeployUnitEvent, OnClickDeployUnit);
        ViewManager.Instance.CloseView(ViewType.DeploymentView);
    }
    
    private void OnClickDeployUnit(object[] obj)
    {
        if (obj[0] is not UnitDataSO unitData) return;
        _unitData = unitData;
        _isClickDeployUnit = true;
    }
}