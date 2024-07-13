using UI.Runtime;
using UnityEditor;
using UnityEngine.UIElements;

namespace UI.Editor
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