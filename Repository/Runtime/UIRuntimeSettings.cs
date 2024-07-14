using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace UIFramework.Runtime
{
    public class UIRuntimeSettings : SerializedScriptableObject
    {
        [Tooltip("创建的 UILayer 的 GameObject Layer Name")]
        public string GoLayerName = "UI";

        [Tooltip("默认的 UI SortingLayer")]
        public string SortingLayerName = "UI";
        
        [Tooltip("相同 UILayer 下有多个页面的时候 Order 间隔多少")]
        public int PageOrderRange = 50;

        public Dictionary<string, int> LayerOrders = new()
        {
            { "Background", 0 },
            { "Panel", 1000 },
            { "Play", 2000 },
            { "Dialog", 3000 },
            { "Effect", 4000 },
            { "Tutor", 5000 },
            { "Overlay", 6000 },
        };
    }
}