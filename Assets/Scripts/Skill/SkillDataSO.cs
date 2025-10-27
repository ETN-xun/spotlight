using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// 技能数据ScriptableObject
/// 配置技能的所有属性和效果
/// </summary>
[CreateAssetMenu(fileName = "SkillData", menuName = "Game/SkillData")]
public class SkillDataSO : ScriptableObject
{
    [Header("基础信息")]
    [Tooltip("技能唯一ID")]
    public string skillID;
    
    [Tooltip("技能名称")]
    public string skillName;
    
    [Tooltip("技能描述")]
    [TextArea(2, 4)]
    public string description;
    
    [Tooltip("技能图标")]
    public Sprite skillIcon;
    
    [Header("技能属性")]
    [Tooltip("技能类型")]
    public SkillType skillType;
    
    [Tooltip("能量消耗")]
    public int energyCost = 1;
    
    [Tooltip("攻击范围")]
    public int range = 1;
    
    [Tooltip("是否需要目标")]
    public bool requiresTarget = true;
    
    [Tooltip("是否可以对友军使用")]
    public bool canTargetAllies = false;
    
    [Tooltip("是否可以对敌军使用")]
    public bool canTargetEnemies = true;
    
    [Header("伤害属性")]
    [Tooltip("基础伤害值")]
    public int baseDamage = 0;
    
    [Tooltip("是否造成撞击伤害")]
    public bool causeCollisionDamage = false;
    
    [Header("位移属性")]
    [Tooltip("位移方向")]
    public DisplacementDirection displacementDirection;
    
    [Tooltip("位移距离")]
    public int displacementDistance = 0;
    
    [Header("生成属性")]
    [Tooltip("生成物护盾")]
    public int spawnHits = 2;
    
    [Tooltip("生成的Tile（用于Tilemap模式）")]
    public TileBase spawnTile;
    
    [Header("范围效果")]
    [Tooltip("影响范围(以目标为中心)")]
    public int effectRadius = 0;
    
    [Tooltip("范围内的相对位置偏移")]
    public List<Vector2Int> effectPattern = new List<Vector2Int>();
    
    [Header("特殊效果")]
    [Tooltip("技能效果列表")]
    public List<EffectType> effects = new List<EffectType>();
    
    [Tooltip("冷却回合数")]
    public int cooldownTurns = 0;
    
    
    
    [Header("状态异常属性")]
    [Tooltip("状态异常类型")]
    public StatusAbnormalType statusAbnormalType;
    
    [Tooltip("状态异常持续回合数")]
    public int statusDuration = 3;
    
    [Tooltip("状态异常强度")]
    public float statusIntensity = 1.0f;
    
    [Tooltip("状态异常是否可叠加")]
    public bool statusCanStack = true;
    
    [Tooltip("状态异常最大叠加层数")]
    public int statusMaxStacks = 5;
    [Tooltip("是否为被动技能")]
    public bool isPassive = false;
    
    /// <summary>
    /// 获取技能的目标格子
    /// </summary>
    /// <param name="casterPosition">施法者位置</param>
    /// <param name="gridManager">网格管理器</param>
    /// <returns>可以作为目标的格子列表</returns>
    public List<Vector2Int> GetTargetableCells(Vector2Int casterPosition, GridManager gridManager)
    {
        List<Vector2Int> targetableCells = new List<Vector2Int>();
        
        // 根据范围计算可攻击的格子
        for (int x = -range; x <= range; x++)
        {
            for (int y = -range; y <= range; y++)
            {
                if (x == 0 && y == 0) continue; // 跳过自身位置
                
                // 使用曼哈顿距离而不是正方形区域，确保只包括相邻格子
                int manhattanDistance = Mathf.Abs(x) + Mathf.Abs(y);
                if (manhattanDistance > range) continue;
                
                Vector2Int targetPos = casterPosition + new Vector2Int(x, y);
                
                // 检查是否在网格范围内
                if (gridManager.IsValidPosition(targetPos))
                {
                    // 根据技能类型和设置判断是否可以作为目标
                    if (IsValidTarget(targetPos, gridManager))
                    {
                        targetableCells.Add(targetPos);
                    }
                }
            }
        }
        
        return targetableCells;
    }
    
    /// <summary>
    /// 检查指定位置是否为有效目标
    /// </summary>
    /// <param name="targetPosition">目标位置</param>
    /// <param name="gridManager">网格管理器</param>
    /// <returns>是否为有效目标</returns>
    private bool IsValidTarget(Vector2Int targetPosition, GridManager gridManager)
    {
        // 这里可以根据技能类型和设置进行更复杂的判断
        // 暂时返回true，具体逻辑在技能系统中实现
        return true;
    }
}