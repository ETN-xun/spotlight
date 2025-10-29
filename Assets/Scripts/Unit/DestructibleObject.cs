using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DestructibleObject
{
    public DestructibleObjectSO data;
    public Vector2Int coordinate;
    public bool isDestroyed = false;
    
    public DestructibleObject(DestructibleObjectSO so, Vector2Int coord)
    {
        data = so;
        coordinate = coord;
        isDestroyed = false;
    }
    
    public DestructibleObject(int hits, string name, Vector2Int coord)
    {
        data = ScriptableObject.CreateInstance<DestructibleObjectSO>();
        data.canDestroy = true;
        data.Hits = hits;
        data.Name = name;
        data.Type = DestructibleObjectType.Register;
        data.isActive = false;
        coordinate = coord;
        isDestroyed = false;
    }

    public void TakeHits()
    {
        if (isDestroyed || !data.canDestroy) return;
        data.Hits -= 1;
        if (data.Hits >= 1)
            UpdateTileVisual();
        if (data.Hits <= 0)
        {
            Die();
        }
    }
    
    private void UpdateTileVisual()
    {
        if (GridManager.Instance.objectTilemap == null) return;

        Vector3Int tilePos = new Vector3Int(coordinate.x, coordinate.y, 0);
        TileBase nextTile = null;

        if (data.Type == DestructibleObjectType.Register)
        {
            if (!data.isActive)
                nextTile = data.tileInactive;
            else if (data.Hits > 0)
                nextTile = data.tileFullHP;
            else
                nextTile = data.tileDead;
        }
        else if (data.Type == DestructibleObjectType.FireWall)
        {
            if (data.Hits >= 1)
                nextTile = data.tileFullHP;
            else
                nextTile = data.tileDead;
        }

        GridManager.Instance.objectTilemap.SetTile(tilePos, nextTile);
    }

    public void Die()
    {
        if (isDestroyed) return;
        isDestroyed = true;

        GridCell cell = GridManager.Instance.GetCell(coordinate);
        if (cell != null)
        {
            cell.DestructibleObject = null;
        }

        if (GridManager.Instance.objectTilemap != null)
        {
            Vector3Int tilePos = new Vector3Int(coordinate.x, coordinate.y, 0);
            GridManager.Instance.objectTilemap.SetTile(tilePos, data.tileDead);
            GridManager.Instance.StartCoroutine(DelayedRemoveTile(tilePos, data.deathDelay));
        }
    }
    
    private IEnumerator DelayedRemoveTile(Vector3Int tilePos, float delay)
    {
        yield return new WaitForSeconds(delay);
        GridManager.Instance.objectTilemap.SetTile(tilePos, null);
    }
    
    public bool CanUnitPassThrough(Unit unit)
    {
        return false;
    }
    
    public bool CanTakeDamage()
    {
        return !isDestroyed && data.canDestroy;
    }

}
