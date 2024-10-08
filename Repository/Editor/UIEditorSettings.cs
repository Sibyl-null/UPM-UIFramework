using System;
using System.Collections.Generic;
using System.IO;
using UIFramework.Runtime;
using UnityEditor;
using UnityEngine;

namespace UIFramework.Editor
{
    public class UIEditorSettings : ScriptableObject
    {
        [Header("公共配置")]
        public GameObject TemplateUIPrefab;
        [Tooltip("UI Prefab 需要处于这些目录下")]
        public List<DefaultAsset> UIPrefabLoadFolders;
        
        [Header("UIInfo 生成配置")]
        public string UIInfoFilePath;
        public TextAsset UIInfoTemplate;
        
        [Header("Page 生成配置")]
        public string RootGenFolder;
        public string RootNamespace;
        public TextAsset PageTemplate;
        
        
        // ------------------------------------------------------------------------
        // methods
        // ------------------------------------------------------------------------

        private const string DefaultSavePath = "Assets/Editor/UI/UIEditorSettings.asset";
        private const string TemplateUIPrefabPath = "Packages/com.beatles.unity.ui/Editor/AssetRes/ZTemplateUI.prefab";
        private const string UIInfoTemplatePath = "Packages/com.beatles.unity.ui/Editor/AssetRes/Templates/UIInfoTemplate.txt";
        private const string PageTemplatePath = "Packages/com.beatles.unity.ui/Editor/AssetRes/Templates/PageTemplate.txt";
        
        [MenuItem("Project/UI/Create EditorSettings", false, UIMenuItems.ProjectInitPriority)]
        public static void CreateAsset()
        {
            if (File.Exists(DefaultSavePath))
            {
                UILogger.Warning("[UI] 目标路径已存在文件 " + DefaultSavePath);
                return;
            }

            string directoryName = Path.GetDirectoryName(DefaultSavePath);
            if (Directory.Exists(directoryName) == false)
                Directory.CreateDirectory(directoryName);

            UIEditorSettings so = CreateInstance<UIEditorSettings>();
            SetDefaultData(so);
            AssetDatabase.CreateAsset(so, DefaultSavePath);
            
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorGUIUtility.PingObject(so);
            UILogger.Info($"[UI] {DefaultSavePath} 创建成功");
        }

        private static void SetDefaultData(UIEditorSettings settings)
        {
            settings.UIInfoFilePath = "Assets/Scripts/UI/Core/UIManager_Info.cs";
            settings.RootGenFolder = "Assets/Scripts/UI";
            settings.RootNamespace = "UI";
            settings.TemplateUIPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(TemplateUIPrefabPath);
            settings.PageTemplate = AssetDatabase.LoadAssetAtPath<TextAsset>(PageTemplatePath);
            settings.UIInfoTemplate = AssetDatabase.LoadAssetAtPath<TextAsset>(UIInfoTemplatePath);
        }
        
        public static UIEditorSettings MustLoad()
        {
            string guid = AssetDatabase.FindAssets($"t:{nameof(UIEditorSettings)}")[0];
            string path = AssetDatabase.GUIDToAssetPath(guid);
            
            UIEditorSettings result = AssetDatabase.LoadAssetAtPath<UIEditorSettings>(path);
            if (result == null)
                throw new Exception("[UI] Can't find UIEditorSettings");
            
            return result;
        }
    }
}