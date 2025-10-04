using UnityEngine;

public abstract class Skill
{
    protected SkillDataSO data;
    protected Unit caster;

    public Skill(SkillDataSO data, Unit caster)
    {
        this.data = data;
        this.caster = caster;
    }

    /// <summary>
    /// 执行技能效果
    /// </summary>
    public abstract void Execute(GridCell targetCell, GridManager gridManager);
}