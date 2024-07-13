using UnityEngine;

namespace UI.Runtime.ResLoader
{
    public interface IResLoader
    {
        GameObject LoadAndInstantiatePrefab(string path, Transform parent);
        void UnLoad(string path);
        UIRuntimeSettings LoadSettings();
    }
}