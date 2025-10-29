using XNode;
using Xnode.Dialogue.Nodes;

public class StartNode : Node
{
    [Output(connectionType = ConnectionType.Override)]
    public Connection output;
    
    public override object GetValue(NodePort port)
    {
        if (port.fieldName.Equals(nameof(output))) return this;
        return base.GetValue(port);
    }
}
