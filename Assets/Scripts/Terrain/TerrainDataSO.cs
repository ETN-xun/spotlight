using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "TerrainData", menuName = "Game/TerrainData")]
public class TerrainDataSO : ScriptableObject
{
    public string terrainName;     // 类型(森林/山脉/岩浆)
    public GameObject terrainPrefab;  //可视化地块
    public bool isWalkable = true; 
    public bool isDestructible = false; 
}
