using System.Collections.Generic;
using UnityEngine;
using View.Base;

namespace View.GameViews
{
    public class DeploymentView : BaseView
    {
        private List<UnitDataSO> _playerUnitsData = new List<UnitDataSO>();

        protected override void InitView()
        {
            base.InitView();
            ViewManager.Instance.RegisterView(ViewType.UnitDeploymentView, new ViewInfo()
            {
                PrefabName = "UnitDeploymentView",
                SortingOrder = 5,
                ParentTransform = transform.Find("Background")
            });
        }

        protected override void InitData()
        {
            base.InitData();
            _playerUnitsData = DataManager.Instance.GetPlayerUnits();
        }
        
        public override void Open(params object[] args)
        {
            base.Open(args);
            foreach (var unitData in _playerUnitsData)
            {
                ViewManager.Instance.OpenView(ViewType.UnitDeploymentView, int.Parse(unitData.unitID), unitData);
            }
        }

        public override void Close(params object[] args)
        {
            base.Close(args);
           foreach (var unitData in _playerUnitsData)
           {
                ViewManager.Instance.CloseView(ViewType.UnitDeploymentView, int.Parse(unitData.unitID), unitData);
           }
        }
    }
}