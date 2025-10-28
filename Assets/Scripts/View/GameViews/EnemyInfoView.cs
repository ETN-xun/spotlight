using Common;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using View.Base;

namespace View.GameViews
{
    public class EnemyInfoView : BaseView
    {
        [SerializeField] private Sprite 死机亡灵Icon;
        [SerializeField] private Sprite 指针操纵者Icon;
        [SerializeField] private Sprite BossIcon;
        [SerializeField] private Sprite 乱码爬虫Icon;
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
                case UnitType.CrashUndead:
                    Find<TextMeshProUGUI>("EnemyName").text = "死机亡灵";
                    Find<Image>("EnemySprite").sprite = 死机亡灵Icon;
                    break;
                case UnitType.GarbledCrawler:
                    Find<TextMeshProUGUI>("EnemyName").text = "乱码爬虫";
                    Find<Image>("EnemySprite").sprite = 乱码爬虫Icon;
                    break;
                case UnitType.NullPointer:
                    Find<TextMeshProUGUI>("EnemyName").text = "指针操纵者";
                    Find<Image>("EnemySprite").sprite = 指针操纵者Icon;
                    break;
                case UnitType.Stone:
                    Find<TextMeshProUGUI>("EnemyName").text = "递归幻影";
                    Find<Image>("EnemySprite").sprite = BossIcon;
                    break;
            }
            
            if (unit.data.isEnemy) return;
        }

        public override void Close(params object[] args)
        {
            base.Close(args);

        }
    }
}