using System.Collections.Generic;
using View.Base;

namespace View.GameViews
{
    public class SkillSelectView : BaseView
    {
        private List<SkillDataSO> _skillsData = new();
        protected override void InitView()
        {
            base.InitView();
            ViewManager.Instance.RegisterView(ViewType.SkillView, new ViewInfo()
            {
                PrefabName = "SkillView",
                ParentTransform = transform.Find("Background")
            });
        }

        public override void Open(params object[] args)
        {
            base.Open(args);
            if (args[0] is not Unit unit) return;
            
            foreach (var skill in unit.data.skills)
            {
                _skillsData.Add(skill);
                ViewManager.Instance.OpenView(ViewType.SkillView, skill.skillID, skill);
            }
        }

        public override void Close(params object[] args)
        {
            base.Close(args);

            foreach (var skill in _skillsData)
            {
                ViewManager.Instance.CloseView(ViewType.SkillView, skill.skillID);
            }
        }
    }
}