using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 挂载预制体上，用来处理格子数据以及点击逻辑
/// </summary>
public class GridCellController : MonoBehaviour
{
    public GridCell Data { get; private set; }

    public void Init(GridCell cell)
    {
        cell.GridCellController = this;
        Data = cell;
        transform.position = GridManager.Instance.CellToWorld(cell.Coordinate);
        Refresh();
        
    }

    public void Highlight(bool isHighlight)
    {
        // TEMP;
        GetComponent<SpriteRenderer>().color = isHighlight ? Color.blue : Color.white;
    }

    /// <summary>
    /// 刷新格子的外观
    /// </summary>
    private void Refresh()
    {

        
    }
}
