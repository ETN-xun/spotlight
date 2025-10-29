using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "DestructibleObjectSO", menuName = "Game/DestructibleObjectSO")]
public class DestructibleObjectSO : ScriptableObject
{
    [Header("基础信息")]
    [Tooltip("单位名称")]
    public string Name;
    
    [Tooltip("是否可以破坏")]
    public bool canDestroy = false;

    [Tooltip("护盾")] 
    public int Hits;
    
    [Tooltip("建筑类型")]
    public DestructibleObjectType Type;
    
    [Tooltip(("是否激活"))]
    public bool isActive = false;
    
    [Header("不同状态的Tile")]
    public TileBase tileFullHP;      // 满护盾Tile
    public TileBase tileInactive;    // 未激活Tile（缓存区专用）
    public TileBase tileDead;        // 死亡Tile

    [Header("死亡延时(秒)")]
    public float deathDelay = 1.5f;
}
