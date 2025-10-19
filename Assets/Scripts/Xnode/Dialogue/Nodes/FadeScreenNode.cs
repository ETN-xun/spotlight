using UnityEngine;
using XNode;

[NodeTint("#2c3e50")] // 给它一个深邃的颜色
public class FadeScreenNode : BaseNode
{
    [Header("Fade Parameters")]
    [Tooltip("淡入和淡出的单程时长（秒）")]
    public float fadeDuration = 1.0f;
    
    [Tooltip("画面全黑时停留的时长（秒）")]
    public float holdDuration = 0.5f;

    [Tooltip("渐变的目标颜色")]
    public Color fadeColor = Color.black;
    
    [Node.Output(connectionType = Node.ConnectionType.Override)] public Connection output;
}