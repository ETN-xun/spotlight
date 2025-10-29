using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MovementSystem : MonoBehaviour
{
    public static MovementSystem Instance { get; private set; }
    public static SkillDataSO StatusAbnormalSkill;
    private bool canSkip;

    private void Awake()
    {
        Instance = this;
    }

    public bool CanEnterCell(GridCell cell)
    {
        if (cell == null) return false;
        if (cell.CurrentUnit != null) return false;
        if (cell.DestructibleObject != null) return false;
        if (cell.ObjectOnCell != null) return false;
        return true;
    }

    public bool TryMoveToCell(Unit unit, GridCell targetCell)
    {
        if (unit == null) return false;
        if (!CanEnterCell(targetCell)) return false;
        unit.MoveTo(targetCell);
        return true;
    }

    /// <summary>
    /// 执行逐格位移
    /// </summary>
    /// <param name="unit">要移动的单位</param>
    /// <param name="direction">位移方向（单位向量）</param>
    /// <param name="distance">位移格数</param>
    /// <param name="onComplete">移动完成后的回调方法（默认为null）</param>
    public void MoveUnit(Unit unit, Vector2Int direction, int distance, System.Action onComplete = null)
    {
        StartCoroutine(MoveUnitCoroutine(unit, direction, distance, onComplete));
    }
    
    private IEnumerator MoveUnitCoroutine(Unit unit, GridCell nextCell)
    {
        if (unit?.CurrentCell is null || nextCell is null) yield break;

        var currentCell = unit.CurrentCell;
        
        // 记录移动前的位置，用于闪回位移
        var originalCell = currentCell;

        // 如何进行攻击再议
        // 撞到单位
        if (nextCell.CurrentUnit is not null)
        {
            var other = nextCell.CurrentUnit;
            // 只有不同阵营的单位碰撞才会造成伤害
            if (unit.data.isEnemy != other.data.isEnemy)
            {
                unit.TakeDamage(unit.data.baseDamage);
                other.TakeDamage(unit.data.baseDamage);
                Debug.Log("撞到敌方单位，双方各自受到" + unit.data.baseDamage + "点伤害");
            }
            else
            {
                Debug.Log("撞到同阵营单位，无法移动但不造成伤害");
            }
            yield break;
        }
        //撞到地形
        if (nextCell.TerrainData is not null)
        {
            var terrain = nextCell.TerrainData;
            var type = terrain.terrainType;
            switch (type)
            {
                case TerrainType.Plain:
                    //轻微像素效果
                        
                    break;
                case TerrainType.CorrosionTile:
                    //造成一点伤害
                    unit.TakeDamage(1);
                    //造成异常效果
                    StatusAbnormal(unit,nextCell);    
                    break;
                case TerrainType.BugTile:
                    //造成异常效果
                    StatusAbnormal(unit,nextCell);    
                    //根据角色类型交换攻击力与生命
                    if(unit.data.isEnemy)
                    {
                        unit.data.baseDamage = unit.data.baseDamage + unit.data.maxHP;
                        unit.data.maxHP = unit.data.baseDamage - unit.data.maxHP;
                        unit.data.baseDamage = unit.data.baseDamage - unit.data.maxHP;
                    }
                    else
                    {
                        switch (unit.data.unitName)
                        { 
                            case "Shadow"://角色为影
                                Exchange(ref unit.data.maxHP,ref unit.data.skills.FirstOrDefault(skill => skill.skillName == "BreakpointExecutionSkill")!.baseDamage);//交换断点斩杀技能数值
                                break;
                            case "Rock"://角色为石
                                Exchange(ref unit.data.maxHP,ref unit.data.skills.FirstOrDefault(skill => skill.skillName == "SpawnSkill")!.baseDamage);//交换地形投放技能数值
                                break;
                            case "Zero"://角色为零
                                Exchange(ref unit.data.maxHP,ref unit.data.skills.FirstOrDefault(skill => skill.skillName == "ForcedMigrationSkill")!.baseDamage);//交换强制迁移技能
                                break;
                        }
                    }
                    break;
            }
            
        }

        //撞到建筑
        if (nextCell.DestructibleObject is not null)
        {
            var destructibleObject = nextCell.DestructibleObject;
            DestructibleObjectType type = destructibleObject.data.Type;
            switch (type)
            {
                case DestructibleObjectType.Register:
                    if(!destructibleObject.data.isActive)
                    {
                        //每回合回复两点能量
                        unit.data.RecoverEnergy += 2;
                        //建筑激活，可以被攻击
                        destructibleObject.data.Hits = 1;
                        destructibleObject.data.canDestroy = true;
                        destructibleObject.data.isActive = true;
                    }
                    else
                    {
                        yield break;
                    }
                    break;
                case DestructibleObjectType.FireWall:
                    if (destructibleObject.data.isActive)
                        yield break;
                    break;
                
            }
        }
        RegisterActivation(nextCell,unit);
        
        yield return new WaitForSeconds(1.0f);
        
        // 检查单位是否仍然存在（可能在等待期间被销毁）
        if (unit == null || unit.gameObject == null)
        {
            Debug.Log("单位在移动过程中被销毁，停止移动");
            yield break;
        }
        
        unit.MoveTo(nextCell);      // TODO: 需要更改的移动方式
    }

    private void StatusAbnormal(Unit unit, GridCell nowCell)
    {
        SkillSystem.Instance.StartSkill(unit, StatusAbnormalSkill);
        SkillSystem.Instance.SelectTarget(nowCell);
    }
    private IEnumerator MoveUnitCoroutine(Unit unit, Vector2Int direction, int distance, System.Action onComplete)
    {
        if (unit == null || unit.CurrentCell == null) yield break;

        GridManager grid = GridManager.Instance;
        GridCell currentCell = unit.CurrentCell;
        
        // 记录移动前的位置，用于闪回位移
        GridCell originalCell = currentCell;

        for (int step = 0; step < distance; step++)
        {
            // 在每次循环开始时检查单位是否仍然存在
            if (unit == null || unit.gameObject == null)
            {
                Debug.Log("单位在移动过程中被销毁，停止移动");
                yield break;
            }
            
            Vector2Int nextPos = currentCell.Coordinate + direction;
            
            if (!grid.IsValidPosition(nextPos))
            {
                yield break;
            }

            GridCell nextCell = grid.GetCell(nextPos);
            

            // 撞到单位
            if (nextCell.CurrentUnit != null)
            {
                Unit other = nextCell.CurrentUnit;

                // 只有不同阵营的单位碰撞才会造成伤害
                if (unit.data.isEnemy != other.data.isEnemy)
                {
                    unit.TakeDamage(unit.data.baseDamage);
                    other.TakeDamage(unit.data.baseDamage);
                    Debug.Log("撞到敌方单位，双方各自受到" + unit.data.baseDamage + "点伤害");
                }
                else
                {
                    Debug.Log("撞到同阵营单位，无法移动但不造成伤害");
                }

                yield break;
            }

            if (nextCell.TerrainData is not null)
            {
                var terrain = nextCell.TerrainData;
                var type = terrain.terrainType;
                switch (type)
                {
                    case TerrainType.Plain:
                        //轻微像素效果
                        
                        break;
                    case TerrainType.CorrosionTile:
                        //造成一点伤害
                        unit.TakeDamage(1);
                        //造成异常效果
                        StatusAbnormal(unit,nextCell);    
                        break;
                    case TerrainType.BugTile:
                        //造成异常效果
                        StatusAbnormal(unit,nextCell);    
                        //根据角色类型交换攻击力与生命
                        if(unit.data.isEnemy)
                        {
                            unit.data.baseDamage = unit.data.baseDamage + unit.data.maxHP;
                            unit.data.maxHP = unit.data.baseDamage - unit.data.maxHP;
                            unit.data.baseDamage = unit.data.baseDamage - unit.data.maxHP;
                        }
                        else
                        {
                            switch (unit.data.unitName)
                            { 
                                case "Shadow"://角色为影
                                    Exchange(ref unit.data.maxHP,ref unit.data.skills.FirstOrDefault(skill => skill.skillID== "breakpoint_execution_01")!.baseDamage);//交换断点斩杀技能数值
                                    break;
                                case "Rock"://角色为石
                                    Exchange(ref unit.data.maxHP,ref unit.data.skills.FirstOrDefault(skill => skill.skillID == "terrain_deployment_01")!.baseDamage);//交换地形投放技能数值
                                    break;
                                case "Zero"://角色为零
                                    Exchange(ref unit.data.maxHP,ref unit.data.skills.FirstOrDefault(skill => skill.skillID == "forced_migration_01")!.baseDamage);//交换强制迁移技能
                                    break;
                            }
                        }
                        break;
                }
            
            }

            //撞到建筑
            if (nextCell.DestructibleObject is not null)
            {
                var destructibleObject = nextCell.DestructibleObject;
                DestructibleObjectType type = destructibleObject.data.Type;
                switch (type)
                {
                    case DestructibleObjectType.Register:
                        if(!destructibleObject.data.isActive)
                        {
                            //每回合回复两点能量
                            unit.data.RecoverEnergy += 2;
                            //建筑激活，可以被攻击
                            destructibleObject.data.Hits = 1;
                            destructibleObject.data.canDestroy = true;
                            destructibleObject.data.isActive = true;
                        }
                        else
                        {
                            yield break;
                        }
                        break;
                    case DestructibleObjectType.FireWall:
                        if (destructibleObject.data.isActive)
                            yield break;
                        break;
                }
                yield break;
            }
            RegisterActivation(nextCell,unit);
            
            // 检查单位是否仍然存在（可能在处理过程中被销毁）
            if (unit == null || unit.gameObject == null)
            {
                Debug.Log("单位在移动过程中被销毁，停止移动");
                yield break;
            }
            
            unit.MoveTo(nextCell);
            currentCell = nextCell;

            yield return new WaitForSeconds(0.05f);
        }

        // 如果单位实际发生了移动，记录位移历史
        if (originalCell != unit.CurrentCell)
        {
            int currentTurn = GameManager.Instance != null ? GameManager.Instance.CurrentTurn : 1;
            Debug.Log($"MovementSystem (MoveUnit): 为 {unit.data.unitName} 记录移动，从 ({originalCell.Coordinate.x}, {originalCell.Coordinate.y}) 到 ({unit.CurrentCell.Coordinate.x}, {unit.CurrentCell.Coordinate.y})");
            FlashbackDisplacementSkill.RecordMovement(unit, originalCell, currentTurn);
        }

        onComplete?.Invoke();
    }
    
    private void Exchange(ref int Num1, ref int Num2)
    {
        Num1 = Num1 + Num2;
        Num2 = Num1 - Num2;
        Num1 = Num1 - Num2;
    }
    /// <summary>
    /// 基于A*算法查找路径
    /// </summary>
    public List<GridCell> FindPathForEnemy(GridCell start, GridCell end)
    {
        var openSet = new List<GridCell>();
        var closedSet = new HashSet<GridCell>();
        var cameFrom = new Dictionary<GridCell, GridCell>();
        var gScore = new Dictionary<GridCell, int>();
        var fScore = new Dictionary<GridCell, int>();
        
        openSet.Add(start);
        gScore[start] = 0;
        fScore[start] = Heuristic(start, end);

        while (openSet.Count > 0)
        {
            var current = openSet[0];
            foreach (var cell in openSet.Where(cell => fScore.ContainsKey(cell) && fScore[cell] < fScore[current]))
            {
                current = cell;
            }
            if (current == end)
                return ReconstructPath(cameFrom, current);

            openSet.Remove(current);
            closedSet.Add(current);

            foreach (var neighbor in GetNeighbors(current))
            {
                if (closedSet.Contains(neighbor) || !neighbor.IsWalkableForEnemy())
                    continue;
                var tentativeG = gScore[current] + 1;
                if (!openSet.Contains(neighbor))
                    openSet.Add(neighbor);
                else if (tentativeG >= gScore.GetValueOrDefault(neighbor, int.MaxValue))
                    continue;
                cameFrom[neighbor] = current;
                gScore[neighbor] = tentativeG;
                fScore[neighbor] = tentativeG + Heuristic(neighbor, end);
            }
        }
        return null; 
    }

    private int Heuristic(GridCell a, GridCell b)
    {
        return Mathf.Abs(a.Coordinate.x - b.Coordinate.x) + Mathf.Abs(a.Coordinate.y - b.Coordinate.y);
    }

    private List<GridCell> ReconstructPath(Dictionary<GridCell, GridCell> cameFrom, GridCell current)
    {
        var path = new List<GridCell> { current };
        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            path.Insert(0, current);
        }
        return path;
    }

    private List<GridCell> GetNeighbors(GridCell cell)
    {
        var neighbors = new List<GridCell>();
        Vector2Int[] dirs = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
        foreach (var dir in dirs)
        {
            var pos = cell.Coordinate + dir;
            if (!GridManager.Instance.IsValidPosition(pos)) continue;
            var neighbor = GridManager.Instance.GetCell(pos);
            if (neighbor != null)
                neighbors.Add(neighbor);
        }
        return neighbors;
    }

    public IEnumerator MoveUnitByPathCoroutine(Unit unit, List<GridCell> path)
    {
        // 路径应该包含起点，循环从索引 1 开始跳过起点
        if (path == null || path.Count < 2)
            yield break;
            
        for (int i = 1; i < path.Count; i++)
        {
            var nextCell = path[i];
            yield return MoveUnitCoroutine(unit, nextCell);
        }
    }
    //获取角色周围的格子
    private List<GridCell> GetNeighbors(Vector2Int coord)
    {
        List<GridCell> neighbors = new List<GridCell>();

        Vector2Int[] directions = new Vector2Int[]
        {
            new Vector2Int(0, 1),   // 上
            new Vector2Int(0, -1),  // 下
            new Vector2Int(-1, 0),  // 左
            new Vector2Int(1, 0)    // 右
        };

        foreach (var dir in directions)
        {
            Vector2Int neighborCoord = coord + dir;
            neighbors.Add(GridManager.Instance.GetCell(neighborCoord));
        }

        return neighbors;
    }
    //缓存区激活判断
    private void RegisterActivation(GridCell gridCell, Unit unit)
    {
        List<GridCell> neighbors = GetNeighbors(gridCell);
        for (int i = 0; i < neighbors.Count; i++)
        {
            GridCell neighbor = neighbors[i];
            if (neighbor.DestructibleObject != null)
            {
                var DestructibleObject = neighbor.DestructibleObject;
                if(DestructibleObject.data.Type == DestructibleObjectType.Register && !DestructibleObject.data.isActive)
                {
                    //每回合回复两点能量
                    unit.data.RecoverEnergy += 2;
                    //建筑激活，可以被攻击
                    DestructibleObject.data.Hits = 1;
                    DestructibleObject.data.canDestroy = true;
                    DestructibleObject.data.isActive = true;
                }
            }
        }
    }
}

