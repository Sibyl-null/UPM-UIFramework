using UnityEngine;
using UnityEngine.UI;

namespace UI.Runtime
{
    [RequireComponent(typeof(Canvas)), RequireComponent(typeof(GraphicRaycaster))]
    public abstract class BaseUI : MonoBehaviour
    {
        public Canvas Canvas;
        public GraphicRaycaster Raycaster;
        public RectTransform Content;
        public Image BlackImage;
        public UIRaycast Raycast;
    }
}