using UI.Runtime.InfoContainer;
using UI.Runtime.Page;

namespace UI.Runtime.PageFactory
{
    public interface IPageFactory
    {
        IPage CreatePage(UIInfo info);
    }
}