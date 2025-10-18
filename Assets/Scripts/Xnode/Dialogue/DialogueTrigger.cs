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
        if (dialogueGraph != null && DialoguePlayer.Instance != null)
        {
            DialoguePlayer.Instance.StartDialogue(dialogueGraph);
        }
        
    }
}