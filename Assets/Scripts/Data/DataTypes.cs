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
    Spawn,
    
    /// <summary>
    /// 状态异常技能 - 施加状态异常效果
    /// </summary>
    StatusAbnormal
}

/// <summary>
/// 地形类型枚举
/// 定义地图地形种类
/// </summary>
public enum TerrainType
{
    /// <summary>
    /// 数据平原：无特殊效果，有轻微像素抖动 
    /// </summary>
    Plain,
    
    /// <summary>
    /// 腐蚀区块：对停留单位造成一点伤害并施加状态异常效果
    /// </summary>
    CorrosionTile,
    
    /// <summary>
    /// Bug格子:停留在格子上的单位生命值和攻击力数值调换，并施加状态异常效果
    /// </summary>
    BugTile,
    
    /// <summary>
    /// 缓存区：占领后每回合额外回复2点能量
    /// </summary>
    RegisterTile,
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
    Shield,
    
    /// <summary>
    /// 状态异常效果 - 数据损坏，影响单位行为
    /// </summary>
    StatusAbnormal
}

/// <summary>
/// 状态异常类型枚举
/// 定义不同的状态异常效果
/// </summary>
public enum StatusAbnormalType
{
    /// <summary>
    /// 数据损坏 - 移动和攻击随机化
    /// </summary>
    DataCorruption,
    
    /// <summary>
    /// 系统错误 - 技能冷却时间增加
    /// </summary>
    SystemError,
    
    /// <summary>
    /// 内存泄漏 - 每回合损失能量
    /// </summary>
    MemoryLeak,
    
    /// <summary>
    /// 缓存污染 - 攻击力降低
    /// </summary>
    CacheCorruption
}