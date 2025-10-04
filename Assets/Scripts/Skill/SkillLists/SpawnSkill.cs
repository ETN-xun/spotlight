using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnSkill : Skill
{
    public SpawnSkill(SkillDataSO data, Unit caster) : base(data, caster) {}

    public override void Execute(GridCell targetCell, GridManager gridManager)
    {
        if (targetCell.ObjectOnCell != null || targetCell.CurrentUnit != null)
        {
            Debug.Log("Spawn blocked: cell occupied");
            return;
        }

        if (data.spawnPrefab != null)
        {
            GameObject obj = GameObject.Instantiate(data.spawnPrefab, gridManager.CellToWorld(targetCell.Coordinate), Quaternion.identity);
            DestructibleObject destructible = obj.GetComponent<DestructibleObject>();
            if (destructible != null)
            {
                destructible.maxHP = data.spawnHealth;
                destructible.currentHP = data.spawnHealth;
            }
            targetCell.ObjectOnCell = destructible;
        }
    }
}
