using Level;
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
    }
}