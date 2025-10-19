using Level;
using Scene;
using TMPro;
using UnityEngine.UI;
using View.Base;

namespace View.GameViews
{
    public class LevelView : BaseView
    {
        public override void Open(params object[] args)
        {
            base.Open(args);
            if (args[0] is not LevelDataSO levelData) return;

            GetComponent<Image>().sprite = levelData.previewSprite;
            Find<TextMeshProUGUI>("LevelName").text = $"Level_{levelData.levelId}";
            GetComponent<Button>().onClick.AddListener (() =>
            {
                OnSelectLevel(levelData);
            });
        }
        
        public override void Close(params object[] args)
        {
            base.Close(args);
            GetComponent<Button>().onClick.RemoveAllListeners();
        }

        private void OnSelectLevel(LevelDataSO levelData)
        {
            LevelManager.Instance.SetCurrentLevel(levelData);
            SceneLoadManager.Instance.LoadScene(SceneType.Demo);
        }
    }
}