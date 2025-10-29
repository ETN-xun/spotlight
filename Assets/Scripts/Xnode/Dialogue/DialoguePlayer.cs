using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using XNode;
using Xnode.Dialogue.Nodes;

public class DialoguePlayer : MonoBehaviour
{
    public DialogueGraph currentGraph;

    [Header("UI References")] public GameObject dialoguePanel;

    public GameObject dialogueLayout;
    public GameObject narrationLayout;
    public Image characterPortraitImage;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI storyText;
    public TextMeshProUGUI narrationText;
    public Image dialogueImage;
    public Image narrationImage;

    [Header("Choice References")] public GameObject choiceButtonContainer;

    public GameObject choiceButtonPrefab;
    public Button nextButton;
    public Button skipButton;

    [Header("Typewriter Effect")] [Tooltip("每秒显示的字符数")]
    public float charactersPerSecond = 20f;

    private readonly List<GameObject> spawnedButtons = new();

    private Node currentNode;
    private bool isTyping;
    private Coroutine typingCoroutine;
    public static DialoguePlayer Instance { get; private set; }


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
        nextButton.onClick.AddListener(OnInteraction);
        skipButton.onClick.AddListener(SkipToNextCheckpoint);
    }

    // 当一个剧情段落结束并需要游戏逻辑介入时触发
    public static event Action<string> OnSectionEnd;

    // 当整个对话UI关闭时触发
    public static event System.Action OnDialogueEnded;

    /// <summary>
    ///     开始一段剧情
    /// </summary>
    public void StartDialogue(DialogueGraph dialogueGraph)
    {
        if (dialogueGraph == null)
        {
            Debug.LogError("[DialoguePlayer] 尝试使用空的剧情图开始对话。");
            return;
        }

        if (currentGraph != null) EndDialogue(false);

        currentGraph = dialogueGraph;
        currentNode = currentGraph.GetStartNode();

        if (currentNode != null)
        {
            dialoguePanel.SetActive(true);
            skipButton.gameObject.SetActive(true);
            // 从StartNode开始，直接前进到第一个实际的节点
            AdvanceToNextNode();
        }
        else
        {
            Debug.LogError($"[DialoguePlayer] 剧情图 '{currentGraph.name}' 中未找到 StartNode。", currentGraph);
        }
    }

    /// <summary>
    ///     获取当前节点的下一个节点
    /// </summary>
    /// <param name="node">当前节点</param>
    /// <param name="index">对于分支节点（如ChoiceNode），指定分支的索引</param>
    /// <returns>下一个节点，如果不存在则返回null</returns>
    public Node GetNextNode(Node node, int index = 0)
    {
        // 优先处理带有索引的分支情况（用于选项节点）
        var p = node.GetOutputPort($"output{(node is ChoiceNode ? $"s {index}" : "")}");
        return p is { IsConnected: true } ? p.GetConnection(0).node : null;
    }


    /// <summary>
    ///     跳过剧情直到下一个 Checkpoint, SectionEnd 或 End 节点
    /// </summary>
    private void SkipToNextCheckpoint()
    {
        ForceStopTyping();

        var targetNode = currentNode;
        // 使用循环来寻找下一个目标节点，增加次数限制防止死循环
        for (var i = 0; i < 100; i++)
        {
            // 如果遇到分支，默认走第一个选项
            targetNode = GetNextNode(targetNode);

            // 如果到达路径终点，则结束对话
            if (targetNode == null)
            {
                EndDialogue(false);
                return;
            }

            // 如果找到目标节点，则处理该节点并停止跳过
            if (targetNode is CheckpointNode || targetNode is SectionEndNode || targetNode is EndNode)
            {
                ProcessNode(targetNode);
                return;
            }
        }

        Debug.LogWarning("[DialoguePlayer] 跳过已达最大步数限制，为防止死循环已停止。");
        EndDialogue(true);
    }

    /// <summary>
    ///     处理玩家交互（点击“下一步”或屏幕）
    /// </summary>
    private void OnInteraction()
    {
        if (isTyping)
            ForceStopTyping();
        else
            AdvanceToNextNode();
    }

    /// <summary>
    ///     前进到当前节点的下一个节点
    /// </summary>
    private void AdvanceToNextNode()
    {
        if (currentNode == null)
        {
            EndDialogue(false);
            return;
        }

        ProcessNode(GetNextNode(currentNode));
    }

    /// <summary>
    ///     核心处理函数，根据节点类型执行不同操作
    /// </summary>
    private void ProcessNode(Node node)
    {
        if (node == null)
        {
            EndDialogue(false);
            return;
        }

        currentNode = node;
        ClearChoices();

        switch (node)
        {
            case DialogueNode dialogueNode:
                DisplayDialogue(dialogueNode);
                break;
            case ChoiceNode choiceNode:
                DisplayChoices(choiceNode);
                break;
            case CheckpointNode _:
                // Checkpoint 节点是透明的，直接跳到下一个
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
            // (可选优化) 增加Default分支用于处理未定义的节点类型
            default:
                Debug.LogWarning($"[DialoguePlayer] 遇到未处理的节点类型: {node.GetType().Name}。对话流程可能中断。", node);
                break;
        }
    }

    /// <summary>
    ///     执行屏幕渐变效果
    /// </summary>
    private void ExecuteFade(FadeScreenNode node)
    {
        ScreenFader.Instance.StartFade(
            node.fadeDuration,
            node.holdDuration,
            node.fadeColor,
            () =>
            {
                // 当画面达到最暗时，隐藏对话UI
                dialoguePanel.SetActive(false);
                skipButton.gameObject.SetActive(false);
                ClearChoices();
            },
            () =>
            {
                // 渐变完全结束后，处理下一个节点
                ProcessNode(GetNextNode(node));
            }
        );
    }

    /// <summary>
    ///     显示对话或旁白
    /// </summary>
    private void DisplayDialogue(DialogueNode node)
    {
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);

        dialoguePanel.SetActive(true);
        //nextButton.gameObject.SetActive(false); // 在打字期间隐藏“下一步”按钮

        // 当没有角色图标时，视为旁白
        var isNarration = node.characterIcon == null;

        dialogueLayout.SetActive(!isNarration);
        narrationLayout.SetActive(isNarration);

        TextMeshProUGUI targetText;
        if (isNarration)
        {
            narrationText.color = node.textColor;
            narrationImage.sprite = node.dialougeIcon;
            targetText = narrationText;
        }
        else
        {
            characterPortraitImage.sprite = node.characterIcon;
            nameText.text = node.characterName;
            dialogueImage.sprite = node.dialougeIcon;
            storyText.color = node.textColor;
            targetText = storyText;
        }

        nextButton.image.sprite = node.dialougeSkip;
        typingCoroutine = StartCoroutine(TypewriterCoroutine(node.dialogueText, targetText));
    }


    /// <summary>
    ///     显示选项按钮
    /// </summary>
    private void DisplayChoices(ChoiceNode node)
    {
        nextButton.gameObject.SetActive(false); // 显示选项时，隐藏“下一步”按钮

        for (var i = 0; i < node.choices.Count; i++)
        {
            var buttonGO = Instantiate(choiceButtonPrefab, choiceButtonContainer.transform);
            spawnedButtons.Add(buttonGO);

            var choiceImage = node.choices[i];
            if (choiceImage != null)
                buttonGO.GetComponent<Button>().image.sprite = choiceImage;

            var choiceIndex = i;
            buttonGO.GetComponent<Button>().onClick.AddListener(() => OnChoiceSelected(choiceIndex));
        }
    }


    /// <summary>
    ///     当玩家点击一个选项按钮时调用
    /// </summary>
    private void OnChoiceSelected(int choiceIndex)
    {
        if (currentNode is ChoiceNode choiceNode) ProcessNode(GetNextNode(choiceNode, choiceIndex));
    }

    /// <summary>
    ///     清理所有已生成的选项按钮
    /// </summary>
    private void ClearChoices()
    {
        if (spawnedButtons.Any())
        {
            foreach (var button in spawnedButtons) Destroy(button);
            spawnedButtons.Clear();
        }
    }

    /// <summary>
    ///     结束对话并清理状态
    /// </summary>
    private void EndDialogue(bool triggerSectionEnd, string eventName = "")
    {
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        isTyping = false;
        typingCoroutine = null;

        dialoguePanel.SetActive(false);
        ClearChoices();
        currentGraph = null;
        currentNode = null;

        if (triggerSectionEnd) OnSectionEnd?.Invoke(eventName);

        OnDialogueEnded?.Invoke();
    }

    /// <summary>
    ///     打字机效果协程
    /// </summary>
    private IEnumerator TypewriterCoroutine(string textToType, TextMeshProUGUI textLabel)
    {
        isTyping = true;
        textLabel.text = "";
        var delay = 1f / charactersPerSecond;

        foreach (var c in textToType)
        {
            textLabel.text += c;
            yield return new WaitForSeconds(delay);
        }

        isTyping = false;
        typingCoroutine = null;

        HandlePostTyping();
    }

    /// <summary>
    ///     强制停止打字并显示完整文本
    /// </summary>
    private void ForceStopTyping()
    {
        if (!isTyping) return;

        StopCoroutine(typingCoroutine);
        isTyping = false;
        typingCoroutine = null;

        if (currentNode is DialogueNode n)
        {
            var targetText = n.characterIcon == null ? narrationText : storyText;
            targetText.text = n.dialogueText;
        }

        HandlePostTyping();
    }

    /// <summary>
    ///     在打字结束或被强制结束后，统一处理后续逻辑
    /// </summary>
    private void HandlePostTyping()
    {
        // 文本显示完毕后，显示“下一步”按钮，等待玩家交互
        nextButton.gameObject.SetActive(true);
    }
}