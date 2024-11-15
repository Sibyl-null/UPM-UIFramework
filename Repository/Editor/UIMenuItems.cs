using System.IO;
using UIFramework.Editor.CodeGenerator;
using UIFramework.Runtime;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UIFramework.Editor
{
    public static class UIMenuItems
    {
        public const int ProjectInitPriority = 1000;
        private const int GenerateCodePriority = 2000;
        private const int GameObjectPriority = 3000;
        public const int AssetsPriority = 4000;

        
        // ------------------------------------------------------------------------
        // 工程初始化相关
        // ------------------------------------------------------------------------
        
        private const string RuntimeSettingsPath = "Assets/Resources/UIRuntimeSettings.asset";
        private const string InitialProjectPackagePath =
            "Packages/com.sibyl.uiframework/Editor/AssetRes/Packages/InitialProject.unitypackage";
        
        [MenuItem("Project/UI/Create RuntimeSettings", false, ProjectInitPriority)]
        public static void CreateRuntimeSettings()
        {
            if (File.Exists(RuntimeSettingsPath))
            {
                Debug.LogWarning("[UI] 目标路径已存在文件 " + RuntimeSettingsPath);
                return;
            }

            string directoryName = Path.GetDirectoryName(RuntimeSettingsPath);
            if (Directory.Exists(directoryName) == false)
                Directory.CreateDirectory(directoryName);

            UIRuntimeSettings so = ScriptableObject.CreateInstance<UIRuntimeSettings>();
            AssetDatabase.CreateAsset(so, RuntimeSettingsPath);
            
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorGUIUtility.PingObject(so);
            Debug.Log($"[UI] {RuntimeSettingsPath} 创建成功");
        }
        
        [MenuItem("Project/UI/初始化包导入", false, ProjectInitPriority)]
        public static void ImportInitialProjectPackage()
        {
            AssetDatabase.ImportPackage(InitialProjectPackagePath, true);
        }
        
        
        // ------------------------------------------------------------------------
        // 代码生成
        // ------------------------------------------------------------------------
        
        [MenuItem("Project/UI/Generate UIInfo", false, GenerateCodePriority)]
        public static void GenerateUIInfo()
        {
            UIInfoGenerator.Generate();
            AssetDatabase.Refresh();
        }
        
        
        // ------------------------------------------------------------------------
        // Assets MenuItems
        // ------------------------------------------------------------------------
        
        [MenuItem("Assets/创建 UI 模版 Prefab", false, AssetsPriority)]
        public static void CreateTemplateUI()
        {
            string path = GetSelectedPath();
            if (string.IsNullOrEmpty(path) || !Directory.Exists(path))
            {
                Debug.LogError("[UI] please select a folder to create template ui");
                return;
            }

            UIEditorSettings settings = UIEditorSettings.MustLoad();
            if (settings.TemplateUIPrefab == null)
            {
                Debug.LogError("[UI] EditorSettings TemplateUIPrefab is null");
                return;
            }
                
            GameObject go = Object.Instantiate(settings.TemplateUIPrefab);
            GameObject prefab = PrefabUtility.SaveAsPrefabAsset(go, $"{path}/{settings.TemplateUIPrefab.name}.prefab");
            
            Object.DestroyImmediate(go);
            AssetDatabase.Refresh();
            EditorGUIUtility.PingObject(prefab);
            Debug.Log("[UI] Create template ui success");
        }

        private static string GetSelectedPath()
        {
            string selectedPath = "";
            
            if (Selection.assetGUIDs.Length > 0)
                selectedPath = AssetDatabase.GUIDToAssetPath(Selection.assetGUIDs[0]);
            
            return selectedPath;
        }
        
        
        // ------------------------------------------------------------------------
        // GameObject MenuItems
        // ------------------------------------------------------------------------
        
        [MenuItem("GameObject/UI/Extension/UIRaycast", false, GameObjectPriority)]
        private static void CreateUIRaycast()
        {
            UIRuntimeSettings settings = UIEditorUtility.LoadScriptableAsset<UIRuntimeSettings>();

            GameObject go = new GameObject("UIRaycast");
            RectTransform rect = go.AddComponent<RectTransform>();
            
            rect.SetParent(Selection.activeTransform, false);
            rect.sizeDelta = new Vector2(100, 100);
            
            go.AddComponent<UIRaycast>();
            go.layer = LayerMask.NameToLayer(settings.GoLayerName);
            
            Selection.activeGameObject = go;
            EditorUtility.SetDirty(go);
        }
    }
}