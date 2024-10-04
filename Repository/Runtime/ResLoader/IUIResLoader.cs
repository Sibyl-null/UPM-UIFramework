using UnityEngine;

namespace UIFramework.Runtime.ResLoader
{
    public interface IUIResLoader
    {
        GameObject LoadAndInstantiatePrefab(string path, Transform parent);
        void UnLoad(string path);
        UIRuntimeSettings LoadSettings();
    }
}