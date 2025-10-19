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
            Find<UnityEngine.UI.Button>("Background/NewGameBtn").onClick.AddListener(OnNewGameBtnClick);
            Find<UnityEngine.UI.Button>("Background/ExitGameBtn").onClick.AddListener(OnExitGameBtnClick);
            Find<UnityEngine.UI.Button>("Background/LastGameBtn").onClick.AddListener(OnLastGameBtnClick);
        }
        public override void Destroy()      // TODO: 正确地移除监听器
        {
            base.Destroy();
            Find<UnityEngine.UI.Button>("Background/NewGameBtn").onClick.RemoveListener(OnNewGameBtnClick);
            Find<UnityEngine.UI.Button>("Background/ExitGameBtn").onClick.RemoveListener(OnExitGameBtnClick);
            Find<UnityEngine.UI.Button>("Background/LastGameBtn").onClick.RemoveListener(OnLastGameBtnClick);
        }

        private void OnNewGameBtnClick()
        {
            SceneLoadManager.Instance.LoadScene(SceneType.LevelSelect);
        }

        private void OnExitGameBtnClick()
        {
            GameAppManager.ExitGame();
        }

        private void OnLastGameBtnClick()
        {
            
        }
    }
}