using UIFramework.Runtime;
using UnityEditor;
using UnityEngine.UIElements;

namespace UIFramework.Editor
{
    [CustomEditor(typeof(UIRuntimeSettings))]
    public class UIRuntimeSettingsDrawer : UnityEditor.Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            VisualTreeAsset asset = UIEditorUtility.LoadUXml<UIRuntimeSettings>();
            VisualElement root = asset.CloneTree();
            return root;
        }
    }
}