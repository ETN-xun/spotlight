using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using XNode;

public class NarrationNode : BaseNode
{
    [Output(dynamicPortList = true, connectionType = Node.ConnectionType.Override)]
    public List<Connection> answers; 
    
    public List<string> answerTexts; 
    
    public Image narrationIcon;
    
    [TextArea(5, 8)] 
    public string narrationText;
    public bool hasChoices;
    
    public Color32 textColor;
    
    public BaseNode GetNextNode(int answerIndex)
    {
        if(answerIndex >= answers.Count || answerIndex < 0)
            return null;
        //获得名字为answer且编号为answerIndex的端点
        NodePort outputPort = GetOutputPort("answers" + answerIndex);
        
        if (outputPort == null || !outputPort.IsConnected)
            return null;
        
        return outputPort.Connection.node as BaseNode;
    }
}