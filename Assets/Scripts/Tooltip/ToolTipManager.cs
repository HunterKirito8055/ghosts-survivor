using AarquieSolutions.Base.Singleton;
using AarquieSolutions.DependencyInjection.ComponentField;

namespace Tooltip
{
    public class ToolTipManager : Singleton<ToolTipManager>
    {
        [FindComponent] private ToolTip toolTip;

        public override void Awake()
        {
            base.Awake();
            this.InitializeDependencies();
        }
        public void Show(string content, string header = "")
        {
            toolTip.Show(content, header);
        }

        public void Hide()
        {
            toolTip.Hide();
        }
    }
}
