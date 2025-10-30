using System;
using Scene;
using UnityEngine;
using View;
using Object = UnityEngine.Object;

/// <summary>
/// 控制整个游戏流程的管理器
/// </summary>
public class GameAppManager : MonoBehaviour
{
    
    // [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    // static void LoadGameAppManager()
    // {
    //     if (FindObjectOfType<GameAppManager>() != null) return;
    //     var prefab = Resources.Load<GameObject>("Prefab/GameAppManager");
    //     if (prefab != null)
    //         Instantiate(prefab);
    //     else
    //         new GameObject("GameAppManager").AddComponent<GameAppManager>();
    // }
    
    private void Awake()
    {
        if (FindObjectsOfType<GameAppManager>().Length > 1)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        
        
    }

    private void Start()
    {
        RegisterViews();
    }
    
    private void Update()
    {
        
    }

    public static void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void RegisterViews()
    {
        ViewManager.Instance.RegisterView(ViewType.TestView, new ViewInfo()
        {
            PrefabName = "TestView",
            SortingOrder = 0,
            ParentTransform = transform.Find("ViewManager")
        });
            
        ViewManager.Instance.RegisterView(ViewType.DeploymentView, new ViewInfo()
        {
            PrefabName = "DeploymentView",
            SortingOrder = 2,
            ParentTransform = transform.Find("ViewManager")
        });

        ViewManager.Instance.RegisterView(ViewType.MainMenuView, new ViewInfo()
        {
            PrefabName = "MainMenuView",
            SortingOrder = 999,
            ParentTransform = transform.Find("ViewManager")
        });

        ViewManager.Instance.RegisterView(ViewType.FightView, new ViewInfo()
        {
            PrefabName = "FightView",
            ParentTransform = transform.Find("ViewManager")
        });
        
        ViewManager.Instance.RegisterView(ViewType.LevelSelectView, new ViewInfo()
        {
            PrefabName = "LevelSelectView",
            ParentTransform = transform.Find("ViewManager")
        });
        
    }
}