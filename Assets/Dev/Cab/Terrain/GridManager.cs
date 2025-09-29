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
    public Transform cellsParent;             //存放格子对象的父节点

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
                    var view = obj.GetComponent<GridCellView>();
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
    public GridCell GetCellAtWorld(Vector3 worldPos)
    {
        int x = Mathf.FloorToInt((worldPos.x - origin.x) / cellSize);
        int y = Mathf.FloorToInt((worldPos.y - origin.y) / cellSize);
        return GetCell(x, y);
    }
}
