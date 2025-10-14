public enum InputState
{
    IdleState,
    SelectPlayerUnitState,
    SelectEnemyUnitState,
    SelectNoUnitState,
    MoveUnitState,
    SelectSkillState,
}
    
public enum InputType
{
    NoClick,
    ClickPlayerUnit,
    ClickEnemyUnit,
    ClickNoUnit,
    ClickView,
    ClickSkill,
    ClickMove,
}