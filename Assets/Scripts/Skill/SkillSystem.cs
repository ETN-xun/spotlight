using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillSystem : MonoBehaviour
{
    public static SkillSystem Instance { get; private set; }

    private Unit currentCaster;
    private Skill currentSkill;
    private SkillDataSO currentSkillData;
    private List<GridCell> highlightedCells = new List<GridCell>();

    private void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// 选择技能
    /// </summary>
    public void StartSkill(Unit caster, SkillDataSO skillData)
    {
        currentCaster = caster;
        currentSkillData = skillData;
        currentSkill = ChooseSkill(skillData, caster);

        //获取目标格子
        Vector2Int casterPos = caster.CurrentCell.Coordinate;
        var coords = skillData.GetTargetableCells(casterPos, GridManager.Instance);

        //高亮
        ClearHighlights();
        foreach (var pos in coords)
        {
            var cell = GridManager.Instance.GetCell(pos);
            if (cell != null)
            {
                highlightedCells.Add(cell);
                cell.GridCellController.Highlight(true);
            }
        }
    }

    /// <summary>
    /// 点击目标格子时调用
    /// </summary>
    public void SelectTarget(GridCell targetCell)
    {
        if (currentSkill == null) return;
        if (!highlightedCells.Contains(targetCell)) return;

        //执行技能
        currentSkill.Execute(targetCell, GridManager.Instance);

        //清理状态
        ClearHighlights();
        currentCaster = null;
        currentSkill = null;
        currentSkillData = null;
    }

    private void ClearHighlights()
    {
        foreach (var c in highlightedCells)
            c.GridCellController.Highlight(false);
        highlightedCells.Clear();
    }

    private Skill ChooseSkill(SkillDataSO data, Unit caster)
    {
        // 首先检查特定的技能ID
        switch (data.skillID)
        {
            case "breakpoint_execution_01":
                return new BreakpointExecutionSkill(data, caster);
            case "forced_migration_01":
                return new ForcedMigrationSkill(data, caster);
        }
        
        // 然后按技能类型创建通用技能
        switch (data.skillType)
        {
            case SkillType.Damage:
                return new DamageSkill(data, caster);
            case SkillType.Displacement:
                return new DisplacementSkill(data, caster);
            case SkillType.Spawn:
                return new SpawnSkill(data, caster);
            case SkillType.StatusAbnormal:
                return new StatusAbnormalSkill(data, caster);
            default:
                return null;
        }
    }
}
