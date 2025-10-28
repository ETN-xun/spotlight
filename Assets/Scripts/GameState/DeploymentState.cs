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
    private List<Unit> _allyUnits = new ();
    private List<Vector2Int> _availableDeployPositions = new ();

    public DeploymentState(GameManager gameManager) : base(gameManager)
    {
    }

    public override void Enter()
    {
        base.Enter();
        MessageCenter.Subscribe(Defines.ClickDeployUnitViewEvent, OnClickDeployUnit);
        ShowDeploymentGrid();
        _allyUnits = LevelManager.Instance.GetCurrentLevel().allyUnits;
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
                
                var cellPosition = cell.Coordinate;
                // 移除有问题的坐标转换调用
                // Utils.Coordinate.Transform(ref cellPosition);
                
                if (!_availableDeployPositions.Contains(cellPosition)) return;
                _deployedUnitCount++;
                var unit = GetUnitById(_unitData.unitID);
                GridManager.Instance.PlaceUnit(cellPosition, unit);
                var view = ViewManager.Instance.GetView<UnitDeploymentView>(ViewType.UnitDeploymentView, _unitData.unitID);
                view.DisableViewClick();
                _isClickDeployUnit = false;
            }
            if (Input.GetMouseButtonDown(1))
            {
                _isClickDeployUnit = false;
            }
        }


        // 检查是否所有单位都已部署完毕
        if (IsAllUnitsDeployed())
        {
            gameManager.ChangeGameState(GameState.EnemyTurn);
        }
    }

    public override void Exit()
    {
        base.Exit();
        MessageCenter.Publish(Defines.DeploymentStateEndedEvent);
        MessageCenter.Unsubscribe(Defines.ClickDeployUnitViewEvent, OnClickDeployUnit);
        GridManager.Instance.ClearAllHighlights();
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
        return _allyUnits.FirstOrDefault(unit => unit.data.unitID == unitId);
    }
    
    /// <summary>
    /// 检查是否所有我方单位都已部署完毕
    /// </summary>
    /// <returns>如果所有单位都已部署则返回true，否则返回false</returns>
    private bool IsAllUnitsDeployed()
    {
        var totalUnits = LevelManager.Instance.GetCurrentLevel().allyUnits.Count;
        var deployedUnits = _deployedUnitCount;
        
        Debug.Log($"部署进度: {deployedUnits}/{totalUnits}");
        
        return deployedUnits >= totalUnits;
    }
    
    private void ShowDeploymentGrid()
    {
        var deploymentArea = LevelManager.Instance.GetCurrentLevel().allyDeployPositions;
        foreach (var pos in deploymentArea)
        {
            var coord = pos;
            // 移除有问题的坐标转换调用，直接使用原始坐标
            Utils.Coordinate.Transform(ref coord);
            _availableDeployPositions.Add(coord);
            GridManager.Instance.Highlight(true, coord);
            Debug.Log("高亮显示部署格子: " + coord);
        }
    }
}