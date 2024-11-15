using UnityEngine;

namespace UIFramework.Runtime.LayerController
{
    public class LayerControllerArg
    {
        public readonly Transform Root;
        public readonly int GoLayerValue;
        public readonly string SortingLayerName;
        public readonly int PageOrderRange;

        public LayerControllerArg(Transform root, int goLayerValue, string sortingLayerName, int pageOrderRange)
        {
            Root = root;
            GoLayerValue = goLayerValue;
            SortingLayerName = sortingLayerName;
            PageOrderRange = pageOrderRange;
        }

        public LayerControllerArg(Transform root, UIRuntimeSettings settings)
        {
            Root = root;
            GoLayerValue = LayerMask.NameToLayer(settings.GoLayerName);
            SortingLayerName = settings.SortingLayerName;
            PageOrderRange = settings.PageOrderRange;
        }
    }
}