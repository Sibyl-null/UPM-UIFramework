using System.Collections.Generic;
using UnityEngine;

namespace UIFramework.Runtime
{
    public class UIRuntimeSettings : ScriptableObject
    {
        [System.Serializable]
        public struct LayerInfo
        {
            public string Name;
            public int Order;
        }

        [Tooltip("创建的 UILayer 的 GameObject Layer Name")]
        public string GoLayerName = "UI";

        [Tooltip("默认的 UI SortingLayer")] public string SortingLayerName = "UI";

        [Tooltip("相同 UILayer 下有多个页面的时候 Order 间隔多少")]
        public int PageOrderRange = 50;

        public List<LayerInfo> LayerInfos = new()
        {
            new LayerInfo { Name = "Background", Order = 0 },
            new LayerInfo { Name = "Panel", Order = 1000 },
            new LayerInfo { Name = "Dialog", Order = 2000 },
            new LayerInfo { Name = "Effect", Order = 3000 },
            new LayerInfo { Name = "Tutor", Order = 4000 },
            new LayerInfo { Name = "Overlay", Order = 5000 },
        };

        public int GetLayerOrder(string layerName)
        {
            foreach (LayerInfo item in LayerInfos)
            {
                if (item.Name == layerName)
                    return item.Order;
            }

            return 0;
        }

        public string GetLayerName(int layerOrder)
        {
            foreach (LayerInfo item in LayerInfos)
            {
                if (item.Order == layerOrder)
                    return item.Name;
            }
            
            return string.Empty;
        }
    }
}