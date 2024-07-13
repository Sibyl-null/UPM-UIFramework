using UI.Core;
using UI.Runtime;
using UI.Runtime.Page;

namespace UI.Page
{
    [UICodeGen((int)UIType.TestOne, (int)UILayer.Dialog, "TestOneUI")]
    public class TestOnePage : GamePage<TestOneUI>
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