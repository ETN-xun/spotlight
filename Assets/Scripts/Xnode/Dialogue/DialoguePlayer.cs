using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System;

public class DialoguePlayer : MonoBehaviour
{
    public static DialoguePlayer Instance { get; private set; }
    
    // 当一个剧情段落结束并需要游戏逻辑介入时触发
    public static event Action<string> OnSectionEnd; 
    // 当整个对话UI关闭时触发（无论正常结束还是跳过）
    public static event System.Action OnDialogueEnded;
    
    public DialogueGraph currentGraph; //当前播放的剧情图
    
    [Header("UI References")]
    public GameObject dialoguePanel;  //剧情ui
    public GameObject dialogueLayout;  // 对话布局
    public GameObject narrationLayout;  // 旁白布局
    public Image characterPortraitImage; //人物头像
    public TextMeshProUGUI nameText;  //人物名称
    public TextMeshProUGUI storyText;  //对话内容
    public TextMeshProUGUI narrationText;  //旁白内容
    
    [Header("Choice References")]
    public GameObject choiceButtonContainer;  //选项按钮存放的物体
    public GameObject choiceButtonPrefab;  // 选项按钮的预制件
    public Button nextButton;   //下一步按钮
    public Button skipButton;  // 跳过按钮
    
    private BaseNode currentNode;  //当前节点
    private List<GameObject> spawnedButtons = new List<GameObject>();   //选项按钮
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }
    private void Start()
    {
        dialoguePanel.SetActive(false);
        //绑定点击事件
        nextButton.onClick.AddListener(AdvanceToNextNode);
        skipButton.onClick.AddListener(SkipToNextCheckpoint);
    }
    /// <summary>
    /// 开始一段剧情
    /// </summary>
    /// <param name="dialogueGraph"></param>
    public void StartDialogue(DialogueGraph dialogueGraph)
    {
        if (currentGraph != null) { EndDialogue(false); }
        
        currentGraph = dialogueGraph;
        currentNode = currentGraph.GetStartNode();
        if (currentNode != null)
        {
            //开始播放剧情
            dialoguePanel.SetActive(true);
            skipButton.gameObject.SetActive(true);
            AdvanceToNextNode();
        }
    }
    
    //跳过逻辑
    private void SkipToNextCheckpoint()
    {
        BaseNode targetNode = currentNode;
        while (targetNode != null)
        {
            // 如果遇到分支，默认走第一个选项
            if (targetNode is DialogueNode dialogueNode && dialogueNode.answers.Count > 0)
                targetNode = dialogueNode.GetNextNode(0);
            
            else if (targetNode is NarrationNode narrationNode && narrationNode.answers.Count > 0)
                targetNode = narrationNode.GetNextNode(0);
            
            targetNode = targetNode.GetNextNode(); 
            
            if (targetNode is CheckpointNode || targetNode is SectionEndNode || targetNode is EndNode)
            {
                ProcessNode(targetNode);
                return;
            }
        }
        EndDialogue(true);
    }
    
    // 用于下一步按钮
    private void AdvanceToNextNode()
    {
        ProcessNode(currentNode.GetNextNode());
    }
    
    /// <summary>
    /// 处理下一个节点
    /// </summary>
    /// <param name="node"></param>
    private void ProcessNode(BaseNode node)
    {
        if (node == null) { EndDialogue(false); return; }
        
        currentNode = node;
        ClearChoices();
        switch (node)
        {
            case DialogueNode dialogueNode:
                DisplayDialogue(dialogueNode);
                break;
            case NarrationNode narrationNode:
                DisplayNarration(narrationNode);
                break;
            case CheckpointNode _: // 如果是检查点，什么都不显示，直接跳到下一个
                AdvanceToNextNode();
                break;
            case SectionEndNode sectionEndNode:
                // 触发段落结束事件！
                EndDialogue(true, sectionEndNode.eventName); // 结束本次播放
                break; 
            case EndNode _:
                EndDialogue(false);
                break;
            case FadeScreenNode fadeNode:
                // 我们不直接结束对话，而是启动渐变效果
                // 渐变效果会负责在合适的时机关闭UI和发送事件
                ExecuteFade(fadeNode);
                break;
        }
    }
    //  淡出淡入
    private void ExecuteFade(FadeScreenNode node)
    {
        ScreenFader.Instance.StartFade(
            node.fadeDuration, 
            node.holdDuration, 
            node.fadeColor,
            // onPeak: 当画面最黑时，关闭对话UI
            onPeak: () => {
                dialoguePanel.SetActive(false); // 关闭UI
                skipButton.gameObject.SetActive(false);
                ClearChoices();
            }
        );
    }

    //显示旁白
    private void DisplayNarration(NarrationNode node)
    {
        narrationLayout.SetActive(true);
        dialogueLayout.SetActive(false);
        
        narrationText.text = node.narrationText;
        narrationText.color = narrationText.color;
        
        if (node.hasChoices)
        {
            nextButton.gameObject.SetActive(false);
            DisplayChoices(node);
        }
        else
        {
            nextButton.gameObject.SetActive(true);
        }
    }
    
    //显示对话
    private void DisplayDialogue(DialogueNode node)
    {
        dialogueLayout.SetActive(true);
        narrationLayout.SetActive(false);

        characterPortraitImage.sprite = node.characterIcon;
        nameText.text = node.characterName;
        storyText.text = node.dialogueText;
        
        if (node.hasChoices)
        {
            nextButton.gameObject.SetActive(false);
            DisplayChoices(node);
        }
        else
        {
            nextButton.gameObject.SetActive(true);
        }
    } 
    
    //显示选项界面
    private void DisplayChoicesBase(IList<BaseNode.Connection> answers,List<string> choices) 
    {
        for (int i = 0; i < answers.Count; i++)
        {
            GameObject buttonGO = Instantiate(choiceButtonPrefab, choiceButtonContainer.transform);
            spawnedButtons.Add(buttonGO);

            // 设置按钮文本
            buttonGO.GetComponentInChildren<TextMeshProUGUI>().text = choices[i]; 

            int choiceIndex = i; 
            
            buttonGO.GetComponent<Button>().onClick.AddListener(() => OnChoiceSelected(choiceIndex));
        }
    }
    private void DisplayChoices(DialogueNode node) => DisplayChoicesBase(node.answers,node.answerTexts);
    private void DisplayChoices(NarrationNode node) => DisplayChoicesBase(node.answers,node.answerTexts);
    
    // 根据选择的索引找到下一个节点
    private void OnChoiceSelected(int choiceIndex)
    {
        
        DialogueNode dialogueNode = currentNode as DialogueNode;
        if (dialogueNode != null)
        {
            currentNode = dialogueNode.GetNextNode(choiceIndex);
            ProcessNode(currentNode);
        }
    }
    private void ClearChoices()
    {
        foreach (GameObject button in spawnedButtons)
        {
            Destroy(button);
        }
        spawnedButtons.Clear();
    }

    private void EndDialogue(bool triggerSectionEnd = false, string eventName = "")
    {
        dialoguePanel.SetActive(false);
        skipButton.gameObject.SetActive(false);
        nextButton.gameObject.SetActive(false);
        ClearChoices();
        currentGraph = null;
        
        if (triggerSectionEnd)
        {
            OnSectionEnd?.Invoke(eventName);//段落结束触发该事件
        }
        
        OnDialogueEnded?.Invoke();//对话结束触发该事件
    }
}