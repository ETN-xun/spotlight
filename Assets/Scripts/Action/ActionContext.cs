namespace Action
{
    public class ActionContext
    {
        public Unit activeUnit;
        public GridCell targetCell;
        public ActionManager manager;

        public ActionContext(Unit unit, GridCell target, ActionManager mgr)
        {
            activeUnit = unit;
            targetCell = target;
            manager = mgr;
        }
    }

}