using UnityEngine;
using XNode;

public class NarrationNode : BaseNode
{
    [Output(connectionType = ConnectionType.Override)] public Connection output;

    [TextArea(5, 8)] 
    public string narrationText;
}