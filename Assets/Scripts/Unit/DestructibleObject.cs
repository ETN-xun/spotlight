using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleObject
{
    public UnitDataSO data;
    public Vector2Int coordinate;
    public bool isDestroyed = false;
    
    public DestructibleObject(int hits, string name, Vector2Int coord)
    {
        data.canDestroy = true;
        data.Hits = hits;
        data.unitName = name;
        coordinate = coord;
    }

    public void TakeDamage()
    {
        if (isDestroyed) return;
        data.Hits -= 1;
        if (data.Hits <= 0)
        {
            Die();
        }
    }

    private void Die()
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

}
