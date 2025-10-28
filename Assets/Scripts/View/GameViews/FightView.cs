using System;
using System.Collections.Generic;
using Action;
using Common;
using Level;
using TMPro;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.UI;
using View.Base;

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
            Find<TextMeshProUGUI>("Background/TurnTarget/Text (TMP)").text = LevelManager.Instance.GetCurrentLevel().levelTarget;
            UpdateEnergyBar(ActionManager.EnergySystem.GetCurrentEnergy());
            
            Find<Button>("Background/EndTurn_Btn").interactable = false;
            
            InitializeOverloadModeButton();
        }
        
        public override void Close(params object[] args)
        {
            base.Close();
            MessageCenter.Unsubscribe(Defines.EnergyChangedEvent, OnEnergyChanged);
            MessageCenter.Unsubscribe(Defines.UnitTakeDamageEvent, OnUnitTakeDamage);
            Find<Button>("Background/Settings_Btn").onClick.RemoveListener(OnClickSettingsButton);
            Find<Button>("Background/EndTurn_Btn").onClick.RemoveListener(OnClickEndTurnButton);
        }
        
        private void OnEnergyChanged(object[] args)
        {
            if (args[0] is not int energy) return;
            UpdateEnergyBar(energy);
        }

        private void OnUnitTakeDamage(object[] args)
        {
            if (args[0] is not string unitId) return;   //根据unit id从ally_manager 或 enemy_manager 获取对应单位的信息进行显示更新
            var ally = Ally.AllyManager.Instance.GetAliveAllyByID(unitId);
            switch (ally.data.unitType)
            {
                case UnitType.Zero:
                    // Find<Image>("Background/PlayerLists/Zero").sprite = 
                    break;
                case UnitType.Shadow:
                    break;
                case UnitType.Stone:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void OnClickSettingsButton()
        {
            
        }
        
        private void OnClickEndTurnButton()
        {
            GameManager.Instance.ChangeGameState(GameState.EnemyTurn);
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