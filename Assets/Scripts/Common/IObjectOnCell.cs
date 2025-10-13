using UnityEngine;

/// <summary>
/// 格子上对象的通用接口
/// 定义了格子上对象的基本属性和行为
/// </summary>
public interface IObjectOnCell
{
    /// <summary>
    /// 对象名称
    /// </summary>
    string Name { get; }
    
    /// <summary>
    /// 对象坐标
    /// </summary>
    Vector2Int Coordinate { get; }
    
    /// <summary>
    /// 对象生命值
    /// </summary>
    int Hits { get; }
    
    /// <summary>
    /// 受到伤害
    /// </summary>
    /// <param name="damage">伤害值</param>
    void TakeDamage(int damage);
    
    /// <summary>
    /// 对象死亡/销毁
    /// </summary>
    void Die();
    
    /// <summary>
    /// 检查单位是否可以穿过此对象
    /// </summary>
    /// <param name="unit">要检查的单位</param>
    /// <returns>是否可以穿过</returns>
    bool CanUnitPassThrough(Unit unit);
    
    /// <summary>
    /// 检查对象是否可以受到伤害
    /// </summary>
    /// <returns>是否可以受到伤害</returns>
    bool CanTakeDamage();
}