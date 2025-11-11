using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using Ally;
using Enemy;
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
    
    [Tooltip("用于指定可部署区域的Tilemap")]
    public Tilemap deploymentZoneTilemap;

    [Header("地形类型")]
    public TerrainDataSO[] terrainLibrary; //存放预制地形类型
    [Header("建筑类型")]
    public DestructibleObjectSO[] destructibleObjectLibrary; //存放预制地形类型

    [Header("随机地图生成")]
    public bool useRandomGeneration = false; //是否使用随机地图生成
    private RandomMapGenerator randomMapGenerator;

    public readonly Dictionary<Vector2Int, GridCell> _gridDict = new Dictionary<Vector2Int, GridCell>();        //网格地图

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        // 获取RandomMapGenerator组件
        randomMapGenerator = GetComponent<RandomMapGenerator>();
        if (randomMapGenerator == null && useRandomGeneration)
        {
            randomMapGenerator = gameObject.AddComponent<RandomMapGenerator>();
        }
    }

    public void InitGrid()
    {
        // 如果启用随机地图生成
        if (useRandomGeneration && randomMapGenerator != null)
        {
            randomMapGenerator.GenerateRandomMap();
            return;
        }
        
        //从 Tilemap 构建地图
        if (terrainTilemap != null)
            BuildGridFromTilemap();
        else
            GenerateGrid(); 
    }

    /// <summary>
    /// 尝试根据场景中指定名称的父对象查找并绑定 Tilemap（例如 "Grid" 或 "Grid2"）。
    /// </summary>
    /// <param name="gridRootName">父对象名称</param>
    /// <returns>是否绑定成功</returns>
    public bool TryAssignTilemapsByGridName(string gridRootName)
    {
        // 支持查找禁用对象：遍历场景根物体与全部子级（包含 inactive）
        Transform target = null;
        var scene = SceneManager.GetActiveScene();
        var roots = scene.GetRootGameObjects();
        foreach (var go in roots)
        {
            if (go.name == gridRootName)
            {
                target = go.transform;
                break;
            }
            var t = go.GetComponentsInChildren<Transform>(true);
            foreach (var child in t)
            {
                if (child.name == gridRootName)
                {
                    target = child;
                    break;
                }
            }
            if (target != null) break;
        }

        if (target == null)
        {
            Debug.LogWarning($"未找到名为 {gridRootName} 的网格父对象（包含禁用对象）");
            return false;
        }
        return AssignTilemapsUnder(target);
    }

    /// <summary>
    /// 从给定父对象的子层级中自动选择并绑定地形、障碍物、部署区 Tilemap。
    /// 命名约定：包含 Terrain/Ground/Map 视为地形；包含 Object/Obstacle 视为障碍；包含 Deploy/Deployment 视为部署区。
    /// 若未匹配到命名，则按出现顺序：第一个作为地形、第二个作为障碍。
    /// </summary>
    /// <param name="parent">父对象</param>
    /// <returns>是否至少绑定了地形 Tilemap</returns>
    public bool AssignTilemapsUnder(Transform parent)
    {
        if (parent == null) return false;
        var tilemaps = parent.GetComponentsInChildren<Tilemap>(true);
        if (tilemaps == null || tilemaps.Length == 0)
        {
            Debug.LogWarning($"在 {parent.name} 下未找到任何 Tilemap 组件");
            return false;
        }

        Tilemap terrain = null;
        Tilemap objects = null;
        Tilemap deploy = null;

        foreach (var tm in tilemaps)
        {
            var n = tm.name.ToLower();
            if (terrain == null && (n.Contains("terrain") || n.Contains("ground") || n.Contains("map") || n.Contains("地形") || n.Contains("地图")))
            {
                terrain = tm;
                continue;
            }
            if (objects == null && (n.Contains("object") || n.Contains("obstacle") || n.Contains("building") || n.Contains("buildings") || n.Contains("建筑") || n.Contains("障碍")))
            {
                objects = tm;
                continue;
            }
            if (deploy == null && (n.Contains("deploy") || n.Contains("deployment") || n.Contains("deployzone") || n.Contains("deploymentzone") || n.Contains("zone") || n.Contains("area") || n.Contains("部署") || n.Contains("可部署")))
            {
                deploy = tm;
                continue;
            }
        }

        // Fallback：按出现顺序绑定
        if (terrain == null)
        {
            terrain = tilemaps[0];
        }
        if (objects == null && tilemaps.Length > 1)
        {
            objects = tilemaps[1];
        }
        if (deploy == null && tilemaps.Length > 2)
        {
            deploy = tilemaps[2];
        }

        terrainTilemap = terrain;
        objectTilemap = objects;
        deploymentZoneTilemap = deploy;

        Debug.Log($"GridManager: 绑定 Tilemap 成功。Terrain={terrainTilemap?.name}, Objects={objectTilemap?.name}, Deploy={deploymentZoneTilemap?.name}");
        return terrainTilemap != null;
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
                var objectData = GetDestructibleObjectFromTile(objectTile);
                if (objectData != null)
                {
                    cell.DestructibleObject = new DestructibleObject(objectData.Hits, objectData.Name, coord);
                    cell.DestructibleObject.data = objectData;
                }
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
        if (cell.DestructibleObject != null || cell.ObjectOnCell != null) return false;

        var unit = Instantiate(unitPrefab, CellToWorld(coord), Quaternion.identity, transform);
        unit.PlaceAt(cell);
        if (unit.data.isEnemy)
            EnemyManager.Instance.AddAliveEnemy(unit);
        else
            AllyManager.Instance.AddAliveAlly(unit);
        
        return true;
    }
    /// <summary>
    /// 移动物体
    /// </summary>
    public bool MoveUnit(Vector2Int targetCoord,Unit unit)
    {
        var targetCell = GetCell(targetCoord);
        if (targetCell == null || targetCell.CurrentUnit != null) return false;
        if (targetCell.DestructibleObject != null || targetCell.ObjectOnCell != null) return false;

        unit.MoveTo(targetCell);
        return true;
    }

    public void Highlight(bool highlight, Vector2Int coord)
    {
        var cellPos = new Vector3Int(coord.x, coord.y, 0);
        var tile = terrainTilemap.GetTile(cellPos);
        if (tile == null) return;

        terrainTilemap.SetTileFlags(cellPos, TileFlags.None);
        terrainTilemap.SetColor(cellPos, highlight ? Color.green : Color.white);
        // EffectSystem.Instance.Play("gezi_gaoliang", cellPos);
    }

    public void ClearAllHighlights()
    {
        foreach (var pos in terrainTilemap.cellBounds.allPositionsWithin)
        {
            var cellPos = new Vector3Int(pos.x, pos.y, pos.z);
            if (!terrainTilemap.HasTile(cellPos))
                continue;

            terrainTilemap.SetTileFlags(cellPos, TileFlags.None);
            terrainTilemap.SetColor(cellPos, Color.white);
        }
    }
    
    public int GetDistance(GridCell a, GridCell b)
    {
        return Mathf.Abs(a.Coordinate.x - b.Coordinate.x) +
               Mathf.Abs(a.Coordinate.y - b.Coordinate.y);
    }
    
    /// <summary>
    /// 手动生成随机地图
    /// </summary>
    public void GenerateRandomMap()
    {
        if (randomMapGenerator == null)
        {
            randomMapGenerator = GetComponent<RandomMapGenerator>();
            if (randomMapGenerator == null)
            {
                randomMapGenerator = gameObject.AddComponent<RandomMapGenerator>();
            }
        }
        
        randomMapGenerator.GenerateRandomMap();
    }
    
    /// <summary>
    /// 切换随机地图生成模式
    /// </summary>
    public void ToggleRandomGeneration(bool enable)
    {
        useRandomGeneration = enable;
        if (enable && randomMapGenerator == null)
        {
            randomMapGenerator = gameObject.AddComponent<RandomMapGenerator>();
        }
    }
}
