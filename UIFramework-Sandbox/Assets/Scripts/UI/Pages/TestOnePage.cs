using UI.Core;
using UIFramework.Runtime.Page;

namespace UI.Pages
{
    public class TestOnePage : GamePage
    {
        protected override UILayer UILayer => UILayer.Bottom;

        protected override void OnInit()
        {
            base.OnInit();
        }

        protected override void OnPrepare(IPageArg arg = null)
        {
            base.OnPrepare(arg);
        }

        protected override void OnClear()
        {
            base.OnClear();
        }
    }
}