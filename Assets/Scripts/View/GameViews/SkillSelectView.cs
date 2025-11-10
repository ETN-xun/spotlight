using System.Collections.Generic;
using Common;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
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
        [SerializeField] private Sprite 闪回位移Icon;
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
                    Find<Image>("SkillView/SkillSprite").sprite = 断点斩杀Icon;
                    Find<Image>("SkillView (1)/SkillSprite").sprite = 闪回位移Icon;
                    break;
                case UnitType.Stone:
                    gameObject.GetComponent<Image>().sprite = 石Icon;
                    Find<Image>("SkillView/SkillSprite").sprite = 堆栈护盾Icon;
                    Find<Image>("SkillView (1)/SkillSprite").sprite = 地形投放Icon;
                    break;
                case UnitType.Zero:
                    gameObject.GetComponent<Image>().sprite = 零Icon;
                    Find<Image>("SkillView/SkillSprite").sprite = 强制位移Icon;
                    Find<Image>("SkillView (1)/SkillSprite").sprite = 移形换影Icon;
                    break;
            }
            
            Find<Button>("SkillView").onClick.AddListener(OnClickSkillView);
            Find<Button>("SkillView (1)").onClick.AddListener(OnClickSkillView1);

            // 绑定悬停提示事件（进入/离开）
            AttachHoverEvents("SkillView", _skillsData[0]);
            AttachHoverEvents("SkillView (1)", _skillsData[1]);
        }

        private void OnClickSkillView()
        {
            MessageCenter.Publish(Defines.ClickSkillViewEvent, _skillsData[0]);
        }

        private void OnClickSkillView1()
        {
            MessageCenter.Publish(Defines.ClickSkillViewEvent, _skillsData[1]);
        }

        private void AttachHoverEvents(string path, SkillDataSO data)
        {
            var button = Find<Button>(path);
            if (button == null || data == null) return;

            var trigger = Find<EventTrigger>(path);
            if (trigger == null)
            {
                trigger = button.gameObject.AddComponent<EventTrigger>();
            }
            // 清理旧条目，避免重复添加
            if (trigger.triggers == null)
            {
                trigger.triggers = new System.Collections.Generic.List<EventTrigger.Entry>();
            }
            else
            {
                trigger.triggers.Clear();
            }

            AddEntry(trigger, EventTriggerType.PointerEnter, (e) =>
            {
                MessageCenter.Publish(Defines.SkillHoverEnterEvent, data);
            });

            AddEntry(trigger, EventTriggerType.PointerExit, (e) =>
            {
                MessageCenter.Publish(Defines.SkillHoverExitEvent);
            });
        }

        private void AddEntry(EventTrigger trigger, EventTriggerType type, UnityAction<BaseEventData> callback)
        {
            var entry = new EventTrigger.Entry { eventID = type, callback = new EventTrigger.TriggerEvent() };
            entry.callback.AddListener(callback);
            trigger.triggers.Add(entry);
        }
    }
}
