using System.Collections.Generic;
using Common;
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

            _skillsData.Clear();
            foreach (var skill in unit.data.skills)
                _skillsData.Add(skill);
            
            switch (unit.data.unitType)
            { 
                case UnitType.Shadow:
                    gameObject.GetComponent<Image>().sprite = 影Icon;
                    Find<Image>("SkillView/SkillSprite").sprite = _skillsData[0].skillIcon;
                    Find<Image>("SkillView (1)/SkillSprite").sprite = _skillsData[1].skillIcon;
                    break;
                case UnitType.Stone:
                    gameObject.GetComponent<Image>().sprite = 石Icon;
                    Find<Image>("SkillView/SkillSprite").sprite = _skillsData[0].skillIcon;
                    Find<Image>("SkillView (1)/SkillSprite").sprite = _skillsData[1].skillIcon;
                    break;
                case UnitType.Zero:
                    gameObject.GetComponent<Image>().sprite = 零Icon;
                    Find<Image>("SkillView/SkillSprite").sprite = _skillsData[0].skillIcon;
                    Find<Image>("SkillView (1)/SkillSprite").sprite = _skillsData[1].skillIcon;
                    break;
            }
            
            Find<Button>("SkillView").onClick.AddListener(OnClickSkillView);
            Find<Button>("SkillView (1)").onClick.AddListener(OnClickSkillView1);
        }

        private void OnClickSkillView()
        {
            MessageCenter.Publish(Defines.ClickSkillViewEvent, _skillsData[0]);
        }

        private void OnClickSkillView1()
        {
            MessageCenter.Publish(Defines.ClickSkillViewEvent, _skillsData[1]);
        }
    }
}