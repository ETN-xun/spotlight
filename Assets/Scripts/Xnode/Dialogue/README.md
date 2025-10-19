# 剧情系统对接文档 (Dialogue System Integration Guide)

**版本:** 1.0
**负责人:** Cab
---

### 1. 系统概述

本剧情系统是一个由数据驱动、基于xNode构建的可视化叙事工具。系统设计遵循高内聚、低耦合原则，**游戏主流程 (`GameManager`) 无需关心剧情系统的内部实现**。

主流程通过调用标准化的**“剧情触发器”资产**来播放剧情，并通过**监听C#事件**来响应剧情节点，从而实现“剧情 → 玩法”的无缝转换。

### 2. 核心概念：剧情触发器 (`DialogueTrigger`)

您在`GameManager`中唯一需要交互的对象是 **`DialogueTrigger`**。

*   **它是什么？**：一个 `ScriptableObject` 资产，代表一个可播放的剧情片段。
*   **我（剧情负责人）会做什么？**：我会负责创建所有的剧情图（Graph），并将每一个需要被调用的剧情片段（如“第一关开场白”、“NPC对话”等）打包成一个`DialogueTrigger`资产，例如 `DT_Level1_Start.asset`。
*   **您需要做什么？**：您只需要在您的`GameManager`中**持有对这些`DialogueTrigger`资产的引用**，并在适当的时机调用它们。


---

### 3. API & 对接指南

#### 3.1 如何播放剧情

播放剧情的唯一入口是调用`DialogueTrigger`资产的公共方法。

```csharp
// 在 GameManager.cs 中
public DialogueTrigger level1StartTrigger;

void Start()
{
    // ... 其他逻辑 ...
    
    // 调用此方法即可播放剧情
    level1StartTrigger?.Trigger(); 
}
```

> **方法**: `public void Trigger()`
> **作用**: 指示剧情系统开始播放该触发器内绑定的剧情图。
> **注意**: 这是一个**“只管触发，不问结果”**的异步调用。调用后，剧情系统会接管流程，直到剧情结束或需要游戏逻辑介入。

#### 3.2 如何响应剧情事件 (核心)

剧情系统通过两个全局静态 `event` 向外广播信号。您需要在`GameManager`中订阅这些事件来做出响应。

**强烈建议在 `OnEnable()` 中订阅，在 `OnDisable()` 中取消订阅。**

```csharp
// 在 GameManager.cs 中

private void OnEnable()
{
    DialoguePlayer.OnSectionEnd += HandleDialogueEvent;
    DialoguePlayer.OnDialogueEnded += OnDialogueUIClosed;
}

private void OnDisable()
{
    DialoguePlayer.OnSectionEnd -= HandleDialogueEvent;
    DialoguePlayer.OnDialogueEnded -= OnDialogueUIClosed;
}
```

---

#### Event 1: `DialoguePlayer.OnSectionEnd`

这是**最重要的逻辑事件**，用于实现“剧情→玩法”的转换。

*   **签名**: `public static event Action<string> OnSectionEnd;`
*   **触发时机**: 当剧情播放到我放置的 `SectionEndNode`（段落结束节点）时触发。
*   **用途**: **接收来自剧情的明确指令**。`string`参数是我在节点中配置的`eventName`，用于告诉您接下来该做什么，（可以你告诉我方法名我去Xnode中修改）。
*   **示例**:
    ```csharp
    private void HandleDialogueEvent(string eventName)
    {
        Debug.Log($"接收到剧情指令: {eventName}");
        switch (eventName)
        {
            case "StartLevel1":
                // 在这里编写开始第一关游戏玩法的逻辑
                break;
            case "SpawnBoss":
                // 在这里编写生成Boss的逻辑
                break;
            // ... 其他事件 ...
        }
    }
    ```

#### Event 2: `DialoguePlayer.OnDialogueEnded`

这是一个通用的**状态事件**，用于处理UI关闭后的通用逻辑（如恢复玩家控制）。

*   **签名**: `public static event Action OnDialogueEnded;`
*   **触发时机**: **任何时候**剧情UI关闭时（包括正常结束、段落结束、玩家跳过）。
*   **用途**: **解锁/恢复**因剧情播放而被暂停的游戏功能。
*   **示例**:
    ```csharp
    private void OnDialogueUIClosed()
    {
        Debug.Log("剧情UI已关闭，恢复玩家输入。");
        // 在这里编写解锁玩家移动、攻击等操作的逻辑
        PlayerInput.Enable(); 
    }
    ```

---

### 4. 完整协作流程示例

以下是实现“播放第一关开场白 → 开始第一关 → 玩家通关 → 播放第一关结束语”的完整流程。

#### 4.1 我方（剧情系统）负责
1.  制作 `Level1_Start.graph`，其结尾是一个`SectionEndNode`，`eventName`设为`"StartLevel1"`。
2.  制作 `Level1_End.graph`，其结尾是一个`SectionEndNode`，`eventName`设为`"GoToMainMenu"`（或其它）。
3.  创建对应的 `DialogueTrigger` 资产：`DT_Level1_Start.asset` 和 `DT_Level1_End.asset`，并将它们提供给您。
4.  确保场景中存在且仅存在一个`DialoguePlayer`的预制件实例。

#### 4.2 您方（GameManager）负责
1.  在`GameManager.cs`中创建两个公共字段，并**在Inspector中拖入我提供的资产**。
    ```csharp
    public DialogueTrigger level1StartTrigger;
    public DialogueTrigger level1EndTrigger;
    ```
2.  在`Start()`方法中，触发开场剧情，并**锁定玩家输入**。
    ```csharp
    void Start()
    {
        level1StartTrigger?.Trigger();
        PlayerInput.Disable(); // 您需要实现这个
    }
    ```
3.  在事件处理函数中，响应指令并解锁玩家输入。
    ```csharp
    private void OnEnable()
    {
        DialoguePlayer.OnSectionEnd += HandleDialogueEvent;
        DialoguePlayer.OnDialogueEnded += UnlockPlayerInput;
    }
    // ... OnDisable ...

    private void HandleDialogueEvent(string eventName)
    {
        if (eventName == "StartLevel1")
        {
            // 开始游戏的核心逻辑
            InitializeLevel(1);
        }
    }

    private void UnlockPlayerInput()
    {
        PlayerInput.Enable(); // 您需要实现这个
    }
    ```
4.  在您的关卡逻辑判定胜利时，调用方法来触发结束剧情。
    ```csharp
    // 这个方法由您的关卡胜利逻辑调用
    public void OnLevel1Completed()
    {
        level1EndTrigger?.Trigger();
        PlayerInput.Disable(); // 再次锁定输入，准备播放结束剧情
    }
    ```

### 5. FAQ

*   **Q: 如果玩家跳过剧情会怎么样？**
    *   **A:** 系统会自动处理。如果跳过的段落包含`SectionEndNode`，`OnSectionEnd`事件依然会**被正确触发**。您无需为此编写任何特殊逻辑。

*   **Q: 我需要关心`DialoguePlayer`的内部实现或场景中的位置吗？**
    *   **A:** 不需要。您只需要确保预制件在场景中即可。所有交互都通过全局事件和`DialogueTrigger`资产完成。

*   **Q: 如果我调用`Trigger()`时，传入的资产为空会怎样？**
    *   **A:** 调用会安全地失败，需要的话我可以加入失败逻辑。
---

如有任何疑问，请随时与我沟通。