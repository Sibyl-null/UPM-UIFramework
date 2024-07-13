using System;
using System.Collections.Generic;
using System.IO;
using UI.Runtime;
using UnityEditor;
using UnityEngine;

namespace UI.Editor
{
    public class UIEditorSettings : ScriptableObject
    {
        [Serializable]
        public struct GenItem
        {
            public string TypeName;
            public string GenSuffix;

            public GenItem(string typeName, string genSuffix)
            {
                TypeName = typeName;
                GenSuffix = genSuffix;
            }
        }
        
        [Header("公共配置")]
        public GameObject TemplateUIPrefab;
        public string UITypeAssemblyQualifiedName;
        public string UILayerAssemblyQualifiedName;
        [Tooltip("打开 CodeGenWindow 时，默认选中的层级")]
        public string DefaultUILayer;
        [Tooltip("UI Prefab 需要处于这些目录下，默认加载路径会自动剔除文件夹的路径，若为 Resources 目录还会剔除后缀")]
        public List<DefaultAsset> UIPrefabLoadFolders;

        [Header("UIType 生成配置")]
        public string UITypeFilePath;
        public TextAsset UITypeTemplate;
        
        [Header("UIInfo 生成配置")]
        public string UIInfoFilePath;
        public TextAsset UIInfoTemplate;
        
        [Header("Page 生成配置")]
        public string PageGenFolder;
        public string PageNamespace;
        public TextAsset PageTemplate;
        
        [Header("BaseUI 生成配置")]
        public string BaseUIGenFolder;
        public TextAsset BaseUITemplate;
        public List<GenItem> GenSupports;
        
        // ------------------------------------------------------------------------
        // methods
        // ------------------------------------------------------------------------

        private const string DefaultSavePath = "Assets/Editor/UI/UIEditorSettings.asset";
        private const string TemplateUIPrefabPath = "Packages/com.beatles.unity.ui/Editor/AssetRes/ZTemplateUI.prefab";
        private const string BaseUITemplatePath = "Packages/com.beatles.unity.ui/Editor/AssetRes/Templates/BaseUITemplate.txt";
        private const string UIInfoTemplatePath = "Packages/com.beatles.unity.ui/Editor/AssetRes/Templates/UIInfoTemplate.txt";
        private const string PageTemplatePath = "Packages/com.beatles.unity.ui/Editor/AssetRes/Templates/PageTemplate.txt";
        private const string UITypeTemplatePath = "Packages/com.beatles.unity.ui/Editor/AssetRes/Templates/UITypeTemplate.txt";
        
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
            settings.UITypeAssemblyQualifiedName = "UI.Core.UIType, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null";
            settings.UILayerAssemblyQualifiedName = "UI.Core.UILayer, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null";
            settings.DefaultUILayer = "Dialog";
            settings.UIInfoFilePath = "Assets/Scripts/UI/Core/UIManager_Info.cs";
            settings.UITypeFilePath = "Assets/Scripts/UI/Core/UIType.cs";
            settings.PageGenFolder = "Assets/Scripts/UI/Page";
            settings.BaseUIGenFolder = "Assets/Gen/UI";
            settings.PageNamespace = "UI.Page";
            settings.TemplateUIPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(TemplateUIPrefabPath);
            settings.PageTemplate = AssetDatabase.LoadAssetAtPath<TextAsset>(PageTemplatePath);
            settings.BaseUITemplate = AssetDatabase.LoadAssetAtPath<TextAsset>(BaseUITemplatePath);
            settings.UIInfoTemplate = AssetDatabase.LoadAssetAtPath<TextAsset>(UIInfoTemplatePath);
            settings.UITypeTemplate = AssetDatabase.LoadAssetAtPath<TextAsset>(UITypeTemplatePath);
            settings.GenSupports = new List<GenItem>()
            {
                new GenItem("GameObject", "Go"),
                new GenItem("RectTransform", "Trans"),
                new GenItem("Image", "Img"),
                new GenItem("Button", "Btn"),
                new GenItem("Text", "Text"),
                new GenItem("LangText", "Text"),
                new GenItem("Canvas", "Canvas"),
                new GenItem("UIToggle", "Toggle"),
                new GenItem("ResizeText", "Text"),
                new GenItem("TextMeshProUGUI", "Tmp")
            };
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