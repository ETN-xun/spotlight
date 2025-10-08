using TMPro;
using UnityEngine.UI;
using View.Base;

namespace View.GameViews
{
    public class UnitInfoView : BaseView
    {
        public override void Open(params object[] args)
        {
            base.Open(args);
            // 仅供测试
            if (args[0] is not Unit unit) return;
            Find<TextMeshProUGUI>("UnitInfo/Name").text = "Name: " + unit.data.unitName;
            Find<TextMeshProUGUI>("UnitInfo/Health").text = "Health: " + unit.currentHP;
        }
    }
}