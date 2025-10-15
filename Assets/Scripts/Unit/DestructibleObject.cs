using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleObject
{
    public DestructibleObjectSO data;
    public Vector2Int coordinate;
    public bool isDestroyed = false;
    
    public DestructibleObject(int hits, string name, Vector2Int coord)
    {
        data.canDestroy = true;
        data.Hits = hits;
        data.Name = name;
        coordinate = coord;
    }

    public void TakeHits()
    {
        if (isDestroyed || !data.canDestroy) return;
        data.Hits -= 1;
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
            cell.DestructibleObject = null;
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
        return !isDestroyed||data.canDestroy;
    }

}
