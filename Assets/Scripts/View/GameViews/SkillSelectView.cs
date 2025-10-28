using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using View.Base;

namespace View.GameViews
{
    public class SkillSelectView : BaseView
    {
        [SerializeField] private Sprite 零Icon;
        [SerializeField] private Sprite 影Icon;
        [SerializeField] private Sprite 石Icon;
        [SerializeField] private Sprite 强制位移Icon;
        [SerializeField] private Sprite 移形换影Icon;
        [SerializeField] private Sprite 堆栈护盾Icon;
        [SerializeField] private Sprite 断点斩杀Icon;
        [SerializeField] private Sprite 地形投放Icon;
        private readonly List<SkillDataSO> _skillsData = new();
        protected override void InitView()
        {
            base.InitView();
        }

        public override void Open(params object[] args)
        {
            base.Open(args);
            if (args[0] is not Unit unit) return;

            switch (unit.data.unitType)
            {
                case UnitType.Shadow:
                    gameObject.GetComponent<Image>().sprite = 影Icon;
                    Find<Image>("SkillView").sprite = 强制位移Icon;
                    Find<Image>("SkillView (1)").sprite = 断点斩杀Icon;
                    break;
                case UnitType.Stone:
                    gameObject.GetComponent<Image>().sprite = 石Icon;
                    Find<Image>("SkillView").sprite = 堆栈护盾Icon;
                    Find<Image>("SkillView (1)").sprite = 地形投放Icon;
                    break;
                case UnitType.Zero:
                    gameObject.GetComponent<Image>().sprite = 零Icon;
                    Find<Image>("SkillView").sprite = 强制位移Icon;
                    Find<Image>("SkillView (1)").sprite = 移形换影Icon;
                    break;
            }
        }

        public override void Close(params object[] args)
        {
            base.Close(args);
        }
    }
}