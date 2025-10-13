using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance { get; private set; }

    [Header("网格设置")]
    public int rows = 8;            //网格长
    public int cols = 8;            //网格宽
    public float cellSize = 1f;               //格子大小
    public Vector3 origin = Vector3.zero;     //网格起点(左下角)

    [Header("瓦片地图")]
    public Tilemap terrainTilemap; //绘制地形
    public Tilemap objectTilemap; //绘制建筑或障碍物

    [Header("地形类型")]
    public TerrainDataSO[] terrainLibrary; //存放预制地形类型
    [Header("建筑类型")]
    public DestructibleObjectSO[] destructibleObjectLibrary; //存放预制地形类型

    public readonly Dictionary<Vector2Int, GridCell> _gridDict = new Dictionary<Vector2Int, GridCell>();        //网格地图

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        //从 Tilemap 构建地图
        if (terrainTilemap != null)
            BuildGridFromTilemap();
        else
            GenerateGrid(); 
    }
    
    /// <summary>
    /// 生成地形
    /// </summary>
    private void BuildGridFromTilemap()
    {
        _gridDict.Clear();

        foreach (var pos in terrainTilemap.cellBounds.allPositionsWithin)
        {
            var tilePos = new Vector3Int(pos.x, pos.y, pos.z);
            if (!terrainTilemap.HasTile(tilePos))
                continue;

            var coord = new Vector2Int(pos.x, pos.y);
            var cell = new GridCell(coord);

            // 绑定地形数据
            var terrainTile = terrainTilemap.GetTile(tilePos);
            cell.TerrainData = GetTerrainDataFromTile(terrainTile);

            // 绑定建筑数据
            if (objectTilemap != null && objectTilemap.HasTile(tilePos))
            {
                var objectTile = objectTilemap.GetTile(tilePos);
                cell.DestructibleObject.data = GetDestructibleObjectFromTile(objectTile);
                cell.DestructibleObject.coordinate = coord;
            }

            _gridDict[coord] = cell;
        }

        cols = terrainTilemap.size.x;
        rows = terrainTilemap.size.y;
    }
    /// <summary>
    /// 生成 8x8 网格
    /// </summary>
    private void GenerateGrid()
    {
        _gridDict.Clear();

        for (int x = 0; x < cols; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                Vector2Int coord = new Vector2Int(x, y);
                GridCell cell = new GridCell(coord);
                _gridDict[coord] = cell;
            }
        }
    }
    /// <summary>
    /// 按照Tile名称查找TerrainDataSO
    /// </summary>
    /// <param name="tile"></param>
    /// <returns></returns>
    private TerrainDataSO GetTerrainDataFromTile(TileBase tile)
    {
        if (tile == null) return null;
        string tileName = tile.name.ToLower();

        foreach (var t in terrainLibrary)
        {
            if (tileName.Contains(t.terrainName.ToLower()))
                return t;
        }

        return null; 
    }
    /// <summary>
    /// 从Tile名称查找建筑物
    /// </summary>
    /// <param name="tile"></param>
    /// <returns></returns>
    private DestructibleObjectSO GetDestructibleObjectFromTile(TileBase tile)
    {
        if (tile == null) return null;
        string tileName = tile.name.ToLower();

        foreach (var t in destructibleObjectLibrary)
        {
            if (tileName.Contains(t.Name.ToLower()))
                return t;
        }
        return null;
    }
    /// <summary>
    /// 按坐标获取格子
    /// </summary>
    public GridCell GetCell(Vector2Int coord)
    {
        _gridDict.TryGetValue(coord, out var cell);
        return cell;
    }

    /// <summary>
    /// 检查位置是否合法
    /// </summary>
    /// <param name="coord"></param>
    /// <returns></returns>
    public bool IsValidPosition(Vector2Int coord) => _gridDict.ContainsKey(coord);

    /// <summary>
    /// 格子坐标 → 世界坐标
    /// </summary>
    public Vector3 CellToWorld(Vector2Int coord)
    {
        if (terrainTilemap != null)
            return terrainTilemap.CellToWorld((Vector3Int)coord) + terrainTilemap.tileAnchor;

        return default;
    }

    /// <summary>
    /// 世界坐标 → 格子
    /// </summary>
    public GridCell WorldToCell(Vector3 worldPos)
    {
        if (terrainTilemap != null)
        {
            Vector3Int cellPos = terrainTilemap.WorldToCell(worldPos);
            return GetCell((Vector2Int)cellPos);
        }

        return null;
    }

    /// <summary>
    /// 放置物体
    /// </summary>
    public bool PlaceUnit(Vector2Int coord, Unit unitPrefab)
    {
        var cell = GetCell(coord);
        if (cell == null || cell.CurrentUnit != null) return false;

        Unit unit = Instantiate(unitPrefab, CellToWorld(coord), Quaternion.identity, transform);

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
