using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "TileLibrary", menuName = "Game/TileLibrary")]
public class TileLibrary : ScriptableObject
{
    public static TileLibrary Instance;
    private void OnEnable() => Instance = this;

    [Header("建筑Tile")]
    public TileBase FirewallTile;

    [Header("地形Tile")]
    public TileBase Plain;
    public TileBase CorrosionTile;
    public TileBase BugTile;
    public TileBase RegisterTile;
}