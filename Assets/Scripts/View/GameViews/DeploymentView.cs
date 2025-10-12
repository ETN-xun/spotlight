using System.Collections.Generic;
using System.Linq;
using Level;
using UnityEngine;
using View.Base;

namespace View.GameViews
{
    public class DeploymentView : BaseView
    {
        private readonly List<UnitDataSO> _playerUnitsData = new List<UnitDataSO>();

        protected override void InitView()
        {
            base.InitView();
            ViewManager.Instance.RegisterView(ViewType.UnitDeploymentView, new ViewInfo()
            {
                PrefabName = "UnitDeploymentView",
                ParentTransform = transform.Find("Background")
            });
        }

        protected override void InitData()
        {
            base.InitData();
            var playerUnits = LevelManager.Instance.GetCurrentLevel().playerUnits;
            foreach (var unit in playerUnits.Where(unit => unit is not null && unit.data is not null))
            {
                _playerUnitsData.Add(unit.data);
            }
        }
        
        public override void Open(params object[] args)
        {
            base.Open(args);
            foreach (var unitData in _playerUnitsData)
            {
                ViewManager.Instance.OpenView(ViewType.UnitDeploymentView, unitData.unitID, unitData);
            }
        }

        public override void Close(params object[] args)
        {
            base.Close(args);
           foreach (var unitData in _playerUnitsData)
           {
                ViewManager.Instance.CloseView(ViewType.UnitDeploymentView, unitData.unitID, unitData);
           }
        }
    }
}