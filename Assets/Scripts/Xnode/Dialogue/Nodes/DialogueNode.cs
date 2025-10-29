using System.Collections.Generic;
using UnityEngine;
using XNode;
using Xnode.Dialogue.Nodes;

// 实现IChoiceNode接口，以支持通用的选项处理逻辑
public class DialogueNode : InOutNode
{
    
    public string characterName;
    public Sprite characterIcon;
    public Sprite dialougeIcon;
    public Sprite dialougeSkip;
    public Color textColor = Color.white;
    
    [TextArea(3,5)]
    public string dialogueText;
}