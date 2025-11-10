using Common;
using TMPro;
using UnityEngine.EventSystems;
using View.Base;
using Common;

namespace View.GameViews
{
    public class SkillView : BaseView, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        private SkillDataSO _skillData;
        public override void Open(params object[] args)
        {
            base.Open(args);
            if (args[0] is not SkillDataSO skillData) return;
            _skillData = skillData;
            Find<TextMeshProUGUI>("SkillName").text = _skillData.skillName;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            MessageCenter.Publish(Defines.ClickSkillViewEvent, _skillData);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (_skillData != null)
            {
                MessageCenter.Publish(Defines.SkillHoverEnterEvent, _skillData);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            MessageCenter.Publish(Defines.SkillHoverExitEvent);
        }
    }
}
