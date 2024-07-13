using System.Collections.Generic;
using UI.Runtime.InfoContainer;
using UI.Runtime.Page;
using UI.Runtime.Utility;

namespace UI.Runtime.PageController
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