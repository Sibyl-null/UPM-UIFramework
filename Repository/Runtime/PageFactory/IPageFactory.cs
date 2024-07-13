using UIFramework.Runtime.InfoContainer;
using UIFramework.Runtime.Page;

namespace UIFramework.Runtime.PageFactory
{
    public interface IPageFactory
    {
        IPage CreatePage(UIInfo info);
    }
}