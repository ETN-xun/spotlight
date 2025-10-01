using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance { get; private set; }

    [Header("Grid Settings")]
    public int rows = 8;            //网格长
    public int cols = 8;            //网格宽
    public float cellSize = 1f;               //格子大小
    public Vector3 origin = Vector3.zero;     //网格起点(左下角)

    [Header("Prefabs")]
    public GameObject cellPrefab;             //可视化格子预制体
    public Transform cellsParent; //存放格子对象的父节点

    [Header("Terrain Library")]
    public TerrainDataSO[] terrainLibrary; //存放预制地形类型

    private GridCell[,] grid;           //网格地图

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        GenerateGrid();
    }

    /// <summary>
    /// 生成 8x8 网格
    /// </summary>
    public void GenerateGrid()
    {
        grid = new GridCell[cols, rows];

        if (cellsParent == null)
        {
            GameObject parent = new GameObject("Cells");
            parent.transform.SetParent(transform);
            cellsParent = parent.transform;
        }

        for (int x = 0; x < cols; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                var coord = new Vector2Int(x, y);
                var cellData = new GridCell(coord);
                grid[x, y] = cellData;
                
                if (cellPrefab != null)
                {
                    Vector3 worldPos = CellToWorld(coord);
                    GameObject obj = Instantiate(cellPrefab, worldPos, Quaternion.identity, cellsParent);
                    obj.name = $"Cell_{x}_{y}";
                    
                    //绑定格子数据
                    var view = obj.GetComponent<GridCellController>();
                    if (view != null) view.Init(cellData);
                }
            }
        }
    }

    /// <summary>
    /// 按坐标获取格子
    /// </summary>
    public GridCell GetCell(Vector2Int coord)
    {
        return GetCell(coord.x, coord.y);
    }

    private GridCell GetCell(int x, int y)
    {
        if (x < 0 || x >= cols || y < 0 || y >= rows) return null;
        return grid[x, y];
    }

    /// <summary>
    /// 检查坐标是否在有效的网格范围内
    /// </summary>
    /// <param name="coord">要检查的坐标</param>
    /// <returns>如果坐标在有效范围内返回true，否则返回false</returns>
    public bool IsValidPosition(Vector2Int coord)
    {
        return coord.x >= 0 && coord.x < cols && coord.y >= 0 && coord.y < rows;
    }

    /// <summary>
    /// 检查坐标是否在有效的网格范围内
    /// </summary>
    /// <param name="x">X坐标</param>
    /// <param name="y">Y坐标</param>
    /// <returns>如果坐标在有效范围内返回true，否则返回false</returns>
    public bool IsValidPosition(int x, int y)
    {
        return x >= 0 && x < cols && y >= 0 && y < rows;
    }

    /// <summary>
    /// 格子坐标 → 世界坐标
    /// </summary>
    public Vector3 CellToWorld(Vector2Int coord)
    {
        float worldX = origin.x + coord.x * cellSize + cellSize * 0.5f;
        float worldY = origin.y + coord.y * cellSize + cellSize * 0.5f;
        return new Vector3(worldX, worldY, origin.z);
    }

    /// <summary>
    /// 世界坐标 → 格子
    /// </summary>
    public GridCell WorldToCell(Vector3 worldPos)
    {
        int x = Mathf.FloorToInt((worldPos.x - origin.x) / cellSize);
        int y = Mathf.FloorToInt((worldPos.y - origin.y) / cellSize);
        return GetCell(x, y);
    }
    /// <summary>
    /// 网格地形生成
    /// </summary>
    public void PlaceTerrain(GridCell cell, TerrainDataSO terrainData)
    {
        if (cell == null || terrainData == null) return;

        cell.TerrainData = terrainData;

        if (terrainData.terrainPrefab != null)
        {
            GameObject obj = Instantiate(terrainData.terrainPrefab, 
                CellToWorld(cell.Coordinate), 
                Quaternion.identity, 
                cellsParent);
        }
    }
    /// <summary>
    /// 地形随机放置
    /// </summary>
    public void GenerateRandomTerrain()
    {
        foreach (var c in grid)
        {
            int r = Random.Range(0, 3); //0=平原,1=山,2=森林
            TerrainDataSO t = terrainLibrary[r];
            PlaceTerrain(c, t);
        }
    }
    
    /// <summary>
    /// 放置物体
    /// </summary>
    public bool PlaceUnit(Vector2Int coord, Unit unitPrefab)
    {
        var cell = GetCell(coord);
        if (cell == null || cell.CurrentUnit != null) return false;
        
        Unit unit = Instantiate(unitPrefab, CellToWorld(coord), Quaternion.identity);
        
        unit.PlaceAt(cell);
        return true;
    }
    /// <summary>
    /// 移动物体
    /// </summary>
    public bool MoveUnit(Vector2Int targetCoord,Unit unit)
    {
        var targetCell = GetCell(targetCoord);
        if (targetCell == null || targetCell.CurrentUnit != null) return false;

        unit.MoveTo(targetCell);
        return true;
    }
}
