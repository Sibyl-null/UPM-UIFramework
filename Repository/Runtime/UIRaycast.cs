using UnityEngine;
using UnityEngine.UI;

namespace UIFramework.Runtime
{
    [AddComponentMenu("UI/Extension/UIRaycast")]
    [RequireComponent(typeof(CanvasRenderer))]
    public class UIRaycast : MaskableGraphic
    {
        protected UIRaycast()
        {
            useLegacyMeshGeneration = false;
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
        }
    }
}