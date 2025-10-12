using System;
using UnityEngine;

namespace View
{
    public sealed class ViewInfo       // 后面可能考虑 替换为 DataSO
    {
        public string PrefabName;
        public int SortingOrder;
        public Transform ParentTransform;
    }

    public enum ViewType
    {
        SceneLoadingView,
        UnitInfoView,
        TestView,
        DeploymentView,
        UnitDeploymentView,
        MainMenuView,
        LevelSelectView,
        LevelView,
        SkillView,
        SkillSelectView,
    }

}