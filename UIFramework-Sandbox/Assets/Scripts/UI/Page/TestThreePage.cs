using UI.Core;
using UI.Runtime;
using UI.Runtime.Page;

namespace UI.Page
{
    [UICodeGen((int)UIType.TestThree, (int)UILayer.Dialog, "TestThreeUI")]
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