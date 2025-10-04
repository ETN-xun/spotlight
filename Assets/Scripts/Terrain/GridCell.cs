using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridCell
{
    public Vector2Int Coordinate;            // 格子坐标 (x,y)
    public TerrainDataSO TerrainData;        // 地形数据
    public Unit CurrentUnit;                 // 当前格子上的单位 (可空)
    public GridCellController GridCellController;
    public DestructibleObject ObjectOnCell;

    public GridCell(Vector2Int coord)
    {
        Coordinate = coord;
        TerrainData = null;
        CurrentUnit = null;
        ObjectOnCell = null;
    }
}
