using System;
using System.Collections;
using System.Collections.Generic;
using Action;
using Common;
using Level;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using View.Base;
using Enemy;
using Scene;
using Sound; // for EnemyManager

namespace View.GameViews
{
    public class FightView : BaseView
    {
        [SerializeField] private List<Image> energyImages;
        [SerializeField] private Sprite lEnergyBright;
        [SerializeField] private Sprite lEnergyDark;
        [SerializeField] private Sprite energyBright;
        [SerializeField] private Sprite energyDark;
        
        [SerializeField] private Sprite overloadModeActiveSprite;
        [SerializeField] private Sprite overloadModeInactiveSprite;

        [SerializeField] private GameObject popupSettingsView;
        
        // 阶段文本组件
        private TextMeshProUGUI stageText;
        // 引导提示文本组件（阶段提示）
        private TextMeshProUGUI hintText;
        // 技能悬停提示文本组件
        private TextMeshProUGUI level2TipText;
private Image level2TipBackground;
private Button level2TipCloseButton;
private TextMeshProUGUI skillText;
        
        // 淡入淡出效果相关
        private Coroutine stageTextFadeCoroutine;
        private Coroutine hintTextFadeCoroutine;
        private bool level2HintActive;
private Coroutine level2HintCoroutine;
private const float level2HintDuration = 30f;
private const float fadeInDuration = 0.3f;
        private const float fadeOutDuration = 0.2f;
        
        protected override void InitView()
        {
            base.InitView();
            ViewManager.Instance.RegisterView(ViewType.SkillSelectView, new ViewInfo()
            {
                PrefabName = "SkillSelectView",
                ParentTransform = transform.Find("Background")
            });

            ViewManager.Instance.RegisterView(ViewType.TerrainInfoView, new ViewInfo()
            {
                PrefabName = "TerrainInfoView",
                ParentTransform = transform.Find("Background")
            });

            ViewManager.Instance.RegisterView(ViewType.EnemyInfoView, new ViewInfo()
            {
                PrefabName = "EnemyInfoView",
                ParentTransform = transform.Find("Background")
            });
            
            MessageCenter.Subscribe(Defines.PlayerTurnStartEvent, OnPlayerTurnStart);
            MessageCenter.Subscribe(Defines.PlayerTurnEndEvent, OnPlayerTurnEnd);
            MessageCenter.Subscribe(Defines.EnemyTurnStartEvent, OnEnemyTurnStart);
            MessageCenter.Subscribe(Defines.EnemyTurnEndEvent, OnEnemyTurnEnd);
            MessageCenter.Subscribe(Defines.DeploymentStateEndedEvent, OnDeploymentStateEnded);
        }
        

        public override void Open(params object[] args)
        {
            base.Open(args);
            MessageCenter.Subscribe(Defines.EnergyChangedEvent, OnEnergyChanged);
            MessageCenter.Subscribe(Defines.UnitTakeDamageEvent, OnUnitTakeDamage);
            MessageCenter.Subscribe(Defines.SkillHoverEnterEvent, OnSkillHoverEnter);
            MessageCenter.Subscribe(Defines.SkillHoverExitEvent, OnSkillHoverExit);
            Find<Button>("Background/Settings_Btn").onClick.AddListener(OnClickSettingsButton);
            Find<Button>("Background/EndTurn_Btn").onClick.AddListener(OnClickEndTurnButton);
            Find<Button>("Background/PopupSettings/ContineGame").onClick.AddListener(OnClickContinueGameButton);
            Find<Button>("Background/PopupSettings/ReturnMainMenu").onClick.AddListener(OnClickReturnMainMenuButton);
            Find<Button>("Background/PopupSettings/SkipFight").onClick.AddListener(OnClickSkipFightButton);
            Find<TextMeshProUGUI>("Background/TurnTarget/Text (TMP)").text = LevelManager.Instance.GetCurrentLevel().levelTarget;
            UpdateEnergyBar(ActionManager.EnergySystem.GetCurrentEnergy());
            popupSettingsView.SetActive(false);
            
            Find<Button>("Background/EndTurn_Btn").interactable = false;
            
            // 初始化阶段文本组件
            stageText = Find<TextMeshProUGUI>("Background/StageText");
            UpdateStageText(GameManager.Instance.CurrentGameState);

            // 初始化提示文本组件
            hintText = Find<TextMeshProUGUI>("Background/HintText");
            skillText = Find<TextMeshProUGUI>("Background/SkillText");
level2TipText = Find<TextMeshProUGUI>("Background/第二关提示");
if (level2TipText == null) level2TipText = Find<TextMeshProUGUI>("第二关提示");
            ShowLevel2HintIfApplicable();
UpdateHintText(GameManager.Instance.CurrentGameState);
            
            // InitializeOverloadModeButton();
        }

        private void OnClickSkipFightButton()
        {
            // 跳过战斗：隐藏战斗/部署相关 UI，直接触发当前关卡的收束剧情，并标记为胜利
            // 先关闭部署与战斗视图，避免在剧情期间 UI 残留
            ViewManager.Instance.CloseView(ViewType.DeploymentView);
            ViewManager.Instance.CloseView(ViewType.FightView);
            int levelIndex = LevelManager.Instance != null
                ? LevelManager.Instance.GetCurrentLevelIndex()
                : 1;
            GameManager.Instance.ReportGameResult(true);
            GameManager.Instance.PlayerCompletedLevel(levelIndex);
        }

        public override void Close(params object[] args)
        {
            base.Close();
            Debug.Log("Close");
            MessageCenter.Unsubscribe(Defines.EnergyChangedEvent, OnEnergyChanged);
            MessageCenter.Unsubscribe(Defines.UnitTakeDamageEvent, OnUnitTakeDamage);
            MessageCenter.Unsubscribe(Defines.SkillHoverEnterEvent, OnSkillHoverEnter);
            MessageCenter.Unsubscribe(Defines.SkillHoverExitEvent, OnSkillHoverExit);
            Find<Button>("Background/Settings_Btn").onClick.RemoveListener(OnClickSettingsButton);
            Find<Button>("Background/EndTurn_Btn").onClick.RemoveListener(OnClickEndTurnButton);
            Find<Button>("Background/PopupSettings/ContineGame").onClick.RemoveListener(OnClickContinueGameButton);
            Find<Button>("Background/PopupSettings/SkipFight").onClick.RemoveListener(OnClickSkipFightButton);
            Find<Button>("Background/PopupSettings/ReturnMainMenu").onClick.RemoveListener(OnClickReturnMainMenuButton);
        }
        
        private void OnClickContinueGameButton()
        {
            popupSettingsView.SetActive(!popupSettingsView.activeSelf);
        }

        private void OnClickReturnMainMenuButton()
        {
            SceneLoadManager.Instance.LoadScene(SceneType.MainMenu);
        }
        
        private void OnEnergyChanged(object[] args)
        {
            if (args == null || args.Length == 0) return;
            if (args[0] is not int energy) return;
            UpdateEnergyBar(energy);
        }

        private void OnUnitTakeDamage(object[] args)
        {
            // 根据 unit id 从 AllyManager 或 EnemyManager 获取单位，做 UI 更新（避免空引用）
            if (args == null || args.Length == 0) return;
            if (args[0] is not string unitId || string.IsNullOrEmpty(unitId)) return;

            // 先尝试我方
            var allyMgr = Ally.AllyManager.Instance;
            var ally = allyMgr != null ? allyMgr.GetAliveAllyByID(unitId) : null;
            if (ally != null)
            {
                switch (ally.data.unitType)
                {
                    case UnitType.Zero:
                        // TODO: 更新 Zero 的受伤 UI
                        break;
                    case UnitType.Shadow:
                        // TODO: 更新 Shadow 的受伤 UI
                        break;
                    case UnitType.Stone:
                        // TODO: 更新 Stone 的受伤 UI
                        break;
                    default:
                        // 其他未定义我方单位类型
                        Debug.LogWarning($"未处理的我方单位类型: {ally.data.unitType}");
                        break;
                }
                return;
            }

            // 再尝试敌方
            var enemyMgr = EnemyManager.Instance;
            var enemy = enemyMgr != null ? enemyMgr.GetAliveEnemyByID(unitId) : null;
            if (enemy != null)
            {
                switch (enemy.data.unitType)
                {
                    case UnitType.GarbledCrawler:
                        // TODO: 更新 敌方乱码爬虫 的受伤 UI
                        break;
                    case UnitType.CrashUndead:
                        // TODO: 更新 敌方死机亡灵 的受伤 UI
                        break;
                    case UnitType.NullPointer:
                        // TODO: 更新 敌方空指针 的受伤 UI
                        break;
                    case UnitType.RecursivePhantom:
                        // TODO: 更新 敌方递归幻影 的受伤 UI
                        break;
                    default:
                        Debug.LogWarning($"未处理的敌方单位类型: {enemy.data.unitType}");
                        break;
                }
                return;
            }

            // 既不是我方也不是敌方（可能单位刚好死亡或未注册），忽略
            Debug.LogWarning($"未找到受伤单位，unitId: {unitId}");
        }

        private void OnClickSettingsButton()
        {
            popupSettingsView.SetActive(!popupSettingsView.activeSelf);
        }
        
        private void OnClickEndTurnButton()
        {
            GameManager.Instance.ChangeGameState(GameState.EnemyTurn);
            SoundManager.Instance.PlaySFX(0);
        }
        
        private void InitializeOverloadModeButton()
        {
            Find<Button>("Background/OverloadMode_Btn").onClick.AddListener(OnClickOverloadModeButton);
            UpdateOverloadModeButtonState();
        }
        
        private void OnClickOverloadModeButton()
        {
            var overloadManager = OverloadModeManager.Instance;
            if (overloadManager != null)
            {
                if (overloadManager.TryActivateOverloadMode())
                {
                    Debug.Log("过载模式已激活！");
                    UpdateOverloadModeButtonState();
                }
                else
                {
                    Debug.Log("无法激活过载模式：" + overloadManager.GetOverloadModeStatusInfo());
                }
            }
        }
        
        private void UpdateOverloadModeButtonState()
        {
            var overloadManager = OverloadModeManager.Instance;
            if (overloadManager == null) return;
            var canActivate = overloadManager.CanActivateOverloadMode();
            var isActive = overloadManager.IsOverloadModeActive;
                
            Find<Button>("Background/OverloadMode_Btn").interactable = canActivate && !isActive;
                
            if (isActive)
            {
                Find<Image>("Background/OverloadMode_Btn").sprite = overloadModeActiveSprite;
            }
            else
            {
                Find<Image>("Background/OverloadMode_Btn").sprite = overloadModeInactiveSprite;
            }
        }

        private void UpdateBloodBar()
        {
            
        }

        private void UpdateEnergyBar(int energy)
        {
            for (var i = 0; i < energyImages.Count; i++)
            {
                if (i <= energy - 1)
                {
                    if (i == 0)
                        energyImages[i].sprite = lEnergyBright;
                    energyImages[i].sprite = energyBright;
                }
                else
                {
                    if (i == 0)
                        energyImages[i].sprite = lEnergyDark;
                    energyImages[i].sprite = energyDark;
                }
            }
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();
            UpdateOverloadModeButtonState();
        }

        protected void OnDestroy()
        {
            MessageCenter.Unsubscribe(Defines.PlayerTurnStartEvent, OnPlayerTurnStart);
            MessageCenter.Unsubscribe(Defines.PlayerTurnEndEvent, OnPlayerTurnEnd);
            MessageCenter.Unsubscribe(Defines.EnemyTurnStartEvent, OnEnemyTurnStart);
            MessageCenter.Unsubscribe(Defines.EnemyTurnEndEvent, OnEnemyTurnEnd);
            MessageCenter.Unsubscribe(Defines.DeploymentStateEndedEvent, OnDeploymentStateEnded);
            MessageCenter.Unsubscribe(Defines.SkillHoverEnterEvent, OnSkillHoverEnter);
            MessageCenter.Unsubscribe(Defines.SkillHoverExitEvent, OnSkillHoverExit);
        }

        private void OnPlayerTurnStart(object[] args)
        {
            Find<Button>("Background/EndTurn_Btn").interactable = true;
            UpdateStageText(GameState.PlayerTurn);
            UpdateHintText(GameState.PlayerTurn);
        }

        private void OnPlayerTurnEnd(object[] args)
        {
            Find<Button>("Background/EndTurn_Btn").interactable = false;
        }
        
        private void OnEnemyTurnStart(object[] args)
        {
            UpdateStageText(GameState.EnemyTurn);
            UpdateHintText(GameState.EnemyTurn);
        }
        
        private void OnEnemyTurnEnd(object[] args)
        {
            // 敌人回合结束，准备进入玩家回合
        }
        
        private void OnDeploymentStateEnded(object[] args)
        {
            // 部署阶段结束，准备进入敌人回合
            UpdateHintText(GameState.EnemyTurn);
        }
        
        /// <summary>
        /// 更新阶段文本显示（带淡入淡出效果）
        /// </summary>
        /// <param name="gameState">当前游戏状态</param>
        private void UpdateStageText(GameState gameState)
        {
            if (stageText == null) return;
            
            // 如果已有淡入淡出协程在运行，先停止它
            if (stageTextFadeCoroutine != null)
            {
                StopCoroutine(stageTextFadeCoroutine);
            }
            
            // 启动新的淡入淡出效果
            stageTextFadeCoroutine = StartCoroutine(FadeStageText(gameState));
        }
        
        /// <summary>
        /// 阶段文本淡入淡出效果协程
        /// </summary>
        /// <param name="gameState">目标游戏状态</param>
        /// <returns></returns>
        private IEnumerator FadeStageText(GameState gameState)
        {
            // 获取目标文本和颜色
            string targetText;
            Color targetColor;
            
            switch (gameState)
            {
                case GameState.Deployment:
                    targetText = "部署阶段";
                    targetColor = Color.white;
                    break;
                case GameState.EnemyTurn:
                    targetText = "敌人回合";
                    targetColor = Color.red;
                    break;
                case GameState.PlayerTurn:
                    targetText = "玩家回合";
                    targetColor = Color.white;
                    break;
                case GameState.GameOver:
                    targetText = "游戏结束";
                    targetColor = Color.white;
                    break;
                default:
                    targetText = "未知阶段";
                    targetColor = Color.white;
                    break;
            }
            
            // 淡出效果
            Color currentColor = stageText.color;
            float elapsedTime = 0f;
            
            while (elapsedTime < fadeOutDuration)
            {
                elapsedTime += Time.deltaTime;
                float alpha = Mathf.Lerp(currentColor.a, 0f, elapsedTime / fadeOutDuration);
                stageText.color = new Color(currentColor.r, currentColor.g, currentColor.b, alpha);
                yield return null;
            }
            
            // 确保完全透明
            stageText.color = new Color(currentColor.r, currentColor.g, currentColor.b, 0f);
            
            // 更新文本内容
            stageText.text = targetText;
            
            // 淡入效果
            elapsedTime = 0f;
            while (elapsedTime < fadeInDuration)
            {
                elapsedTime += Time.deltaTime;
                float alpha = Mathf.Lerp(0f, targetColor.a, elapsedTime / fadeInDuration);
                stageText.color = new Color(targetColor.r, targetColor.g, targetColor.b, alpha);
                yield return null;
            }
            
            // 确保最终颜色正确
            stageText.color = targetColor;
            
            // 清除协程引用
            stageTextFadeCoroutine = null;
        }

        /// <summary>
        /// 更新引导提示文本（HintText），根据不同阶段显示不同内容
        /// </summary>
        /// <param name="gameState">当前游戏阶段</param>
private void UpdateHintText(GameState gameState)
{
    if (hintText == null) return;
    if (hintTextFadeCoroutine != null)
    {
        StopCoroutine(hintTextFadeCoroutine);
    }
    hintTextFadeCoroutine = StartCoroutine(FadeHintText(gameState));
}

        /// <summary>
        /// 引导提示文本淡入淡出效果协程
        /// </summary>
        /// <param name="gameState">当前游戏阶段</param>
        /// <returns></returns>
        private IEnumerator FadeHintText(GameState gameState)
        {
            // 目标文本与颜色
            string targetText;
            Color targetColor;

            switch (gameState)
            {
                case GameState.Deployment:
                    targetText = "点击下方的角色卡片，然后点击场上的一个绿色格子进行部署";
                    targetColor = Color.white;
                    break;
                case GameState.EnemyTurn:
                    targetText = "敌人行动中……";
                    targetColor = Color.red; // 与阶段文本一致，突出敌人回合
                    break;
                case GameState.PlayerTurn:
                    targetText = "点击一个我方角色，点击一个绿色格子进行移动，或者选择左下角的两个技能之一进行释放";
                    targetColor = Color.white;
                    break;
                default:
                    targetText = string.Empty;
                    targetColor = Color.white;
                    break;
            }

            // 先淡出当前文本
            Color currentColor = hintText.color;
            float elapsed = 0f;
            while (elapsed < fadeOutDuration)
            {
                elapsed += Time.deltaTime;
                float alpha = Mathf.Lerp(currentColor.a, 0f, elapsed / fadeOutDuration);
                hintText.color = new Color(currentColor.r, currentColor.g, currentColor.b, alpha);
                yield return null;
            }

            // 清零并更新文本
            hintText.color = new Color(currentColor.r, currentColor.g, currentColor.b, 0f);
            hintText.text = targetText;

            // 再淡入到目标颜色
            elapsed = 0f;
            while (elapsed < fadeInDuration)
            {
                elapsed += Time.deltaTime;
                float alpha = Mathf.Lerp(0f, targetColor.a, elapsed / fadeInDuration);
                hintText.color = new Color(targetColor.r, targetColor.g, targetColor.b, alpha);
                yield return null;
            }

            // 最终颜色校准
            hintText.color = targetColor;
            hintTextFadeCoroutine = null;
        }

        private void OnSkillHoverEnter(object[] args)
        {
            if (skillText == null) return;
            if (args == null || args.Length == 0) { skillText.text = string.Empty; return; }
            if (args[0] is SkillDataSO skill)
            {
                skillText.text = GetSkillHoverText(skill);
            }
            else
            {
                skillText.text = string.Empty;
            }
        }

        private void OnSkillHoverExit(object[] args)
        {
            if (skillText == null) return;
            skillText.text = string.Empty;
        }

        private string GetSkillHoverText(SkillDataSO skill)
        {
            if (skill == null) return string.Empty;

            // 优先根据技能ID匹配，其次回退到技能名称包含关键字
            switch (skill.skillID)
            {
                case "breakpoint_execution_01":
                    return "断点斩杀：“选择一个距离为1的敌人，对其造成2点伤害”";
                case "flashback_displacement_01":
                    return "闪回位移：“瞬移到上次移动前的位置，留下虚影抵挡伤害”";
                case "stack_shield_01":
                    return "堆栈护盾：“为一个我方角色提供护盾，吸收一次伤害”";
                case "terrain_deployment_01":
                    return "地形投放：“创建一个自己的虚影抵挡伤害”";
                case "forced_migration_01":
                    return "强制迁移：“将一个敌方角色移动到自身位置并对其造成1点伤害，自身后退一步”";
                case "position_swap_01":
                    return "移形换影：“选择两个角色，交换其位置”";
            }

            var name = skill.skillName ?? string.Empty;
            if (name.Contains("断点"))
                return "断点斩杀：“选择一个距离为1的敌人，对其造成2点伤害”";
            if (name.Contains("闪回") || name.Contains("Flashback"))
                return "闪回位移：“瞬移到上次移动前的位置，留下虚影抵挡伤害”";
            if (name.Contains("堆栈") || name.Contains("护盾"))
                return "堆栈护盾：“为一个我方角色提供护盾，吸收一次伤害”";
            if (name.Contains("地形") || name.Contains("投放"))
                return "地形投放：“创建一个自己的虚影抵挡伤害”";
            if (name.Contains("强制") || name.Contains("迁移") || name.Contains("位移"))
                return "强制迁移：“将一个敌方角色移动到自身位置并对其造成1点伤害，自身后退一步”";
            if (name.Contains("移形换影") || name.Contains("交换") || name.Contains("PositionSwap"))
                return "移形换影：“选择两个角色，交换其位置”";

            return string.Empty;
        }
    

private IEnumerator Level2HintRoutine()
{
    level2HintActive = true;
    level2TipText.gameObject.SetActive(true);
    level2TipText.alpha = 1f;
    var color = level2TipText.color;
    level2TipText.color = new Color(color.r, color.g, color.b, 1f);
    if (level2TipBackground != null) level2TipBackground.gameObject.SetActive(true);
    float elapsed = 0f;
    while (elapsed < level2HintDuration)
    {
        elapsed += Time.deltaTime;
        yield return null;
    }
    level2TipText.gameObject.SetActive(false);
    if (level2TipBackground != null) level2TipBackground.gameObject.SetActive(false);
    level2HintActive = false;
}


private void ShowLevel2HintIfApplicable()
{
    int levelIndex = LevelManager.Instance != null ? LevelManager.Instance.GetCurrentLevelIndex() : 1;
    if (level2TipText == null)
    {
        level2TipText = Find<TextMeshProUGUI>("Background/第二关提示");
        if (level2TipText == null) level2TipText = Find<TextMeshProUGUI>("第二关提示");
    }
    if (level2TipBackground == null)
    {
        level2TipBackground = Find<Image>("Background/第二关提示背景");
        if (level2TipBackground == null) level2TipBackground = Find<Image>("第二关提示背景");
    }
    if (level2TipCloseButton == null)
    {
        level2TipCloseButton = Find<Button>("Background/第二关提示关闭按钮");
        if (level2TipCloseButton == null) level2TipCloseButton = Find<Button>("第二关提示关闭按钮");
    }

    if (levelIndex != 2)
    {
        if (level2TipText != null) level2TipText.gameObject.SetActive(false);
        if (level2TipBackground != null) level2TipBackground.gameObject.SetActive(false);
        if (level2TipCloseButton != null) level2TipCloseButton.gameObject.SetActive(false);
        return;
    }

    if (level2TipText == null) return;

    level2TipText.gameObject.SetActive(true);
    var c = level2TipText.color;
    level2TipText.color = new Color(c.r, c.g, c.b, 1f);
    #if TMP_PRESENT
    level2TipText.alpha = 1f;
    #endif
    var cg = level2TipText.GetComponent<CanvasGroup>();
    if (cg != null) cg.alpha = 1f;

    if (level2TipBackground != null) level2TipBackground.gameObject.SetActive(true);
    if (level2TipCloseButton != null) level2TipCloseButton.gameObject.SetActive(true);

    level2HintActive = true;
    // 不再启动30秒协程，提示保持显示，直到用户操作关闭
}


private void EnsureLevel2TipBackground()
{
    if (level2TipText == null) return;
    if (level2TipBackground != null) return;
    var go = new GameObject("第二关提示底");
    go.transform.SetParent(level2TipText.transform, false);
    level2TipBackground = go.AddComponent<Image>();
    level2TipBackground.color = new Color(0f, 0f, 0f, 0.5f);
    level2TipBackground.raycastTarget = false;
    var bgRt = level2TipBackground.rectTransform;
    bgRt.anchorMin = new Vector2(0f, 0f);
    bgRt.anchorMax = new Vector2(1f, 1f);
    bgRt.offsetMin = new Vector2(-8f, -4f);
    bgRt.offsetMax = new Vector2(8f, 4f);
    go.transform.SetAsFirstSibling();
}
}
}
