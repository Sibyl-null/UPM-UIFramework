using System;
using UnityEngine;

namespace UIFramework.Runtime.Utility
{
    public static class UIUtility
    {
        private static readonly Vector2 NormalizePivot = new Vector2(0.5f, 0.5f);

        public static void NormalizeTransform(this RectTransform trans)
        {
            trans.sizeDelta = Vector2.zero;
            trans.anchorMin = Vector2.zero;
            trans.anchorMax = Vector2.one;
            trans.pivot = NormalizePivot;
            trans.anchoredPosition = Vector2.zero;
        }
        
        // -----------------------------------------------------------------------------
        
        private static Func<int, string> _uiLayerLogFunc;

        public static void SetUILayerLogger(Func<int, string> func)
        {
            _uiLayerLogFunc = func;
        }

        public static string LogUILayer(int uiLayer)
        {
            if (_uiLayerLogFunc != null)
            {
                return _uiLayerLogFunc.Invoke(uiLayer);
            }

            return uiLayer.ToString();
        }
    }
}