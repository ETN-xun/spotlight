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
    private readonly List<GraphicRaycaster> _raycasters = new List<GraphicRaycaster>();
    private GraphicRaycaster _primaryRaycaster;

    private void Start()
    {
        UnlockProgressStorage.Initialize();

        var buttonsRoot = GameObject.Find("Buttons");
        if (buttonsRoot != null)
        {
            _levelButtons.AddRange(buttonsRoot.GetComponentsInChildren<Button>(true));
            _primaryRaycaster = buttonsRoot.GetComponentInParent<GraphicRaycaster>();
        }

        var raycastersInScene = FindObjectsOfType<GraphicRaycaster>();
        if (raycastersInScene != null && raycastersInScene.Length > 0)
        {
            _raycasters.AddRange(raycastersInScene);
        }

        ApplyUnlockStateToButtons();
    }

    private void ApplyUnlockStateToButtons()
    {
        int unlocked = UnlockProgressStorage.LoadUnlockedLevels();
        for (int i = 0; i < _levelButtons.Count; i++)
        {
            bool isUnlocked = (i + 1) <= unlocked;
            _levelButtons[i].interactable = isUnlocked;
        }
    }

    public void ChangeScene()
    {
        int levelIndex = ResolveClickedLevelIndex();

        var levelData = Level.LevelManager.Instance != null
            ? Level.LevelManager.Instance.GetLevelDataByIndex(levelIndex)
            : null;
        // 构建版偶现：若数据尚未初始化导致取不到关卡数据，强制初始化一次再重试
        if (levelData == null)
        {
            var dm = DataManager.Instance;
            if (dm != null)
            {
                dm.InitializeData();
                levelData = LevelManager.Instance.GetLevelDataByIndex(levelIndex);
            }
        }
        if (levelData != null)
        {
            LevelManager.Instance.SetCurrentLevel(levelData);
        }

        SceneLoadManager.Instance.LoadScene(SceneType.Demo);
    }

    private int ResolveClickedLevelIndex()
    {
        GameObject selected = EventSystem.current != null ? EventSystem.current.currentSelectedGameObject : null;
        Button clickedButton = null;
        if (selected != null)
        {
            clickedButton = selected.GetComponent<Button>() ?? selected.GetComponentInParent<Button>();
        }

        if (clickedButton == null && EventSystem.current != null)
        {
            Vector2 pos;
            if (Input.touchCount > 0)
            {
                pos = Input.GetTouch(0).position;
            }
            else
            {
                pos = Input.mousePosition;
            }

            var eventData = new PointerEventData(EventSystem.current)
            {
                position = pos
            };

            var results = new List<RaycastResult>();
            if (_primaryRaycaster != null)
            {
                _primaryRaycaster.Raycast(eventData, results);
                for (int i = 0; i < results.Count; i++)
                {
                    var r = results[i];
                    clickedButton = r.gameObject.GetComponent<Button>() ?? r.gameObject.GetComponentInParent<Button>();
                    if (clickedButton != null)
                    {
                        break;
                    }
                }
            }
            // 兜底：若主画布未找到按钮，尝试其他 GraphicRaycaster（与旧逻辑一致）
            if (clickedButton == null)
            {
                foreach (var gr in _raycasters)
                {
                    results.Clear();
                    gr.Raycast(eventData, results);
                    if (results.Count > 0)
                    {
                        foreach (var r in results)
                        {
                            clickedButton = r.gameObject.GetComponent<Button>() ?? r.gameObject.GetComponentInParent<Button>();
                            if (clickedButton != null)
                            {
                                break;
                            }
                        }
                    }
                    if (clickedButton != null) break;
                }
            }
        }

        if (clickedButton != null)
        {
            int idx = _levelButtons.IndexOf(clickedButton);
            if (idx >= 0)
            {
                return idx + 1;
            }
            return clickedButton.transform.GetSiblingIndex() + 1;
        }

        return 1;
    }
}
