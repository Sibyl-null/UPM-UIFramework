using UIFramework.Runtime;
using UIFramework.Runtime.Page;

namespace UI.Core
{
    public class GamePage<T> : BasePage where T : BaseUI
    {
        public T UI { get; private set; }

        protected override void OnInit()
        {
            base.OnInit();
            UI = (T)BaseUI;
        }
        
        public override void OnEscape()
        {
            base.OnEscape();
            UIManager.Instance.ClosePage((UIType)UIInfo.UIType);
        }
    }
}
