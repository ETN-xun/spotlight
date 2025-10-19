using XNode;

public class StartNode : BaseNode
{
    [Node.Output(connectionType = Node.ConnectionType.Override)] public Connection output;
}
