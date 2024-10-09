using System;

namespace UIFramework.Runtime.Utility
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class UICodeGenAttribute : Attribute
    {
        public string LayerName { get; }

        public UICodeGenAttribute(string layerName)
        {
            LayerName = layerName;
        }
    }
}