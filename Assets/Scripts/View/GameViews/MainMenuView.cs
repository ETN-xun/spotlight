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
        

        private void OnNewGameBtnClick()
        {
            SceneLoadManager.Instance.LoadScene(SceneType.TestScene);
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