using UI.Core;
using UIFramework.Runtime;
using UIFramework.Runtime.Page;

namespace UI.Page
{
    [UICodeGen((int)UILayer.Dialog, "TestThreeUI")]
    public class TestThreePage : GamePage<TestThreeUI>
    {   
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