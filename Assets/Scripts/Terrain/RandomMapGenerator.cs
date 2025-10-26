using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// 随机地图生成器
/// 根据需求文档生成符合约束条件的随机地图
/// </summary>
public class RandomMapGenerator : MonoBehaviour
{
    [Header("地图设置")]
    [SerializeField] private int mapWidth = 15;
    [SerializeField] private int mapHeight = 8;
    
    [Header("地形配置")]
    [SerializeField] private TerrainDataSO plainTerrainData;
    [SerializeField] private TerrainDataSO corrosionTerrainData;
    [SerializeField] private TerrainDataSO bugTerrainData;
    
    [Header("建筑配置")]
    [SerializeField] private DestructibleObjectSO registerObjectData;
    [SerializeField] private DestructibleObjectSO firewallObjectData;
    
    [Header("瓦片配置")]
    [SerializeField] private TileBase plainTile;
    [SerializeField] private TileBase corrosionTile;
    [SerializeField] private TileBase bugTile;
    [SerializeField] private TileBase registerTile;
    [SerializeField] private TileBase firewallTile;
    
    [Header("生成参数")]
    [SerializeField] private int minRegisters = 3;
    [SerializeField] private int maxRegisters = 5;
    [SerializeField] private int minFirewalls = 8;
    [SerializeField] private int maxFirewalls = 12;
    [SerializeField] private float corrosionProbability = 0.15f;
    [SerializeField] private float bugProbability = 0.1f;
    
    [Header("区域限制")]
    [SerializeField] private int playerDeploymentRows = 2; // 玩家部署区行数
    [SerializeField] private int enemySpawnRows = 2; // 敌人生成区行数
    
    private GridManager gridManager;
    private Tilemap terrainTilemap;
    private Tilemap objectTilemap;
    
    // 地图数据
    private TerrainType[,] terrainMap;
    private DestructibleObjectType?[,] objectMap;
    
    // 路径验证相关
    private bool[,] walkableMap; // 可行走区域缓存
    private int firewallCount;
    private int corrosionCount;
    private int bugTileCount;
    
    // 区域定义
    private List<Vector2Int> playerDeploymentArea;
    private List<Vector2Int> enemySpawnArea;
    private List<Vector2Int> middleArea;
    
    private void Awake()
    {
        gridManager = GetComponent<GridManager>();
        if (gridManager == null)
            gridManager = FindObjectOfType<GridManager>();
            
        terrainTilemap = gridManager.terrainTilemap;
        objectTilemap = gridManager.objectTilemap;
    }
    
    /// <summary>
    /// 生成随机地图
    /// </summary>
    public void GenerateRandomMap()
    {
        Debug.Log("开始生成随机地图...");
        
        // 初始化地图数据
        InitializeMapData();
        
        // 定义区域
        DefineAreas();
        
        // 生成地图内容
        GenerateMapContent();
        
        // 应用区域限制规则
        ApplyAreaRestrictions();
        
        // 验证和修正路径连通性
        if (!ValidatePathConnectivity())
        {
            Debug.LogWarning("路径验证失败，尝试修正...");
            FixPathConnectivity();
            
            // 如果修正后仍然无法连通，重新生成
            if (!ValidatePathConnectivity())
            {
                Debug.LogWarning("路径修正失败，重新生成地图");
                GenerateRandomMap(); // 递归重新生成
                return;
            }
        }
        
        // 应用到Tilemap和GridManager
        ApplyToTilemap();
        
        Debug.Log("随机地图生成完成！");
    }
    
    /// <summary>
    /// 初始化地图数据
    /// </summary>
    private void InitializeMapData()
    {
        terrainMap = new TerrainType[mapWidth, mapHeight];
        objectMap = new DestructibleObjectType?[mapWidth, mapHeight];
        
        // 初始化为数据平原
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                terrainMap[x, y] = TerrainType.Plain;
                objectMap[x, y] = null;
            }
        }
    }
    
    /// <summary>
    /// 定义区域
    /// </summary>
    private void DefineAreas()
    {
        playerDeploymentArea = new List<Vector2Int>();
        enemySpawnArea = new List<Vector2Int>();
        middleArea = new List<Vector2Int>();
        
        // 玩家部署区（底部行）
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < playerDeploymentRows; y++)
            {
                playerDeploymentArea.Add(new Vector2Int(x, y));
            }
        }
        
        // 敌人生成区（顶部行）
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = mapHeight - enemySpawnRows; y < mapHeight; y++)
            {
                enemySpawnArea.Add(new Vector2Int(x, y));
            }
        }
        
        // 中间区域
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = playerDeploymentRows; y < mapHeight - enemySpawnRows; y++)
            {
                middleArea.Add(new Vector2Int(x, y));
            }
        }
    }
    
    /// <summary>
    /// 生成地图内容
    /// </summary>
    private void GenerateMapContent()
    {
        // 1. 放置缓存区（Register）
        PlaceRegisters();
        
        // 2. 放置防火墙
        PlaceFirewalls();
        
        // 3. 放置腐蚀区块
        PlaceCorrosionTiles();
        
        // 4. 放置Bug格子
        PlaceBugTiles();
    }
    
    /// <summary>
    /// 放置缓存区
    /// </summary>
    private void PlaceRegisters()
    {
        int registerCount = Random.Range(minRegisters, maxRegisters + 1);
        int placed = 0;
        int attempts = 0;
        int maxAttempts = 100;
        
        while (placed < registerCount && attempts < maxAttempts)
        {
            Vector2Int pos = middleArea[Random.Range(0, middleArea.Count)];
            
            if (objectMap[pos.x, pos.y] == null)
            {
                objectMap[pos.x, pos.y] = DestructibleObjectType.Register;
                placed++;
            }
            
            attempts++;
        }
        
        Debug.Log($"放置了 {placed} 个缓存区");
    }
    
    /// <summary>
    /// 放置防火墙
    /// </summary>
    private void PlaceFirewalls()
    {
        int firewallCount = Random.Range(minFirewalls, maxFirewalls + 1);
        int placed = 0;
        int attempts = 0;
        int maxAttempts = 200;
        
        while (placed < firewallCount && attempts < maxAttempts)
        {
            Vector2Int pos = middleArea[Random.Range(0, middleArea.Count)];
            
            if (objectMap[pos.x, pos.y] == null)
            {
                objectMap[pos.x, pos.y] = DestructibleObjectType.FireWall;
                placed++;
            }
            
            attempts++;
        }
        
        Debug.Log($"放置了 {placed} 个防火墙");
    }
    
    /// <summary>
    /// 放置腐蚀区块
    /// </summary>
    private void PlaceCorrosionTiles()
    {
        int placed = 0;
        
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                // 跳过玩家部署区
                if (playerDeploymentArea.Contains(new Vector2Int(x, y)))
                    continue;
                    
                // 跳过已有建筑的位置
                if (objectMap[x, y] != null)
                    continue;
                
                if (Random.value < corrosionProbability)
                {
                    terrainMap[x, y] = TerrainType.CorrosionTile;
                    placed++;
                }
            }
        }
        
        Debug.Log($"放置了 {placed} 个腐蚀区块");
    }
    
    /// <summary>
    /// 放置Bug格子
    /// </summary>
    private void PlaceBugTiles()
    {
        int placed = 0;
        
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                // 跳过玩家部署区和敌人生成区
                if (playerDeploymentArea.Contains(new Vector2Int(x, y)) || 
                    enemySpawnArea.Contains(new Vector2Int(x, y)))
                    continue;
                    
                // 跳过已有建筑的位置
                if (objectMap[x, y] != null)
                    continue;
                    
                // 跳过腐蚀区块
                if (terrainMap[x, y] == TerrainType.CorrosionTile)
                    continue;
                
                if (Random.value < bugProbability)
                {
                    terrainMap[x, y] = TerrainType.BugTile;
                    placed++;
                }
            }
        }
        
        Debug.Log($"放置了 {placed} 个Bug格子");
    }
    

    
    /// <summary>
    /// 将生成的地图应用到Tilemap
    /// </summary>
    private void ApplyToTilemap()
    {
        // 清空现有瓦片
        terrainTilemap.SetTilesBlock(terrainTilemap.cellBounds, new TileBase[terrainTilemap.cellBounds.size.x * terrainTilemap.cellBounds.size.y * terrainTilemap.cellBounds.size.z]);
        objectTilemap.SetTilesBlock(objectTilemap.cellBounds, new TileBase[objectTilemap.cellBounds.size.x * objectTilemap.cellBounds.size.y * objectTilemap.cellBounds.size.z]);
        
        // 清空现有网格数据
        gridManager._gridDict.Clear();
        
        // 应用地形瓦片和网格数据
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                Vector3Int pos = new Vector3Int(x, y, 0);
                Vector2Int coord = new Vector2Int(x, y);
                
                // 创建网格单元
                var cell = new GridCell(coord);
                
                // 设置地形数据和瓦片
                var terrainType = terrainMap[x, y];
                cell.TerrainData = GetTerrainData(terrainType);
                TileBase terrainTile = GetTerrainTile(terrainType);
                if (terrainTile != null)
                {
                    terrainTilemap.SetTile(pos, terrainTile);
                }
                
                // 设置建筑数据和瓦片
                if (objectMap[x, y] != null)
                {
                    var objectType = objectMap[x, y].Value;
                    var objectData = GetObjectData(objectType);
                    if (objectData != null)
                    {
                        cell.DestructibleObject = new DestructibleObject(objectData.Hits, objectData.Name, coord);
                        cell.DestructibleObject.data = objectData;
                    }
                    
                    TileBase objectTile = GetObjectTile(objectType);
                    if (objectTile != null)
                    {
                        objectTilemap.SetTile(pos, objectTile);
                    }
                }
                
                // 添加到网格管理器
                gridManager._gridDict[coord] = cell;
            }
        }
        
        // 更新GridManager的尺寸
        gridManager.cols = mapWidth;
        gridManager.rows = mapHeight;
    }
    
    /// <summary>
    /// 获取地形瓦片
    /// </summary>
    private TileBase GetTerrainTile(TerrainType terrainType)
    {
        switch (terrainType)
        {
            case TerrainType.Plain:
                return plainTile;
            case TerrainType.CorrosionTile:
                return corrosionTile;
            case TerrainType.BugTile:
                return bugTile;
            default:
                return plainTile;
        }
    }
    
    /// <summary>
    /// 获取建筑瓦片
    /// </summary>
    private TileBase GetObjectTile(DestructibleObjectType objectType)
    {
        switch (objectType)
        {
            case DestructibleObjectType.Register:
                return registerTile;
            case DestructibleObjectType.FireWall:
                return firewallTile;
            default:
                return null;
        }
    }
    
    /// <summary>
    /// 获取地形数据
    /// </summary>
    public TerrainDataSO GetTerrainData(TerrainType terrainType)
    {
        switch (terrainType)
        {
            case TerrainType.Plain:
                return plainTerrainData;
            case TerrainType.CorrosionTile:
                return corrosionTerrainData;
            case TerrainType.BugTile:
                return bugTerrainData;
            default:
                return plainTerrainData;
        }
    }
    
    /// <summary>
    /// 获取建筑数据
    /// </summary>
    public DestructibleObjectSO GetObjectData(DestructibleObjectType objectType)
    {
        switch (objectType)
        {
            case DestructibleObjectType.Register:
                return registerObjectData;
            case DestructibleObjectType.FireWall:
                return firewallObjectData;
            default:
                return null;
        }
    }
    
    /// <summary>
    /// 验证路径连通性
    /// 确保敌人生成区到玩家部署区有可行走的路径
    /// </summary>
    private bool ValidatePathConnectivity()
    {
        UpdateWalkableMap();
        
        // 从敌人生成区的任意一个可行走位置开始
        Vector2Int startPos = GetValidEnemySpawnPosition();
        if (startPos.x == -1) return false; // 没有有效的敌人生成位置
        
        // 检查是否能到达玩家部署区的任意位置
        return CanReachPlayerArea(startPos);
    }
    
    /// <summary>
    /// 更新可行走地图缓存
    /// </summary>
    private void UpdateWalkableMap()
    {
        walkableMap = new bool[mapWidth, mapHeight];
        
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                // 检查是否可行走：无防火墙且地形可行走
                bool hasFirewall = objectMap[x, y] == DestructibleObjectType.FireWall;
                bool terrainWalkable = terrainMap[x, y] != TerrainType.BugTile; // 虫洞不可行走
                
                walkableMap[x, y] = !hasFirewall && terrainWalkable;
            }
        }
    }
    
    /// <summary>
    /// 获取有效的敌人生成位置
    /// </summary>
    private Vector2Int GetValidEnemySpawnPosition()
    {
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = mapHeight - enemySpawnRows; y < mapHeight; y++)
            {
                if (walkableMap[x, y])
                {
                    return new Vector2Int(x, y);
                }
            }
        }
        return new Vector2Int(-1, -1); // 没有找到有效位置
    }
    
    /// <summary>
    /// 检查是否能从起始位置到达玩家部署区
    /// 使用广度优先搜索(BFS)
    /// </summary>
    private bool CanReachPlayerArea(Vector2Int startPos)
    {
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        bool[,] visited = new bool[mapWidth, mapHeight];
        
        queue.Enqueue(startPos);
        visited[startPos.x, startPos.y] = true;
        
        Vector2Int[] directions = {
            new Vector2Int(0, 1),   // 上
            new Vector2Int(0, -1),  // 下
            new Vector2Int(1, 0),   // 右
            new Vector2Int(-1, 0)   // 左
        };
        
        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();
            
            // 检查是否到达玩家部署区
            if (current.y < playerDeploymentRows)
            {
                return true;
            }
            
            // 探索相邻位置
            foreach (var dir in directions)
            {
                Vector2Int next = current + dir;
                
                if (IsValidPosition(next) && !visited[next.x, next.y] && walkableMap[next.x, next.y])
                {
                    visited[next.x, next.y] = true;
                    queue.Enqueue(next);
                }
            }
        }
        
        return false; // 无法到达玩家部署区
    }
    
    /// <summary>
    /// 检查位置是否有效
    /// </summary>
    private bool IsValidPosition(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < mapWidth && pos.y >= 0 && pos.y < mapHeight;
    }
    
    /// <summary>
    /// 修正地图以确保路径连通性
    /// </summary>
    private void FixPathConnectivity()
    {
        int maxAttempts = 50;
        int attempts = 0;
        
        while (!ValidatePathConnectivity() && attempts < maxAttempts)
        {
            attempts++;
            
            // 尝试移除一些阻挡路径的防火墙
            RemoveBlockingFirewalls();
            
            // 如果防火墙数量太少，重新生成
            if (GetFirewallCount() < minFirewalls)
            {
                RegenerateFirewalls();
            }
        }
        
        if (attempts >= maxAttempts)
        {
            Debug.LogWarning("无法在最大尝试次数内修正路径连通性，可能存在地图设计问题");
        }
    }
    
    /// <summary>
    /// 移除阻挡路径的防火墙
    /// </summary>
    private void RemoveBlockingFirewalls()
    {
        // 找到关键路径上的防火墙并移除
        List<Vector2Int> firewallsToRemove = new List<Vector2Int>();
        
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                if (objectMap[x, y] == DestructibleObjectType.FireWall)
                {
                    // 临时移除这个防火墙，检查是否改善连通性
                    objectMap[x, y] = null;
                    
                    if (ValidatePathConnectivity())
                    {
                        firewallsToRemove.Add(new Vector2Int(x, y));
                        break; // 只移除一个就够了
                    }
                    else
                    {
                        // 恢复防火墙
                        objectMap[x, y] = DestructibleObjectType.FireWall;
                    }
                }
            }
        }
    }
    
    /// <summary>
    /// 重新生成防火墙
    /// </summary>
    private void RegenerateFirewalls()
    {
        // 清除现有防火墙
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                if (objectMap[x, y] == DestructibleObjectType.FireWall)
                {
                    objectMap[x, y] = null;
                }
            }
        }
        
        // 重新生成防火墙
        PlaceFirewalls();
    }
    
    /// <summary>
    /// 获取当前防火墙数量
    /// </summary>
    private int GetFirewallCount()
    {
        int count = 0;
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                if (objectMap[x, y] == DestructibleObjectType.FireWall)
                {
                    count++;
                }
            }
        }
        return count;
     }
     
     /// <summary>
     /// 应用区域限制规则
     /// </summary>
     private void ApplyAreaRestrictions()
     {
         // 1. 清理玩家部署区的限制
         ApplyPlayerDeploymentRestrictions();
         
         // 2. 清理敌人生成区的限制
         ApplyEnemySpawnRestrictions();
         
         // 3. 确保中间区域的合理性
         ApplyMiddleAreaRestrictions();
     }
     
     /// <summary>
     /// 应用玩家部署区限制规则
     /// </summary>
     private void ApplyPlayerDeploymentRestrictions()
     {
         foreach (var pos in playerDeploymentArea)
         {
             // 玩家部署区不能有防火墙（玩家需要部署单位）
             if (objectMap[pos.x, pos.y] == DestructibleObjectType.FireWall)
             {
                 objectMap[pos.x, pos.y] = null;
                 firewallCount--;
             }
             
             // 玩家部署区不能有缓存区（Register）
             if (objectMap[pos.x, pos.y] == DestructibleObjectType.Register)
             {
                 objectMap[pos.x, pos.y] = null;
             }
             
             // 玩家部署区可以有腐蚀地块，但不能有虫洞（虫洞不可行走）
             if (terrainMap[pos.x, pos.y] == TerrainType.BugTile)
             {
                 terrainMap[pos.x, pos.y] = TerrainType.Plain;
                 bugTileCount--;
             }
         }
     }
     
     /// <summary>
     /// 应用敌人生成区限制规则
     /// </summary>
     private void ApplyEnemySpawnRestrictions()
     {
         foreach (var pos in enemySpawnArea)
         {
             // 敌人生成区不能有防火墙（敌人需要生成和移动）
             if (objectMap[pos.x, pos.y] == DestructibleObjectType.FireWall)
             {
                 objectMap[pos.x, pos.y] = null;
                 firewallCount--;
             }
             
             // 敌人生成区不能有缓存区
             if (objectMap[pos.x, pos.y] == DestructibleObjectType.Register)
             {
                 objectMap[pos.x, pos.y] = null;
             }
             
             // 敌人生成区不能有虫洞（敌人需要能够移动）
             if (terrainMap[pos.x, pos.y] == TerrainType.BugTile)
             {
                 terrainMap[pos.x, pos.y] = TerrainType.Plain;
                 bugTileCount--;
             }
             
             // 敌人生成区可以有腐蚀地块（增加挑战性）
         }
     }
     
     /// <summary>
     /// 应用中间区域限制规则
     /// </summary>
     private void ApplyMiddleAreaRestrictions()
     {
         // 确保至少有一条从敌人生成区到玩家部署区的路径
         // 这个在路径验证中已经处理
         
         // 缓存区只能放置在中间区域
         List<Vector2Int> invalidRegisters = new List<Vector2Int>();
         
         for (int x = 0; x < mapWidth; x++)
         {
             for (int y = 0; y < mapHeight; y++)
             {
                 if (objectMap[x, y] == DestructibleObjectType.Register)
                 {
                     Vector2Int pos = new Vector2Int(x, y);
                     if (!middleArea.Contains(pos))
                     {
                         invalidRegisters.Add(pos);
                     }
                 }
             }
         }
         
         // 移除不在中间区域的缓存区
         foreach (var pos in invalidRegisters)
         {
             objectMap[pos.x, pos.y] = null;
         }
         
         // 如果缓存区数量不足，在中间区域重新放置
         int currentRegisters = GetRegisterCount();
         if (currentRegisters < minRegisters)
         {
             PlaceAdditionalRegisters(minRegisters - currentRegisters);
         }
     }
     
     /// <summary>
     /// 获取当前缓存区数量
     /// </summary>
     private int GetRegisterCount()
     {
         int count = 0;
         for (int x = 0; x < mapWidth; x++)
         {
             for (int y = 0; y < mapHeight; y++)
             {
                 if (objectMap[x, y] == DestructibleObjectType.Register)
                 {
                     count++;
                 }
             }
         }
         return count;
     }
     
     /// <summary>
     /// 在中间区域放置额外的缓存区
     /// </summary>
     private void PlaceAdditionalRegisters(int count)
     {
         List<Vector2Int> availablePositions = new List<Vector2Int>();
         
         // 找到中间区域的可用位置
         foreach (var pos in middleArea)
         {
             if (objectMap[pos.x, pos.y] == null && terrainMap[pos.x, pos.y] != TerrainType.BugTile)
             {
                 availablePositions.Add(pos);
             }
         }
         
         // 随机放置缓存区
         for (int i = 0; i < count && availablePositions.Count > 0; i++)
         {
             int randomIndex = Random.Range(0, availablePositions.Count);
             Vector2Int pos = availablePositions[randomIndex];
             objectMap[pos.x, pos.y] = DestructibleObjectType.Register;
             availablePositions.RemoveAt(randomIndex);
         }
     }
     
     /// <summary>
     /// 检查位置是否在指定区域内
     /// </summary>
     private bool IsInPlayerDeploymentArea(Vector2Int pos)
     {
         return pos.y < playerDeploymentRows;
     }
     
     /// <summary>
     /// 检查位置是否在敌人生成区内
     /// </summary>
     private bool IsInEnemySpawnArea(Vector2Int pos)
     {
         return pos.y >= mapHeight - enemySpawnRows;
     }
     
     /// <summary>
     /// 检查位置是否在中间区域内
     /// </summary>
     private bool IsInMiddleArea(Vector2Int pos)
     {
         return pos.y >= playerDeploymentRows && pos.y < mapHeight - enemySpawnRows;
     }
}