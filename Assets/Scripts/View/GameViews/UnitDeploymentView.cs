using System;
using Common;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using View.Base;

namespace View.GameViews
{
    public class UnitDeploymentView : BaseView, IPointerClickHandler
    {
        private UnitDataSO _unitData;
        public override void Open(params object[] args)
        {
            base.Open(args);
            if (args[0] is not UnitDataSO unitData) return;
            _unitData = unitData;
            Find<TextMeshProUGUI>("UnitName").text = _unitData.unitName;
            Find<Image>("UnitImage").sprite = _unitData.unitIcon;
            // 重置交互状态，避免缓存的禁用状态在重进关卡时导致按钮不可点击
            EnableViewClick();
        }
        

        public void OnPointerClick(PointerEventData eventData)
        {
            MessageCenter.Publish(Defines.ClickDeployUnitViewEvent,  _unitData);
        }

        public void DisableViewClick()
        {
            var image =  GetComponent<Image>();
            image.color = Color.gray;
            image.raycastTarget = false;
        }

        public void EnableViewClick()
        {
            var image =  GetComponent<Image>();
            image.color = Color.white;
            image.raycastTarget = true;
        }
    }
}
