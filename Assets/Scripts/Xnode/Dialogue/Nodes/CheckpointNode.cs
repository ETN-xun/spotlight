using XNode;
/// <summary>
/// 用以检测跳过可以到达的节点
/// </summary>
[NodeTint("#3498db")] 

public class CheckpointNode : BaseNode
{
    [Output(connectionType = ConnectionType.Override)] public Connection output;

    //public string checkpointName; 
}