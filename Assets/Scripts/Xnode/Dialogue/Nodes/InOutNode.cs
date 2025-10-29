using XNode;

namespace Xnode.Dialogue.Nodes
{
    public class InOutNode : InNode
    {
        [Output(connectionType = ConnectionType.Override)]
        public Connection output;

        public override object GetValue(NodePort port)
        {
            if (port.fieldName.Equals(nameof(output))) return this;
            return base.GetValue(port);
        }
    }
}