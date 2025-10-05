using System.Collections;
using System.Collections.Generic;
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
                        
                        break;
                    case TerrainType.Abyss:
                        
                        break;
                    case TerrainType.Forest:
                        
                        break;
                    case TerrainType.Lava:
                        
                        break;
                    case TerrainType.Mountain:
                        
                        break;
                    case TerrainType.Spring:
                        
                        break;
                    case TerrainType.ExplosiveBarrel:

                        break;
                    case TerrainType.MagneticField:
                        
                        break;
                }
            }

            //撞到建筑
            if (nextCell.ObjectOnCell != null)
            {
                DestructibleObject obj = nextCell.ObjectOnCell;

                obj.TakeDamage(unit.data.baseDamage);

                yield break;
            }
            
            unit.MoveTo(nextCell);
            currentCell = nextCell;

            yield return new WaitForSeconds(0.05f);
        }

        onComplete?.Invoke();
    }
}
