using UnityEngine;

namespace UIFramework.Runtime.Utility
{
    internal static class UIUtility
    {
        private static readonly Vector2 NormalizePivot = new(0.5f, 0.5f);

        public static void NormalizeTransform(this RectTransform trans)
        {
            trans.sizeDelta = Vector2.zero;
            trans.anchorMin = Vector2.zero;
            trans.anchorMax = Vector2.one;
            trans.pivot = NormalizePivot;
            trans.anchoredPosition = Vector2.zero;
        }
    }
}