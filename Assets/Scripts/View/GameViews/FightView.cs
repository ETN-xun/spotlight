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
        }

        protected override void InitData()
        {
            base.InitData();
            _levelData = LevelManager.Instance.GetCurrentLevel();
        }

        public override void Open(params object[] args)
        {
            base.Open(args);
            Find<Button>("Background/Settings_Btn").onClick.AddListener(OnClickSettingsButton);
            Find<Button>("Background/PlayerList/Player_01").onClick.AddListener(() =>
            {
                OnClickPlayerButton(0);
            });
            Find<Button>("Background/PlayerList/Player_02").onClick.AddListener(() =>
            {
                OnClickPlayerButton(1);
            });
            Find<Button>("Background/PlayerList/Player_03").onClick.AddListener(() =>
            {
                OnClickPlayerButton(2);
            });
            Find<Button>("Background/EndTurn_Btn").onClick.AddListener(OnClickEndTurnButton);
            
            InitializeOverloadModeButton();
        }
        
        public override void Close(params object[] args)
        {
            base.Close();
            Find<Button>("Background/Settings_Btn").onClick.RemoveListener(OnClickSettingsButton);
            Find<Button>("Background/PlayerList/Player_01").onClick.RemoveListener(() =>
            {
                OnClickPlayerButton(0);
            });
            Find<Button>("Background/PlayerList/Player_02").onClick.RemoveListener(() =>
            {
                OnClickPlayerButton(1);
            });
            Find<Button>("Background/PlayerList/Player_03").onClick.RemoveListener(() =>
            {
                OnClickPlayerButton(2);
            });
            Find<Button>("Background/EndTurn_Btn").onClick.RemoveListener(OnClickEndTurnButton);
        }

        private void OnClickSettingsButton()
        {
            
        }

        private void OnClickPlayerButton(int playerId = 0)
        {
            // 还应该有选中等的逻辑
            // if (ViewManager.Instance.IsOpen(ViewType.UnitInfoView))
            // {
            //     ViewManager.Instance.CloseView(ViewType.UnitInfoView);
            // }
            // else
            // {
            //     ViewManager.Instance.OpenView(ViewType.UnitInfoView, "" ,_levelData.playerUnits[playerId]);
            // }
        }
        
        private void OnClickEndTurnButton()
        {
            Debug.Log("结束回合按钮被点击");
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

        protected override void OnUpdate()
        {
            base.OnUpdate();
            UpdateOverloadModeButtonState();
        }
    }
}