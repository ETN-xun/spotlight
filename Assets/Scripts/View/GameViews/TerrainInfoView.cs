using TMPro;
using View.Base;

namespace View.GameViews
{
    public class TerrainInfoView : BaseView
    {
        public override void Open(params object[] args)
        {
            base.Open(args);
            if (args[0] is not GridCell cell) return;
            if (cell.TerrainData is null) return;
            Find<TextMeshProUGUI>("Text").text = cell.TerrainData.name;
        }
    }
}