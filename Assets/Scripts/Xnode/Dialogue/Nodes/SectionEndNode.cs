using XNode;
/// <summary>
/// 段落结束节点
/// </summary>
[NodeTint("#e74c3c")] 
public class SectionEndNode : BaseNode
{
    // 段落终点，没有常规的输出口
    
    public string eventName; // 要发送给GameManager的事件名
}