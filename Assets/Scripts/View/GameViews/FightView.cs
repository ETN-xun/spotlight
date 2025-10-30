using System;
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
        }
        

        public override void Open(params object[] args)
        {
            base.Open(args);
            MessageCenter.Subscribe(Defines.EnergyChangedEvent, OnEnergyChanged);
            MessageCenter.Subscribe(Defines.UnitTakeDamageEvent, OnUnitTakeDamage);
            Find<Button>("Background/Settings_Btn").onClick.AddListener(OnClickSettingsButton);
            Find<Button>("Background/EndTurn_Btn").onClick.AddListener(OnClickEndTurnButton);
            Find<Button>("Background/PopupSettings/ContineGame").onClick.AddListener(OnClickContinueGameButton);
            Find<Button>("Background/PopupSettings/ReturnMainMenu").onClick.AddListener(OnClickReturnMainMenuButton);
            Find<Button>("Background/PopupSettings/SkipFight").onClick.AddListener(OnClickSkipFightButton);
            Find<TextMeshProUGUI>("Background/TurnTarget/Text (TMP)").text = LevelManager.Instance.GetCurrentLevel().levelTarget;
            UpdateEnergyBar(ActionManager.EnergySystem.GetCurrentEnergy());
            popupSettingsView.SetActive(false);
            
            Find<Button>("Background/EndTurn_Btn").interactable = false;
            
            // InitializeOverloadModeButton();
        }

        private void OnClickSkipFightButton()
        {
            GameManager.Instance.ChangeGameState(GameState.GameOver);
        }

        public override void Close(params object[] args)
        {
            base.Close();
            Debug.Log("Close");
            MessageCenter.Unsubscribe(Defines.EnergyChangedEvent, OnEnergyChanged);
            MessageCenter.Unsubscribe(Defines.UnitTakeDamageEvent, OnUnitTakeDamage);
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
        }

        private void OnPlayerTurnStart(object[] args)
        {
            Find<Button>("Background/EndTurn_Btn").interactable = true;
        }

        private void OnPlayerTurnEnd(object[] args)
        {
            Find<Button>("Background/EndTurn_Btn").interactable = false;
        }
    }
}