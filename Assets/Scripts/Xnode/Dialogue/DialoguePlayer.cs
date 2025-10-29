using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System;
using System.Collections;

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
    public Image dialogueImage;
    public Image narrationImage;
    
    [Header("Choice References")]
    public GameObject choiceButtonContainer;  //选项按钮存放的物体
    public GameObject choiceButtonPrefab;  // 选项按钮的预制件
    public Button nextButton;   //下一步按钮
    public Button skipButton;  // 跳过按钮
    
    [Header("Typewriter Effect")]
    [Tooltip("每秒显示的字符数")]
    public float charactersPerSecond = 20f;

    // 私有状态变量
    private Coroutine typingCoroutine; // 用于持有对当前正在运行的协程的引用
    private bool isTyping = false; // 标记当前是否正在打字
    
    private BaseNode currentNode;  //当前节点
    private List<GameObject> spawnedButtons = new List<GameObject>();   //选项按钮
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
        dialoguePanel.SetActive(false);
    }
    private void Start()
    {
        //绑定点击事件
        nextButton.onClick.AddListener(OnInteraction);
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
        ForceStopTyping();
        BaseNode targetNode = currentNode;
        while (targetNode != null)
        {
            // 如果遇到分支，默认走第一个选项
            if (targetNode is DialogueNode dialogueNode && dialogueNode.hasChoices)
                targetNode = dialogueNode.GetNextNode(0);
            
            else if (targetNode is NarrationNode narrationNode && narrationNode.hasChoices)
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
    //点击事件
    private void OnInteraction()
    {
        if (isTyping && typingCoroutine != null)
        {
            ForceStopTyping();
        }
        else
        {
            AdvanceToNextNode();
        }
    }
    // 用于下一步按钮
    private void AdvanceToNextNode()
    {
        //Debug.Log(currentNode.name);
        //Debug.Log(currentNode.GetNextNode());
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
            case CheckpointNode _: 
                AdvanceToNextNode();
                break;
            case SectionEndNode sectionEndNode:
                EndDialogue(true, sectionEndNode.eventName); 
                break; 
            case EndNode _:
                EndDialogue(false);
                break;
            case FadeScreenNode fadeNode:
                ExecuteFade(fadeNode);
                break;
        }
    }
    //  淡出淡入
    private void ExecuteFade(FadeScreenNode node)
    {
//        Debug.Log(node);
        ScreenFader.Instance.StartFade(
            node.fadeDuration, 
            node.holdDuration, 
            node.fadeColor,
            // onPeak: 当画面最黑时，关闭对话UI
            onPeak: () => {
                dialoguePanel.SetActive(false); // 关闭UI
                skipButton.gameObject.SetActive(false);
                ClearChoices();
            },
            onComplete: () => {
                Debug.Log(123456465);
                // 从 FadeScreenNode 获取它的下一个连接节点
                BaseNode nextNode = node.GetNextNode(); 
                // 直接处理下一个节点，实现无缝衔接！
                ProcessNode(nextNode); 
            }
        );
    }

    //显示旁白
    private void DisplayNarration(NarrationNode node)
    {
        if(typingCoroutine != null) StopCoroutine(typingCoroutine);
        
        narrationLayout.SetActive(true);
        dialogueLayout.SetActive(false);
        
        //narrationText.text = node.narrationText;
        narrationText.color = node.textColor;
        narrationImage.sprite = node.narrationIcon;
        nextButton.image.sprite = node.narrationSkip;
        
        nextButton.gameObject.SetActive(true);
        typingCoroutine = StartCoroutine(TypewriterCoroutine(node, narrationText));
        
        /*if (node.hasChoices)
            DisplayChoices(node);*/
    }
    
    //显示对话
    private void DisplayDialogue(DialogueNode node)
    {
        if(typingCoroutine != null) StopCoroutine(typingCoroutine);
        
        dialogueLayout.SetActive(true);
        narrationLayout.SetActive(false);

        characterPortraitImage.sprite = node.characterIcon;
        nameText.text = node.characterName;
        dialogueImage.sprite = node.dialougeIcon;
        nextButton.image.sprite = node.dialougeSkip;
        
        //storyText.text = node.dialogueText;
        
        nextButton.gameObject.SetActive(true);
        typingCoroutine = StartCoroutine(TypewriterCoroutine(node, storyText));
        
        /*if (node.hasChoices)
            DisplayChoices(node);*/
    } 
    
    //显示选项界面
    private void DisplayChoices(DialogueNode node)
    {
        //Debug.Log(node.answers.Count);
        for (int i = 0; i < node.answers.Count; i++)
        {
            GameObject buttonGO = Instantiate(choiceButtonPrefab, choiceButtonContainer.transform);
            spawnedButtons.Add(buttonGO);

            // 设置按钮文本
            buttonGO.GetComponentInChildren<TextMeshProUGUI>().text = null;
            buttonGO.GetComponent<Button>().image.sprite = node.answer[i];

            int choiceIndex = i; 
            
            buttonGO.GetComponent<Button>().onClick.AddListener(() => OnChoiceSelected(choiceIndex));
        }
    }

    private void DisplayChoices(NarrationNode node)
    {
        for (int i = 0; i < node.answers.Count; i++)
        {
            GameObject buttonGO = Instantiate(choiceButtonPrefab, choiceButtonContainer.transform);
            spawnedButtons.Add(buttonGO);

            // 设置按钮文本
            buttonGO.GetComponentInChildren<TextMeshProUGUI>().text = null;
            buttonGO.GetComponent<Button>().image.sprite = node.answer[i];

            int choiceIndex = i; 
            
            buttonGO.GetComponent<Button>().onClick.AddListener(() => OnChoiceSelected(choiceIndex));
        }
    }
    
    // 根据选择的索引找到下一个节点
    private void OnChoiceSelected(int choiceIndex)
    {
        NarrationNode narrationNode = currentNode as NarrationNode;
        DialogueNode dialogueNode = currentNode as DialogueNode;
        if (dialogueNode != null)
        {
            currentNode = dialogueNode.GetNextNode(choiceIndex);
            ProcessNode(currentNode);
        }

        else if (narrationNode != null)
        {
            currentNode = narrationNode.GetNextNode(choiceIndex);
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
    /// <summary>
    /// 逐个字出现
    /// </summary>
    /// <param name="textToType"></param>
    /// <param name="textLabel"></param>
    /// <returns></returns>
    private IEnumerator TypewriterCoroutine(BaseNode node, TextMeshProUGUI textLabel)
    {
        isTyping = true;
        textLabel.text = ""; 
        float delay = 1f / charactersPerSecond;
        string textToType = "";
        
        if (node is DialogueNode dn) textToType = dn.dialogueText;
        if (node is NarrationNode nn) textToType = nn.narrationText;
        
        foreach (char c in textToType)
        {
            textLabel.text += c; 
            yield return new WaitForSeconds(delay); 
        }
        
        typingCoroutine = null;
        isTyping = false;

        if (currentNode is DialogueNode dialogueNode && dialogueNode.hasChoices)
        {
            DisplayChoices(node as DialogueNode);
            Debug.Log(1111);
            nextButton.gameObject.SetActive(false);
        }

        else if (currentNode is NarrationNode narrationNode && narrationNode.hasChoices)
        {
            DisplayChoices(node as NarrationNode);
            Debug.Log(2222);
            nextButton.gameObject.SetActive(false);
        }
        else
        {
            nextButton.gameObject.SetActive(true);
        }
    }
    /// <summary>
    /// 强制停止当前正在运行的打字协程，并立即显示完整文本。
    /// </summary>
    private void ForceStopTyping()
    {
        if (isTyping && typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            
            isTyping = false;
            typingCoroutine = null;
            
            if (currentNode is DialogueNode dn) storyText.text = dn.dialogueText;
            if (currentNode is NarrationNode nn) narrationText.text = nn.narrationText;

            if (currentNode is DialogueNode dialogueNode && dialogueNode.hasChoices)
            {
                DisplayChoices(currentNode as DialogueNode);
                nextButton.gameObject.SetActive(false);
            }
            else if (currentNode is NarrationNode narrationNode && narrationNode.hasChoices)
            {
                DisplayChoices(currentNode as NarrationNode);
                nextButton.gameObject.SetActive(false);
            }
            else
            {
                nextButton.gameObject.SetActive(true);
            }
        }
    }
}