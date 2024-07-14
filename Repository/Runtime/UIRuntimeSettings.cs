using UnityEngine;

namespace UIFramework.Runtime
{
    public class UIRuntimeSettings : ScriptableObject
    {
        [Tooltip("创建的 UILayer 的 GameObject Layer Name")]
        public string GoLayerName = "UI";
        
        [Tooltip("默认的 UI SortingLayer")]
        public string SortingLayerName = "UI";
        
        [Tooltip("相同 UILayer 下有多个页面的时候 Order 间隔多少")]
        public int PageOrderRange = 50;
    }
}