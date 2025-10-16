using Common;
using TMPro;
using UnityEngine.UI;
using View.Base;

namespace View.GameViews
{
    public class UnitInfoView : BaseView
    {
        protected override void InitView()
        {
            base.InitView();
            ViewManager.Instance.RegisterView(ViewType.SkillSelectView, new ViewInfo()
            {
                PrefabName = "SkillSelectView",
                ParentTransform = transform
            });
        }

        public override void Open(params object[] args)
        {
            base.Open(args);
            if (args[0] is not Unit unit) return;
            Find<TextMeshProUGUI>("UnitName").text = unit.data.unitName;
            Find<Image>("UnitIcon").sprite = unit.data.unitSprite;
            if (unit.data.isEnemy) return;
            ViewManager.Instance.OpenView(ViewType.SkillSelectView, "", unit);
        }

        public override void Close(params object[] args)
        {
            base.Close(args);
            if (ViewManager.Instance.IsOpen(ViewType.SkillSelectView))
                ViewManager.Instance.CloseView(ViewType.SkillSelectView);
        }
    }
}