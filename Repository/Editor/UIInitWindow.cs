using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UIFramework.Editor.CodeGenerator;
using UIFramework.Runtime.Page;
using UnityEditor;
using UnityEngine;

namespace UIFramework.Editor
{
    internal class UIInitWindow : OdinEditorWindow
    {
        [MenuItem("Assets/打开 UI 初始化窗口 #z", false, UIMenuItems.AssetsPriority)]
        public static void OpenWindow()
        {
            GetWindow<UIInitWindow>();
        }

        [Button("确认", ButtonSizes.Medium)]
        private void Sure()
        {
            if (!CheckValid())
                return;
            
            UIEditorSettings settings = UIEditorSettings.MustLoad();
            GameObject go = Selection.activeGameObject;
            
            PageGenerator.Generate(go);
            UIInfoGenerator.GenerateByInitWindow(
                new UIInfoGenerator.InfoItem(go.name, AssetDatabase.GetAssetPath(go)),
                $"{settings.PageNamespace}");
            
            Close();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private bool CheckValid()
        {
            GameObject go = Selection.activeGameObject;
            if (go == null)
            {
                Debug.LogError("[UI] InitUIScript 未选中任何 Prefab");
                return false;
            }
            
            BasePage page = go.GetComponent<BasePage>();
            if (page != null)
            {
                Debug.LogError("[UI] 选中 Prefab 已有 BasePage 组件");
                return false;
            }

            MonoScript script = UIEditorUtility.LoadMonoScriptAsset(go.name);
            if (script != null)
            {
                Debug.LogError($"[UI] 已存在 {go.name}.cs 脚本");
                return false;
            }

            return true;
        }
    }
}