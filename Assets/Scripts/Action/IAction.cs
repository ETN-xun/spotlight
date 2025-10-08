using System.Collections;

namespace Action
{
    public interface IAction
    {
        IEnumerator Execute(ActionContext context);
    }
}