using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}
