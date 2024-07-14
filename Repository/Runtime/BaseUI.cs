using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace UIFramework.Runtime
{
    [RequireComponent(typeof(Canvas)), RequireComponent(typeof(GraphicRaycaster))]
    public abstract class BaseUI : MonoBehaviour
    {
#if UNITY_EDITOR
        [ValueDropdown("GetAllLayerName")]
        public string LayerName;

        private string[] GetAllLayerName()
        {
            string guid = UnityEditor.AssetDatabase.FindAssets("t:UIRuntimeSettings").Single();
            string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
            var settings = UnityEditor.AssetDatabase.LoadAssetAtPath<UIRuntimeSettings>(path);
            return settings.LayerOrders.Keys.ToArray();
        }
#endif
        
        public Canvas Canvas;
        public GraphicRaycaster Raycaster;
        public RectTransform Content;
        public Image BlackImage;
        public UIRaycast Raycast;
    }
}