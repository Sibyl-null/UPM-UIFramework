using System;

namespace UIFramework.Runtime.InfoContainer
{
    public class UIInfo
    {
        public readonly Type PageType;
        public readonly string LoadPath;

        public UIInfo(Type pageType, string loadPath)
        {
            PageType = pageType;
            LoadPath = loadPath;
        }
    }
}