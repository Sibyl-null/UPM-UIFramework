using System;
using UnityEngine;

namespace UI.Runtime.Utility
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

        private static Func<int, string> _uiTypeLogFunc;
        
        public static void SetUITypeLogger(Func<int, string> func)
        {
            _uiTypeLogFunc = func;
        }

        public static string LogUIType(int uiType)
        {
            if (_uiTypeLogFunc != null)
            {
                return _uiTypeLogFunc.Invoke(uiType);
            }

            return uiType.ToString();
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