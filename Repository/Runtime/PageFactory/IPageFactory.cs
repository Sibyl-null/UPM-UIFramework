using UIFramework.Runtime.InfoContainer;
using UIFramework.Runtime.Page;
using UnityEngine;

namespace UIFramework.Runtime.PageFactory
{
    public interface IPageFactory
    {
        (IPage page, GameObject go) CreatePage(UIInfo info);
    }
}