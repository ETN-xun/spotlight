using System;
using XNode;
using Xnode.Dialogue.Nodes;

/// <summary>
///     节点通用逻辑
/// </summary>
public abstract class InNode : Node
{
    [Input(connectionType = ConnectionType.Override)]
    public Connection input;

    public override object GetValue(NodePort port)
    {
       
        return null;
    }

   
}


// 创建空类用以连接逻辑
[Serializable]
public class Connection
{
}