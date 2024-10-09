using UIFramework.Runtime;
using UIFramework.Runtime.ResLoader;
using UIFramework.Runtime.Utility;
using UnityEngine;

namespace UI.Core
{
    /** 替换为项目中自己的加载方式 */
    public class UIResLoader : IUIResLoader
    {
        public GameObject LoadAndInstantiatePrefab(string path, Transform parent)
        {
            path = path.Replace("Assets/Resources/", "").Replace(".prefab", "");
            
            GameObject prefab = Resources.Load<GameObject>(path);
            if (prefab == null)
            {
                UILogger.Warning($"[UI] Load prefab failed! {path}");
                return null;
            }
            
            return Object.Instantiate(prefab, parent);
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