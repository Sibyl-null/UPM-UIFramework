using UnityEngine;

namespace UI.Runtime
{
    public class UIRuntimeSettings : ScriptableObject
    {
        [Tooltip("创建的 UILayer 的 GameObject Layer Index")]
        public int GameObjectLayerValue = 5;
        
        [Tooltip("默认的 UI SortingLayer")]
        public string SortingLayerName = "Default";
        
        [Tooltip("相同 UILayer 下有多个页面的时候 Order 间隔多少")]
        public int PageOrderRange = 50;
    }
}