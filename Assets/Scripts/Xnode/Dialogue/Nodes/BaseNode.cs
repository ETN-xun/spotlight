using UnityEngine;
using XNode;
/// <summary>
/// 节点通用逻辑
/// </summary>
public abstract class BaseNode : Node
{
    [Input(connectionType = ConnectionType.Override)] public Connection input;
    //获取下一个节点
    public BaseNode GetNextNode()
    {
        foreach (NodePort port in this.Ports)
        {
            if (port.IsOutput && port.IsConnected)
            {
                return port.GetConnection(0).node as BaseNode;
            }
        }
        return null;
    }

    public override object GetValue(NodePort port)
    {
        return null;
    }
    //创建空类用以连接逻辑
    [System.Serializable]
    public class Connection { }
}