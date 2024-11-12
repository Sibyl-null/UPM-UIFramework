using UIFramework.Runtime.Page;

namespace UI.Core
{
    public abstract class GamePage : BasePage
    {
        public override int Layer => (int)UILayer;

        protected abstract UILayer UILayer { get; }

        public override void OnEscape()
        {
            base.OnEscape();
            UIManager.Instance.ClosePage(UIInfo.PageType);
        }
    }
}
