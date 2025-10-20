using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartText : MonoBehaviour
{
    // 在 GameManager.cs 中
    public DialogueTrigger level1StartTrigger;

    void Start()
    {
        // ... 其他逻辑 ...
    
        // 调用此方法即可播放剧情
        level1StartTrigger?.Trigger(); 
    }
}