using Level;
using Scene;
using TMPro;
using UnityEngine;
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

            // 解锁 gating：根据 PlayerPrefs 中的解锁数量决定是否可点击
            bool isUnlocked = true;
            int unlocked = PlayerPrefs.GetInt("UnlockedLevels", 1);
            if (int.TryParse(levelData.levelId, out int idx))
            {
                isUnlocked = idx <= unlocked;
            }
            var btn = GetComponent<Button>();
            btn.interactable = isUnlocked;
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
            // 保险：若未解锁则不响应
            int unlocked = PlayerPrefs.GetInt("UnlockedLevels", 1);
            if (int.TryParse(levelData.levelId, out int idx) && idx > unlocked)
            {
                return;
            }
            LevelManager.Instance.SetCurrentLevel(levelData);
            SceneLoadManager.Instance.LoadScene(SceneType.Demo);
        }
    }
}
