using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MovementSystem : MonoBehaviour
{
    public static MovementSystem Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
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

    private IEnumerator MoveUnitCoroutine(Unit unit, Vector2Int direction, int distance, System.Action onComplete)
    {
        if (unit == null || unit.CurrentCell == null) yield break;

        GridManager grid = GridManager.Instance;
        GridCell currentCell = unit.CurrentCell;
        
        // 记录移动前的位置，用于闪回位移
        GridCell originalCell = currentCell;

        for (int step = 0; step < distance; step++)
        {
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

                unit.TakeDamage(unit.data.baseDamage);
                other.TakeDamage(unit.data.baseDamage);

                yield break;
            }

            //撞到地形
            if (nextCell.TerrainData != null)
            {
                var terrain = nextCell.TerrainData;
                TerrainType type = terrain.terrainType;
                switch (type)
                {
                    case TerrainType.Plain:
                        //轻微像素效果
                        
                        break;
                    case TerrainType.CorrosionTile:
                        //造成一点伤害
                        unit.TakeDamage(1);
                        //造成异常效果
                        
                        break;
                    case TerrainType.BugTile:
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
                        //造成异常效果

                        break;
                }
            }

            //撞到建筑
            if (nextCell.ObjectOnCell != null)
            {
                var DestructibleObject = nextCell.DestructibleObject;
                DestructibleObjectType type = DestructibleObject.data.Type;
                switch (type)
                {
                    case DestructibleObjectType.Register:
                        //每回合回复两点能量
                        unit.data.RecoverEnergy += 2;
                        //建筑激活，可以被攻击
                        DestructibleObject.data.Hits = 1;
                        DestructibleObject.data.canDestroy = true;
                        break;
                }
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
    public List<GridCell> FindPath(GridCell start, GridCell end)
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
            // 取fScore最小的cell
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
                if (closedSet.Contains(neighbor) || !IsCellWalkable(neighbor))
                    continue;
                int tentativeG = gScore[current] + 1;
                if (!openSet.Contains(neighbor))
                    openSet.Add(neighbor);
                else if (tentativeG >= (gScore.ContainsKey(neighbor) ? gScore[neighbor] : int.MaxValue))
                    continue;
                cameFrom[neighbor] = current;
                gScore[neighbor] = tentativeG;
                fScore[neighbor] = tentativeG + Heuristic(neighbor, end);
            }
        }
        return null; // 无法到达
    }

    private int Heuristic(GridCell a, GridCell b)
    {
        // 曼哈顿距离
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
            Vector2Int pos = cell.Coordinate + dir;
            if (GridManager.Instance.IsValidPosition(pos))
            {
                var neighbor = GridManager.Instance.GetCell(pos);
                if (neighbor != null)
                    neighbors.Add(neighbor);
            }
        }
        return neighbors;
    }

    private bool IsCellWalkable(GridCell cell)
    {
        return cell.CurrentUnit == null && cell.DestructibleObject == null;
    }

    /// <summary>
    /// 基于A*路径的单位移动
    /// </summary>
    public void MoveUnit(Unit unit, GridCell targetCell, System.Action onComplete = null)
    {
        if (unit == null || unit.CurrentCell == null || targetCell == null) return;
        var path = FindPath(unit.CurrentCell, targetCell);
        if (path == null || path.Count < 2) return;
        StartCoroutine(MoveUnitByPathCoroutine(unit, path, onComplete));
    }

    private IEnumerator MoveUnitByPathCoroutine(Unit unit, List<GridCell> path, System.Action onComplete)
    {
        // 记录移动前的位置
        GridCell originalCell = unit.CurrentCell;
        
        for (int i = 1; i < path.Count; i++)
        {
            var nextCell = path[i];
            if (!IsCellWalkable(nextCell)) yield break;
            unit.MoveTo(nextCell);      // TODO: 替换为 MoveUnit
            yield return new WaitForSeconds(0.05f);
        }
        
        // 如果单位实际发生了移动，记录位移历史
        if (originalCell != unit.CurrentCell)
        {
            int currentTurn = GameManager.Instance != null ? GameManager.Instance.CurrentTurn : 1;
            FlashbackDisplacementSkill.RecordMovement(unit, originalCell, currentTurn);
        }
        
        onComplete?.Invoke();
    }
}
