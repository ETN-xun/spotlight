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

    /// <summary>
    /// 刷新格子的外观
    /// </summary>
    private void Refresh()
    {

        /*if (Data.CurrentUnit != null)
        {
            //有单位时显示
            
        }
        else if (Data.ObjectOnCell != null)
        {   
            //有建筑物时显示逻辑
            
        }
        else
        {
            //无建筑物时显示逻辑
            
        }*/
    }
    /// <summary>
    /// 鼠标点击逻辑
    /// </summary>
    private void OnMouseDown()
    {
        if (Data != null)
        {
            Debug.Log($"Clicked cell: {Data.Coordinate}");
            //鼠标点击逻辑
            
        }
    }
}
