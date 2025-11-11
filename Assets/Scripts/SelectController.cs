using System.Collections;
using System.Collections.Generic;
using Level;
using Scene;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectController : MonoBehaviour
{
    private List<Button> _levelButtons = new List<Button>();

    private void Start()
    {
        // 初始化解锁状态（默认仅解锁第一关）
        if (!PlayerPrefs.HasKey("UnlockedLevels"))
        {
            PlayerPrefs.SetInt("UnlockedLevels", 1);
            PlayerPrefs.Save();
        }

        // 收集并按层级顺序记录关卡按钮（假设顺序为1/2/3）
        var buttonsRoot = GameObject.Find("Buttons");
        if (buttonsRoot != null)
        {
            _levelButtons.AddRange(buttonsRoot.GetComponentsInChildren<Button>(true));
        }

        ApplyUnlockStateToButtons();
    }

    private void ApplyUnlockStateToButtons()
    {
        int unlocked = PlayerPrefs.GetInt("UnlockedLevels", 1);
        for (int i = 0; i < _levelButtons.Count; i++)
        {
            // 第 i+1 关是否解锁
            bool isUnlocked = (i + 1) <= unlocked;
            _levelButtons[i].interactable = isUnlocked;
        }
    }

    public void ChangeScene()
    {
        // 根据当前点击的按钮确定关卡索引（1/2/3）
        int levelIndex = 1;
        var selected = EventSystem.current != null ? EventSystem.current.currentSelectedGameObject : null;
        if (selected != null)
        {
            // 通过同级序号推断关卡序号（Buttons容器内的顺序与关卡顺序一致）
            levelIndex = selected.transform.GetSiblingIndex() + 1;
        }

        // 设置当前关卡数据（依赖 LevelManager/DataManager 单例）
        var levelId = levelIndex.ToString();
        var levelData = DataManager.Instance.GetLevelData(levelId);
        if (levelData != null)
        {
            LevelManager.Instance.SetCurrentLevel(levelData);
        }

        // 进入 Demo 场景，在该场景的 GameManager 中按选择的关卡启动剧情
        SceneLoadManager.Instance.LoadScene(SceneType.Demo);
    }
}
