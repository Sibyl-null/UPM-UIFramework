using UnityEngine;

namespace UI.Runtime.LayerController
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
            GoLayerValue = settings.GameObjectLayerValue;
            SortingLayerName = settings.SortingLayerName;
            PageOrderRange = settings.PageOrderRange;
        }
    }
}