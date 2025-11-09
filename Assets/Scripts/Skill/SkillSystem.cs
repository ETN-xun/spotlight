using System.Collections;
using System.Collections.Generic;
using Action;
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
        // SelectTarget(skillData.);
        //获取目标格子
        Vector2Int casterPos = caster.CurrentCell.Coordinate;
        // var coords = skillData.GetTargetableCells(casterPos, GridManager.Instance);

        //高亮
        // ClearHighlights();
        // foreach (var pos in coords)
        // {
        //     var cell = GridManager.Instance.GetCell(pos);
        //     if (cell != null)
        //     {
        //         highlightedCells.Add(cell);
        //         cell.GridCellController.Highlight(true);
        //     }
        // }
    }

    /// <summary>
    /// 点击目标格子时调用
    /// </summary>
    public void SelectTarget(GridCell targetCell)
    {
        if (currentSkill == null) return;
        
        // 验证目标是否在有效范围内
        if (!IsValidTarget(targetCell))
        {
            Debug.Log("目标不在有效范围内或不是有效目标");
            return;
        }
        
        //执行技能
        currentSkill.Execute(targetCell, GridManager.Instance);

        //清理状态
        ClearHighlights();
        currentCaster = null;
        currentSkill = null;
        currentSkillData = null;
    }
    
    /// <summary>
    /// 验证目标是否有效
    /// </summary>
    /// <param name="targetCell">目标格子</param>
    /// <returns>是否为有效目标</returns>
    private bool IsValidTarget(GridCell targetCell)
    {
        if (currentCaster == null || currentSkillData == null || targetCell == null)
            return false;
            
        // 检查目标是否在施法者的攻击范围（曼哈顿距离）内
        Vector2Int casterPos = currentCaster.CurrentCell.Coordinate;
        Vector2Int targetPos = targetCell.Coordinate;
        int manhattanDistance = Mathf.Abs(casterPos.x - targetPos.x) + Mathf.Abs(casterPos.y - targetPos.y);
        bool inRange = manhattanDistance <= currentCaster.data.attackRange;
        
        if (!inRange)
        {
            Debug.Log("目标不在技能范围内");
            return false;
        }
        
        // 检查目标类型是否有效
        return IsValidTargetType(targetCell);
    }
    
    /// <summary>
    /// 检查目标类型是否有效
    /// </summary>
    /// <param name="targetCell">目标格子</param>
    /// <returns>是否为有效目标类型</returns>
    private bool IsValidTargetType(GridCell targetCell)
    {
        // 如果目标格子有单位
        if (targetCell.CurrentUnit != null)
        {
            Unit target = targetCell.CurrentUnit;
            bool isTargetEnemy = target.data.isEnemy;
            bool isCasterEnemy = currentCaster.data.isEnemy;
            
            // 检查是否可以对敌军使用
            if (currentSkillData.canTargetEnemies && isTargetEnemy != isCasterEnemy)
            {
                return true;
            }
            
            // 检查是否可以对友军使用
            if (currentSkillData.canTargetAllies && isTargetEnemy == isCasterEnemy)
            {
                return true;
            }
            
            return false;
        }
        
        // 如果目标格子有可摧毁对象
        if (targetCell.DestructibleObject != null)
        {
            // 大部分技能都可以对可摧毁对象使用
            return true;
        }
        
        // 空格子的情况 - 某些技能（如地形投放）可以对空格子使用
        if (currentSkillData.skillType == SkillType.Spawn)
        {
            return true;
        }
        
        // 其他情况根据技能类型判断
        return false;
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
            case "breakpoint_execution_01"://断点斩杀技能
                return new BreakpointExecutionSkill(data, caster);
            case "forced_migration_01"://强制迁移技能
                return new ForcedMigrationSkill(data, caster);
            case "stack_shield_01"://堆栈护盾技能
                return new ShieldSkill(data, caster);
            case "terrain_deployment_01"://地形投放技能
                return new SpawnSkill(data, caster);
            case "position_swap_01"://移形换影技能
                return new PositionSwapSkill(data, caster);
            case "flashback_displacement_01"://闪回位移技能
                return new FlashbackDisplacementSkill(data, caster);
            case "Status_Abnormal_01"://状态异常技能
                return new StatusAbnormalSkill(data, caster);
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
