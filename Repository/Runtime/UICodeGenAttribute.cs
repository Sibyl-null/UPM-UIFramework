using System;
using System.Diagnostics;

namespace UIFramework.Runtime
{
    [Conditional("UNITY_EDITOR")]
    [AttributeUsage(AttributeTargets.Class)]
    public class UICodeGenAttribute : Attribute
    {
        public int Layer;
        public string LoadPath;

        public UICodeGenAttribute(int layer, string loadPath)
        {
            Layer = layer;
            LoadPath = loadPath;
        }
    }
}