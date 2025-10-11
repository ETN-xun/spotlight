using System.Collections.Generic;
using Level;
using UnityEngine.UI;
using View.Base;

namespace View.GameViews
{
    public class LevelSelectView : BaseView
    {
        private List<LevelDataSO> _allLevelData = new List<LevelDataSO>();
        
        protected override void OnStart()
        {
            base.OnStart();
            ViewManager.Instance.RegisterView(ViewType.LevelView, new ViewInfo()
            {
                PrefabName = "LevelView",
                ParentTransform = transform.Find("Background")
            });
            _allLevelData = DataManager.Instance.allLevelData;
            foreach (var levelData in _allLevelData)
            {
                ViewManager.Instance.OpenView(ViewType.LevelView, levelData.levelId, levelData);
            }
        }
        
        // public override void Close(params object[] args)
        // {
        //     base.Close(args);
        //     foreach (var levelData in _allLevelData)
        //     {
        //         ViewManager.Instance.CloseView(ViewType.UnitDeploymentView, levelData.levelId, levelData);
        //     }
        // }
    }
}