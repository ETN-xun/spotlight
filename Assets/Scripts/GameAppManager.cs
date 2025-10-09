using System;
using Scene;
using UnityEngine;
using View;


public class GameAppManager : MonoBehaviour
{
    private void Awake()
    {
        if (FindObjectsOfType<GameAppManager>().Length > 1)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        
        RegisterViews();
    }

    private void Start()
    {
#if !UNITY_EDITOR
        SceneLoadManager.Instance.LoadScene(SceneType.MainMenu);
#endif
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
            ParentTransform = transform
        });

        ViewManager.Instance.RegisterView(ViewType.UnitInfoView, new ViewInfo()
        {
            PrefabName = "UnitInfoView",
            SortingOrder = 1,
            ParentTransform = transform
        });
            
        ViewManager.Instance.RegisterView(ViewType.DeploymentView, new ViewInfo()
        {
            PrefabName = "DeploymentView",
            SortingOrder = 2,
            ParentTransform = transform
        });

        ViewManager.Instance.RegisterView(ViewType.MainMenuView, new ViewInfo()
        {
            PrefabName = "MainMenuView",
            SortingOrder = 999,
            ParentTransform = transform
        });
    }
}