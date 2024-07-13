using System;

namespace UIFramework.Runtime.InfoContainer
{
    public class UIInfo
    {
        public readonly Type PageType;
        public readonly int Layer;
        public readonly string LoadPath;

        public UIInfo(Type pageType, int layer, string loadPath)
        {
            PageType = pageType;
            Layer = layer;
            LoadPath = loadPath;
        }
    }
}