using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleObject : IObjectOnCell
{
    public UnitDataSO data;
    public Vector2Int coordinate;
    public bool isDestroyed = false;
    
    // IObjectOnCell 接口实现
    public string Name => data?.unitName ?? "Unknown";
    public Vector2Int Coordinate => coordinate;
    public int Hits => data?.Hits ?? 0;
    
    public DestructibleObject(int hits, string name, Vector2Int coord)
    {
        data.canDestroy = true;
        data.Hits = hits;
        data.unitName = name;
        coordinate = coord;
    }

    public void TakeDamage()
    {
        TakeDamage(1); // 默认伤害为1
    }
    
    public void TakeDamage(int damage)
    {
        if (isDestroyed) return;
        data.Hits -= damage;
        if (data.Hits <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        if (isDestroyed) return;
        isDestroyed = true;

        //从逻辑格子中移除
        GridCell cell = GridManager.Instance.GetCell(coordinate);
        if (cell != null)
        {
            cell.ObjectOnCell = null;
        }

        //从Tilemap中移除对应Tile
        if (GridManager.Instance.objectTilemap != null)
        {
            Vector3Int tilePos = new Vector3Int(coordinate.x, coordinate.y, 0);
            GridManager.Instance.objectTilemap.SetTile(tilePos, null);
        }
    }
    
    public bool CanUnitPassThrough(Unit unit)
    {
        // 可破坏物体通常不允许单位穿过
        return false;
    }
    
    public bool CanTakeDamage()
    {
        // 可破坏物体可以受到伤害
        return !isDestroyed;
    }

}
