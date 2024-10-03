using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        private const int ImportPackagePriority = 1500;
        private const int GenerateCodePriority = 2000;
        
        private const int GameObjectPriority = 3000;
        public const int AssetsPriority = 4000;

        
        // ------------------------------------------------------------------------
        // 工程初始化相关
        // ------------------------------------------------------------------------
        
        public const string AutoTag = "AutoField";
        private const string RuntimeSettingsPath = "Assets/Resources/UIRuntimeSettings.asset";
        
        [MenuItem("Project/UI/Create RuntimeSettings", false, ProjectInitPriority)]
        public static void CreateRuntimeSettings()
        {
            if (File.Exists(RuntimeSettingsPath))
            {
                UILogger.Warning("[UI] 目标路径已存在文件 " + RuntimeSettingsPath);
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
            UILogger.Info($"[UI] {RuntimeSettingsPath} 创建成功");
        }
        
            
        [MenuItem("Project/UI/SetTags", false, ProjectInitPriority)]
        public static void SetTags()
        {
            var asset = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0];
            SerializedObject tagManager = new SerializedObject(asset);

            SerializedProperty tagsProp = tagManager.FindProperty("tags");
            AddFlags(tagsProp, new List<string> { AutoTag });

            tagManager.ApplyModifiedProperties();
            UILogger.Info("[UI] SetTags success");
        }

        private static void AddFlags(SerializedProperty flagProp, List<string> flags)
        {
            var hashSet = new HashSet<string>();
            for (int i = 0; i < flagProp.arraySize; i++)
            {
                SerializedProperty sp = flagProp.GetArrayElementAtIndex(i);
                hashSet.Add(sp.stringValue);
            }

            flagProp.ClearArray();

            foreach (var flag in flags)
            {
                hashSet.Add(flag);
            }

            var list = hashSet.ToList();
            for (int i = 0; i < list.Count; i++)
            {
                flagProp.InsertArrayElementAtIndex(i);
                SerializedProperty sp = flagProp.GetArrayElementAtIndex(i);
                sp.stringValue = list[i];
            }
        }
        
        
        // ------------------------------------------------------------------------
        // Package 导入相关
        // ------------------------------------------------------------------------

        private const string InitialProjectPackagePath =
            "Packages/com.sibyl.uiframework/Editor/AssetRes/Packages/InitialProject.unitypackage";
        
        [MenuItem("Project/UI/Import Packages/Initial Project", false, ImportPackagePriority)]
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
        
        [MenuItem("Assets/Create Template UI", false, AssetsPriority)]
        public static void CreateTemplateUI()
        {
            string path = GetSelectedPath();
            if (string.IsNullOrEmpty(path) || !Directory.Exists(path))
            {
                UILogger.Error("[UI] please select a folder to create template ui");
                return;
            }

            UIEditorSettings settings = UIEditorSettings.MustLoad();
            if (settings.TemplateUIPrefab == null)
            {
                UILogger.Error("[UI] EditorSettings TemplateUIPrefab is null");
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

        [MenuItem("Assets/Generate Base UI #z", false, AssetsPriority)]
        public static void GenerateBaseUI()
        {
            GameObject go = Selection.activeGameObject;
            if (go == null)
            {
                UILogger.Error("[UI] BaseUIGenerator 未选中任何 Prefab");
                return;
            }
            
            BaseUIGenerator.Generate(go);
            AssetDatabase.Refresh();
        }
        
        [MenuItem("Assets/InitUIScript #a", false, AssetsPriority)]
        public static void InitUIScript()
        {
            GameObject go = Selection.activeGameObject;
            if (go == null)
            {
                UILogger.Error("[UI] InitUIScript 未选中任何 Prefab");
                return;
            }
            
            MonoScript script = UIEditorUtility.LoadMonoScriptAsset(go.name.TrimUIEnd() + "Page");
            if (script != null)
            {
                EditorUtility.DisplayDialog("失败", $"{script.name} 脚本已存在", "确定");
                return;
            }
            
            BaseUIGenerator.Generate(go);
            PageGenerator.Generate(go);
            AssetDatabase.Refresh();
            
            EditorUtility.DisplayDialog("初始脚本成功", "请指定 Layer 后，手动生成 UIInfo", "确定");
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