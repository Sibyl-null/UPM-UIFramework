using UnityEngine;

namespace UIFramework.Runtime.ResLoader
{
    public interface IUIResLoader
    {
        GameObject Load(string path);
        void UnLoad(string path);
        UIRuntimeSettings LoadSettings();
    }
}