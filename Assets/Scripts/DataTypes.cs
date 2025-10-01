using System;

/// <summary>
/// 技能类型枚举
/// 定义技能的分类
/// </summary>
public enum SkillType
{
    /// <summary>
    /// 伤害技能 - 造成直接伤害
    /// </summary>
    Damage,
    
    /// <summary>
    /// 位移技能 - 推拉移动单位
    /// </summary>
    Displacement,
    
    /// <summary>
    /// 生成技能 - 创建临时物体
    /// </summary>
    Spawn
}

/// <summary>
/// 地形类型枚举
/// 定义地图地形种类
/// </summary>
public enum TerrainType
{
    /// <summary>
    /// 平原 - 基础地形
    /// </summary>
    Plain,
    
    /// <summary>
    /// 山脉 - 阻挡地形
    /// </summary>
    Mountain,
    
    /// <summary>
    /// 森林 - 减速地形
    /// </summary>
    Forest,
    
    /// <summary>
    /// 岩浆 - 致命地形
    /// </summary>
    Lava,
    
    /// <summary>
    /// 深渊 - 致命地形
    /// </summary>
    Abyss,
    
    /// <summary>
    /// 动能弹簧 - 弹射地形
    /// </summary>
    Spring,
    
    /// <summary>
    /// 磁性立场 - 增强拉力地形
    /// </summary>
    MagneticField,
    
    /// <summary>
    /// 炸药桶 - 可摧毁爆炸地形
    /// </summary>
    ExplosiveBarrel
}

/// <summary>
/// 位移方向枚举
/// </summary>
public enum DisplacementDirection
{
    /// <summary>
    /// 推 - 远离施法者
    /// </summary>
    Push,
    
    /// <summary>
    /// 拉 - 靠近施法者
    /// </summary>
    Pull
}

/// <summary>
/// 效果类型枚举
/// 定义技能效果的类型
/// </summary>
public enum EffectType
{
    /// <summary>
    /// 伤害效果
    /// </summary>
    Damage,
    
    /// <summary>
    /// 位移效果
    /// </summary>
    Displacement,
    
    /// <summary>
    /// 生成效果
    /// </summary>
    Spawn,
    
    /// <summary>
    /// 治疗效果
    /// </summary>
    Heal,
    
    /// <summary>
    /// 护盾效果
    /// </summary>
    Shield
}