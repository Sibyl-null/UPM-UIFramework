using UIFramework.Runtime.Page;

namespace UI.Core
{
    public class GamePage : BasePage
    {
        public override void OnEscape()
        {
            base.OnEscape();
            UIManager.Instance.ClosePage(UIInfo.PageType);
        }
    }
}
