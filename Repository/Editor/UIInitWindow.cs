using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UIFramework.Editor.CodeGenerator;
using UIFramework.Runtime;
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
        
        [ValueDropdown(nameof(GetLayerNames))]
        public string LayerName;

        private IEnumerable<string> GetLayerNames()
        {
            UIRuntimeSettings settings = UIEditorUtility.LoadScriptableAsset<UIRuntimeSettings>();
            return settings.LayerInfos.Select(x => x.Name);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            UIEditorSettings settings = UIEditorSettings.MustLoad();
            LayerName = settings.DefaultLayerName;
        }

        [Button("确认", ButtonSizes.Medium)]
        private void Sure()
        {
            if (!CheckValid())
                return;
            
            UIEditorSettings settings = UIEditorSettings.MustLoad();
            GameObject go = Selection.activeGameObject;
            
            PageGenerator.Generate(go, LayerName);
            UIInfoGenerator.GenerateByInitWindow(new UIInfoGenerator.InfoItem
                {
                    PageType = go.name,
                    Layer = LayerName,
                    LoadPath = AssetDatabase.GetAssetPath(go),
                }, $"{settings.RootNamespace}");
            
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
            
            UIRuntimeSettings settings = UIEditorUtility.LoadScriptableAsset<UIRuntimeSettings>();
            if (settings.LayerInfos.Count(x => x.Name == LayerName) == 0)
            {
                Debug.LogError("[UI] 该 LayerName 未在 RuntimeSettings 中定义");
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