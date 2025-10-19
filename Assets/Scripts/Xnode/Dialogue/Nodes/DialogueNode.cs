using System.Collections.Generic;
using UnityEngine;
using XNode;

public class DialogueNode : BaseNode
{
    [Output(dynamicPortList = true, connectionType = Node.ConnectionType.Override)]
    public List<Connection> answers; 
    
    public List<string> answerTexts; 
    
    public string characterName;
    public Sprite characterIcon;
    
    [TextArea(3,5)]
    public string dialogueText;

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
