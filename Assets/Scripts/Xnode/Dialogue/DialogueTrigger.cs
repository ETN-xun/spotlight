using UnityEngine;

[CreateAssetMenu(fileName = "New Dialogue Trigger", menuName = "Dialogue/Dialogue Trigger")]
public class DialogueTrigger : ScriptableObject
{
    public DialogueGraph dialogueGraph;

    /// <summary>
    /// 调用这个方法来触发剧情。
    /// </summary>
    public void Trigger()
    {
//        Debug.Log(11111);
        if (dialogueGraph != null && DialoguePlayer.Instance != null)
        {
           // Debug.Log(2222222);
            DialoguePlayer.Instance.StartDialogue(dialogueGraph);
        }
        
    }
}