using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 地形投放技能
/// </summary>

public class SpawnSkill : Skill
{
    public SpawnSkill(SkillDataSO data, Unit caster) : base(data, caster) {}

    public override void Execute(GridCell targetCell, GridManager gridManager)
    {
        // 验证目标位置是否有效
        if (!IsValidSpawnTarget(targetCell))
        {
            Debug.Log("无法在此位置使用地形投放技能：位置无效");
            return;
        }
        
        // 如果目标位置有对象，先清除它们
        if (targetCell.DestructibleObject != null || targetCell.CurrentUnit != null)
        {
            if (targetCell.DestructibleObject != null)
            {
                targetCell.DestructibleObject.TakeHits();
                Debug.Log($"{caster.data.unitName} 摧毁了可摧毁对象");
            }
            else if (targetCell.CurrentUnit != null)
            {
                targetCell.CurrentUnit.TakeDamage(1);
                Debug.Log($"{caster.data.unitName} 对 {targetCell.CurrentUnit.data.unitName} 造成了1点伤害");
            }
        }

        // 生成新的地形对象
        if (gridManager.objectTilemap != null && data.spawnTile != null)
        {
            Vector3Int tilePos = new Vector3Int(targetCell.Coordinate.x, targetCell.Coordinate.y, 0);
            gridManager.objectTilemap.SetTile(tilePos, data.spawnTile);

            // 建筑逻辑数据
            DestructibleObject destructible = new DestructibleObject( 
                hits: data.spawnHits,
                name: data.skillName,
                coord: targetCell.Coordinate
            );
            targetCell.DestructibleObject = destructible;
            
            Debug.Log($"{caster.data.unitName} 在 ({targetCell.Coordinate.x}, {targetCell.Coordinate.y}) 投放了地形");
        }
    }
    
    /// <summary>
    /// 检查是否为有效的生成目标位置
    /// </summary>
    /// <param name="targetCell">目标格子</param>
    /// <returns>是否为有效位置</returns>
    private bool IsValidSpawnTarget(GridCell targetCell)
    {
        // 地形投放技能可以在任何位置使用
        // 如果有单位或可摧毁对象，会先清除它们再生成新地形
        return true;
    }
}
