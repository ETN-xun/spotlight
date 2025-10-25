using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 单位数据ScriptableObject
/// 配置单位的基础属性和技能
/// </summary>
[CreateAssetMenu(fileName = "UnitData", menuName = "Game/UnitData")]
public class UnitDataSO : ScriptableObject
{
    [Header("基础信息")]
    [Tooltip("单位唯一ID")]
    public string unitID;
    
    public string unitName;
    
    public UnitType unitType;
    
    [Tooltip("单位描述")]
    [TextArea(2, 3)]
    public string description;
    
    public Sprite unitIcon;
    
    [Tooltip("单位预制体")]
    public GameObject unitPrefab;
    
    [Tooltip("是否为敌方单位")]
    public bool isEnemy;

    [Header("基础属性")]
    [Tooltip("最大生命值")]
    public int maxHP = 3;
    
    [Tooltip("受到攻击次数")]
    public int Hits = 2;
    
    [Tooltip("移动范围")]
    public int moveRange = 2;
    
    [Tooltip("基础攻击范围")]
    public int attackRange = 1;
    
    [Tooltip("移动消耗的能量")]
    public int movementEnergyCost = 1;
    
    [Tooltip("能量")]
    public int Energy = 1;
    
    [Tooltip("每回合恢复的能量")]
    public int RecoverEnergy = 0;

    [Header("战斗属性")]
    [Tooltip("基础攻击力")]
    public int baseDamage = 1;
    
    [Tooltip("是否可以被摧毁")]
    public bool canDestroy = false;
    
    [Tooltip("碰撞伤害")]
    public int collisionDamage = 1;

    [Header("技能")]
    [Tooltip("单位拥有的技能列表")]
    public SkillDataSO[] skills;
    
    [Header("AI设置(仅敌方单位)")]
    [Tooltip("AI优先级权重")]
    public float aiPriority = 1.0f;
    
    [Tooltip("攻击建筑的优先级")]
    public float buildingTargetPriority = 2.0f;
    
    [Tooltip("攻击单位的优先级")]
    public float unitTargetPriority = 1.0f;
    
    [Header("热管理设置(仅我方单位)")]
    [Tooltip("每回合触发过热降频行动次数")]
    public int overheatedActionsPerTurn = 1;
}
