using Level;
using UnityEngine;
using UnityEngine.UI;
using View.Base;

namespace View.GameViews
{
    public class FightView : BaseView
    {
        private LevelDataSO _levelData;
        private Button _overloadModeButton;
        private Text _overloadModeButtonText;
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
            
            // 初始化过载模式按钮
            InitializeOverloadModeButton();
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
            
        }
        
        private void InitializeOverloadModeButton()
        {
            // 尝试找到过载模式按钮，如果UI中还没有，我们先用EndTurn按钮旁边的位置
            _overloadModeButton = Find<Button>("Background/OverloadMode_Btn");
            if (_overloadModeButton != null)
            {
                _overloadModeButton.onClick.AddListener(OnClickOverloadModeButton);
                _overloadModeButtonText = _overloadModeButton.GetComponentInChildren<Text>();
                UpdateOverloadModeButtonState();
            }
            else
            {
                Debug.LogWarning("过载模式按钮未找到，请在UI中添加 Background/OverloadMode_Btn");
            }
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
            if (_overloadModeButton == null || _overloadModeButtonText == null) return;
            
            var overloadManager = OverloadModeManager.Instance;
            if (overloadManager != null)
            {
                bool canActivate = overloadManager.CanActivateOverloadMode();
                bool isActive = overloadManager.IsOverloadModeActive;
                
                _overloadModeButton.interactable = canActivate && !isActive;
                
                if (isActive)
                {
                    _overloadModeButtonText.text = $"过载中 ({overloadManager.OverloadRemainingTurns})";
                }
                else if (canActivate)
                {
                    _overloadModeButtonText.text = "激活过载";
                }
                else
                {
                    _overloadModeButtonText.text = "能量不足";
                }
            }
        }
        
        private void Update()
        {
            // 每帧更新按钮状态
            UpdateOverloadModeButtonState();
        }
    }
}