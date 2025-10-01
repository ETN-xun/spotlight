using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 地形数据ScriptableObject
/// 配置地形类型和交互效果
/// </summary>
[CreateAssetMenu(fileName = "TerrainData", menuName = "Game/TerrainData")]
public class TerrainDataSO : ScriptableObject
{
    [Header("基础信息")]
    [Tooltip("地形唯一ID")]
    public string terrainID;
    
    [Tooltip("地形名称")]
    public string terrainName;
    
    [Tooltip("地形类型")]
    public TerrainType terrainType;
    
    [Tooltip("地形描述")]
    [TextArea(2, 3)]
    public string description;
    
    [Tooltip("地形预制体")]
    public GameObject terrainPrefab;
    
    [Tooltip("地形图标")]
    public Sprite terrainIcon;

    [Header("基础属性")]
    [Tooltip("是否可行走")]
    public bool isWalkable = true;
    
    [Tooltip("是否可被摧毁")]
    public bool isDestructible = false;
    
    [Tooltip("摧毁所需攻击次数")]
    public int destructionThreshold = 1;
    
    [Tooltip("当前耐久度")]
    public int currentDurability = 1;

    [Header("移动效果")]
    [Tooltip("移动力减少值")]
    public int movementReduction = 0;
    
    [Tooltip("是否阻挡视线")]
    public bool blocksLineOfSight = false;

    [Header("伤害效果")]
    [Tooltip("进入时造成的伤害")]
    public int enterDamage = 0;
    
    [Tooltip("每回合造成的持续伤害")]
    public int persistentDamage = 0;
    
    [Tooltip("是否立即击杀")]
    public bool instantKill = false;

    [Header("位移效果")]
    [Tooltip("是否有弹射效果")]
    public bool hasSpringEffect = false;
    
    [Tooltip("弹射距离")]
    public int springDistance = 1;
    
    [Tooltip("是否增强拉力效果")]
    public bool enhancesPullEffect = false;
    
    [Tooltip("拉力增强倍数")]
    public float pullEnhancement = 1.0f;

    [Header("爆炸效果")]
    [Tooltip("是否有爆炸效果")]
    public bool hasExplosionEffect = false;
    
    [Tooltip("爆炸伤害")]
    public int explosionDamage = 2;
    
    [Tooltip("爆炸范围")]
    public int explosionRadius = 1;
    
    [Tooltip("爆炸推力")]
    public int explosionKnockback = 1;

    [Header("特殊效果")]
    [Tooltip("是否提供护盾效果")]
    public bool providesShield = false;
    
    [Tooltip("护盾值")]
    public int shieldAmount = 0;
    
    [Tooltip("是否可以连锁反应")]
    public bool canChainReact = false;
}
