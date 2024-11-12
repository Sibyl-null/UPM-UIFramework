using UIFramework.Runtime;
using UIFramework.Runtime.ResLoader;
using UnityEngine;

namespace UI.Core
{
    /** 替换为项目中自己的加载方式 */
    public class UIResLoader : IUIResLoader
    {
        public GameObject Load(string path)
        {
            path = path.Replace("Assets/Resources/", "").Replace(".prefab", "");
            
            GameObject prefab = Resources.Load<GameObject>(path);
            if (prefab == null)
            {
                Debug.LogWarning($"[UI] Load prefab failed! {path}");
                return null;
            }
            
            return prefab;
        }

        public void UnLoad(string path)
        {
            Debug.Log("UI UnLoad");
        }

        public UIRuntimeSettings LoadSettings()
        {
            return Resources.Load<UIRuntimeSettings>("UIRuntimeSettings");
        }
    }
}