using System;
using Scene;
using UnityEngine;
using View.Base;

namespace View.GameViews
{
    public class MainMenuView : BaseView
    {
        protected override void OnAwake()
        {
            base.OnAwake();
            Find<UnityEngine.UI.Button>("Background/Buttons/Continue").onClick.AddListener(OnContinueGameBtnClick);
            Find<UnityEngine.UI.Button>("Background/Buttons/NewGame").onClick.AddListener(OnNewGameBtnClick);
            Find<UnityEngine.UI.Button>("Background/Buttons/ExitGame").onClick.AddListener(OnExitGameBtnClick);
            Find<UnityEngine.UI.Button>("Background/Buttons/CreativeStaff").onClick.AddListener(OnContinueGameBtnClick);
            Find<UnityEngine.UI.Button>("Background/Settings").onClick.AddListener(OnSettingsBtnClick);
        }
        public override void Destroy()      
        {
            Find<UnityEngine.UI.Button>("Background/Buttons/Continue").onClick.RemoveListener(OnContinueGameBtnClick);
            Find<UnityEngine.UI.Button>("Background/Buttons/NewGame").onClick.RemoveListener(OnNewGameBtnClick);
            Find<UnityEngine.UI.Button>("Background/Buttons/ExitGame").onClick.RemoveListener(OnExitGameBtnClick);
            Find<UnityEngine.UI.Button>("Background/Buttons/CreativeStaff").onClick.RemoveListener(OnCreativeStaffBtnClick);
            Find<UnityEngine.UI.Button>("Background/Settings").onClick.RemoveListener(OnSettingsBtnClick);
            base.Destroy();
        }

        private void OnNewGameBtnClick()
        {
            SceneLoadManager.Instance.LoadScene(SceneType.LevelSelect);
        }

        private void OnExitGameBtnClick()
        {
            GameAppManager.ExitGame();
        }

        private void OnContinueGameBtnClick()
        {
            
        }
        
        private void OnCreativeStaffBtnClick()
        {
            
        }
        
        private void OnSettingsBtnClick()
        {
            
        }

        private void OnDestroy()
        {
            Destroy();
        }
    }
}