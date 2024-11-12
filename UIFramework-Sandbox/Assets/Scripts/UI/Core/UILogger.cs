using UIFramework.Runtime.Utility;
using UnityEngine;

namespace UI.Core
{
    public class UILogger : IUILogger
    {
        public void Info(string message)
        {
            Debug.Log(message);
        }

        public void Warning(string message)
        {
            Debug.LogWarning(message);
        }

        public void Error(string message)
        {
            Debug.LogError(message);
        }

        public string UILayerToString(int layer)
        {
            return ((UILayer)layer).ToString();
        }
    }
}