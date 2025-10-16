using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }
    
    private Unit _selectedUnit;
    private GridCell _selectedCell;
    private Camera _mainCamera;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
        
        _mainCamera = Camera.main;
    }

    public Unit GetSelectedUnit()
    {
        return _selectedUnit;
    }

    public GridCell GetSelectedCell()
    {
        return _selectedCell;
    }

    public InputType DetectInputType()
    {
        if (EventSystem.current.IsPointerOverGameObject() || !Input.GetMouseButtonDown(0))
            return InputType.NoClick;
        var worldPoint = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
        var cell = GridManager.Instance.WorldToCell(worldPoint);
        if (cell is null) return InputType.NoClick;
        _selectedCell = cell;
        if (cell.CurrentUnit is null) return InputType.ClickNoUnit;
        var unit = cell.CurrentUnit;
        _selectedUnit = unit;
        return unit.data.isEnemy ? InputType.ClickEnemyUnit : InputType.ClickPlayerUnit;
    }
}