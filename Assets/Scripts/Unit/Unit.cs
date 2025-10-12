using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public UnitDataSO data; // 静态数据
    public int currentHP { get; private set; }
    public GridCell CurrentCell { get; private set; }

    private void Start()
    {
        InitFromData();
    }

    public void InitFromData()
    {
        if (data == null) return;
        currentHP = data.maxHP;
        GetComponent<SpriteRenderer>().sprite = data.unitSprite;
    }

    public void PlaceAt(GridCell cell)
    {
        if (cell == null || cell.CurrentUnit != null) return;
        if (CurrentCell != null) CurrentCell.CurrentUnit = null;
        
        CurrentCell = cell;
        cell.CurrentUnit = this;
        
        transform.position = GridManager.Instance.CellToWorld(cell.Coordinate);
    }

    public void MoveTo(GridCell targetCell)
    {
        if (targetCell == null || targetCell.CurrentUnit != null) return;
        PlaceAt(targetCell);
    }

    public void TakeDamage(int dmg)
    {
        currentHP -= dmg;
        if (currentHP <= 0) Die();
    }

    private void Die()
    {
        if (CurrentCell != null) CurrentCell.CurrentUnit = null;
        Destroy(gameObject);
    }
    
    public List<GridCell> GetMoveRange()
    {
        List<GridCell> result = new List<GridCell>();
        int range = data.moveRange;

        for (int dx = -range; dx <= range; dx++)
        {
            for (int dy = -range; dy <= range; dy++)
            {
                int dist = Mathf.Abs(dx) + Mathf.Abs(dy); 
                if (dist <= range)
                {
                    var target = GridManager.Instance.GetCell(CurrentCell.Coordinate + new Vector2Int(dx, dy));
                    if (target != null && target.CurrentUnit == null)
                    {
                        result.Add(target);
                    }
                }
            }
        }

        return result;
    }
}
