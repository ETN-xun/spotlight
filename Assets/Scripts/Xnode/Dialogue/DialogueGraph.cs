using UnityEngine;
using XNode;

[CreateAssetMenu(fileName = "New Dialogue Graph", menuName = "Dialogue/New Graph")]
public class DialogueGraph : NodeGraph
{
    // 我们可以把寻找开始节点的方法放在这里
    public StartNode GetStartNode()
    {
        foreach (var node in nodes)
        {
            if (node is StartNode startNode)
            {
                return startNode;
            }
        }
        return null;
    }
}