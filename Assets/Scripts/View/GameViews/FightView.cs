using System;
using Common;
using Level;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using View.Base;

namespace View.GameViews
{
    public class FightView : BaseView
    {
        private LevelDataSO _levelData;
        protected override void InitView()
        {
            base.InitView();
            ViewManager.Instance.RegisterView(ViewType.UnitInfoView, new ViewInfo()
            {
                PrefabName = "UnitInfoView",
                ParentTransform = transform.Find("Background")
            });

            ViewManager.Instance.RegisterView(ViewType.TerrainInfoView, new ViewInfo()
            {
                PrefabName = "TerrainInfoView",
                ParentTransform = transform.Find("Background")
            });
        }

        protected override void InitData()
        {
            base.InitData();
            _levelData = LevelManager.Instance.GetCurrentLevel();
        }

        public override void Open(params object[] args)
        {
            base.Open(args);
            MessageCenter.Subscribe(Defines.EnergyChangedEvent, OnEnergyChanged);
            MessageCenter.Subscribe(Defines.UnitTakeDamageEvent, OnUnitTakeDamage);
            Find<Button>("Background/Settings_Btn").onClick.AddListener(OnClickSettingsButton);
            // Find<Button>("Background/PlayerList/Player_01").onClick.AddListener(() =>
            // {
            //     OnClickPlayerButton(0);
            // });
            // Find<Button>("Background/PlayerList/Player_02").onClick.AddListener(() =>
            // {
            //     OnClickPlayerButton(1);
            // });
            // Find<Button>("Background/PlayerList/Player_03").onClick.AddListener(() =>
            // {
            //     OnClickPlayerButton(2);
            // });
            Find<Button>("Background/EndTurn_Btn").onClick.AddListener(OnClickEndTurnButton);
            
            Find<TextMeshProUGUI>("Background/EnergyBar/Text").text = $"Current energy : {Action.ActionManager.EnergySystem.GetCurrentEnergy()}";
            
            InitializeOverloadModeButton();
        }
        
        public override void Close(params object[] args)
        {
            base.Close();
            MessageCenter.Unsubscribe(Defines.EnergyChangedEvent, OnEnergyChanged);
            MessageCenter.Unsubscribe(Defines.UnitTakeDamageEvent, OnUnitTakeDamage);
            Find<Button>("Background/Settings_Btn").onClick.RemoveListener(OnClickSettingsButton);
            // Find<Button>("Background/PlayerList/Player_01").onClick.RemoveListener(() =>
            // {
            //     OnClickPlayerButton(0);
            // });
            // Find<Button>("Background/PlayerList/Player_02").onClick.RemoveListener(() =>
            // {
            //     OnClickPlayerButton(1);
            // });
            // Find<Button>("Background/PlayerList/Player_03").onClick.RemoveListener(() =>
            // {
            //     OnClickPlayerButton(2);
            // });
            Find<Button>("Background/EndTurn_Btn").onClick.RemoveListener(OnClickEndTurnButton);
        }
        
        private void OnEnergyChanged(object[] args)
        {
            if (args[0] is not int energy) return;
            Find<TextMeshProUGUI>("Background/EnergyBar/Text").text = $"Current energy : {energy}";
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

        private void OnClickPlayerButton(int playerId = 0)
        {

        }
        
        private void OnClickEndTurnButton()
        {
            GameManager.Instance.ChangeGameState(GameState.EnemyTurn);
        }
        
        private void InitializeOverloadModeButton()
        {
            Find<Button>("Background/OverloadMode_Btn").onClick.AddListener(OnClickOverloadModeButton);
            Find<TextMeshProUGUI>("Background/OverloadMode_Btn/Text").text = "激活过载";
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
                Find<TextMeshProUGUI>("Background/OverloadMode_Btn/Text").text = $"ing ({overloadManager.OverloadRemainingTurns})";
            }
            else if (canActivate)
            {
                Find<TextMeshProUGUI>("Background/OverloadMode_Btn/Text").text = "active";
            }
            else
            {
                Find<TextMeshProUGUI>("Background/OverloadMode_Btn/Text").text = "not enough";
            }
        }

        private void UpdateBloodBar()
        {
            
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();
            UpdateOverloadModeButtonState();
        }
    }
}