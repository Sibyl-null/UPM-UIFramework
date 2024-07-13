using System;

namespace UI.Runtime.InfoContainer
{
    public class UIInfo
    {
        public readonly int UIType;
        public readonly int Layer;
        public readonly Type PageType;
        public readonly string LoadPath;

        public UIInfo(int type, int layer, Type pageType, string loadPath)
        {
            UIType = type;
            Layer = layer;
            PageType = pageType;
            LoadPath = loadPath;
        }
    }
}