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
        if (targetCell.DestructibleObject != null || targetCell.CurrentUnit != null)
        {
            Debug.Log("Spawn blocked: cell occupied");
            return;
        }

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
        }

    }
}
