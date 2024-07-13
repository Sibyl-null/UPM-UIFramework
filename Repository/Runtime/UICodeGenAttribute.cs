using System;
using System.Diagnostics;

namespace UI.Runtime
{
    [Conditional("UNITY_EDITOR")]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class UICodeGenAttribute : Attribute
    {
        public int UIType;
        public int Layer;
        public string LoadPath;

        public UICodeGenAttribute(int uiType, int layer, string loadPath)
        {
            UIType = uiType;
            Layer = layer;
            LoadPath = loadPath;
        }
    }
}