namespace Action
{
    public class ActionContext
    {
        public Unit ActiveUnit;
        public GridCell TargetCell;
        public ActionManager Manager;

        public ActionContext(Unit unit, GridCell target, ActionManager mgr)
        {
            ActiveUnit = unit;
            TargetCell = target;
            Manager = mgr;
        }
    }

}