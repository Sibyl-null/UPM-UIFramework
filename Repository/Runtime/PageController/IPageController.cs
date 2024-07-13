using System.Collections.Generic;
using UIFramework.Runtime.InfoContainer;
using UIFramework.Runtime.Page;
using UIFramework.Runtime.Utility;

namespace UIFramework.Runtime.PageController
{
    public interface IPageController
    {
        void Release();

        IPage GetPage(UIInfo info);
        IEnumerable<IPage> GetAllPages();

        IPage CreatePage(UIInfo info);
        UIAsyncHandle OpenPage(UIInfo info, IPageArg arg = null);
        UIAsyncHandle ClosePage(UIInfo info, bool closeAnim);
        UIAsyncHandle DestroyPage(UIInfo info, bool closeAnim);
    }
}