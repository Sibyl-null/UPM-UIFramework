using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UIFramework.Editor.CodeGenerator;
using UIFramework.Runtime;
using UIFramework.Runtime.Page;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace UIFramework.Editor.UICodeGenWindow
{
    public class UICodeGenWindow : EditorWindow
    {
        [MenuItem("Assets/Open UICodeGenWindow #a", false, UIMenuItems.AssetsPriority)]
        public static void OpenWindow()
        {
            GetWindow<UICodeGenWindow>("UICodeGenWindow");
        }

        private DropdownField _uiLayerDrop;
        private TextField _loadPathField;
        private TextField _uiScriptPathField;
        private TextField _pageScriptPathField;
        private Button _genButton;
        private Button _forceGenButton;
        
        private GameObject _selectedPrefab;
        private UIEditorSettings _settings;
        private Type _uiTypeType;
        private Type _uiLayerType;
        private string _loadPath;
        
        public void CreateGUI()
        {
            // 使用 try/catch 的原因是，如果在这里抛出异常，将无法再次打开窗口，只能通过重启 Unity 解决。
            try
            {
                ObtainParameters();
                ImportUXml();
                GetUIElements();
                InitializeUI();
            }
            catch (Exception e)
            {
                UILogger.Error(e.Message);
            }
        }

        private void OnDisable()
        {
            _selectedPrefab = null;
        }

        private void ImportUXml()
        {
            var visualTree = UIEditorUtility.LoadUXml<UICodeGenWindow>();
            visualTree.CloneTree(rootVisualElement);
        }
        
        private void GetUIElements()
        {
            _uiLayerDrop = rootVisualElement.Q<DropdownField>("UILayerDrop");
            _loadPathField = rootVisualElement.Q<TextField>("LoadPathField");
            _uiScriptPathField = rootVisualElement.Q<TextField>("UIScriptPathField");
            _pageScriptPathField = rootVisualElement.Q<TextField>("PageScriptPathField");
            _genButton = rootVisualElement.Q<Button>("GenButton");
            _forceGenButton = rootVisualElement.Q<Button>("ForceGenButton");
        }

        private void ObtainParameters()
        {
            _selectedPrefab = Selection.activeGameObject;
            if (_selectedPrefab == null)
                throw new Exception("[UI] No Prefab is selected.");
            
            _settings = UIEditorSettings.MustLoad();
            
            _uiLayerType = Type.GetType(_settings.UILayerAssemblyQualifiedName);
            if (_uiLayerType == null)
                throw new Exception("[UI] EditorSettings: UILayerAssemblyQualifiedName is invalid.");

            _loadPath = GetLoadPath();
            if (string.IsNullOrEmpty(_loadPath))
                throw new Exception("[UI] Prefab 需要在 EditorSettings 中指定的文件夹下.");
        }
        
        private string GetLoadPath()
        {
            string assetPath = AssetDatabase.GetAssetPath(_selectedPrefab);

            if (_settings.UIPrefabLoadFolders.Any(folder => folder == null))
            {
                throw new Exception("[UI] EditorSettings: UIPrefabLoadFolders contains null.");
            }

            foreach (string folderPath in _settings.UIPrefabLoadFolders.Select(AssetDatabase.GetAssetPath)
                         .Where(folderPath => assetPath.StartsWith(folderPath)))
            {
                if (folderPath.Contains("Resources"))
                    assetPath = assetPath.Substring(0, assetPath.Length - Path.GetExtension(assetPath).Length);
                    
                assetPath = assetPath.Substring(folderPath.Length + 1);
                return assetPath;
            }
            
            return null;
        }
        
        private void InitializeUI()
        {
            string pageName = _selectedPrefab.name.TrimEnd("UI".ToCharArray()) + "Page";
            
            _uiLayerDrop.choices = Enum.GetNames(_uiLayerType).ToList();
            if (Enum.IsDefined(_uiLayerType, _settings.DefaultUILayer))
                _uiLayerDrop.value = _settings.DefaultUILayer;
            
            _loadPathField.value = _loadPath;
            _uiScriptPathField.value = $"{_settings.BaseUIGenFolder}/{_selectedPrefab.name}.cs";
            _pageScriptPathField.value = $"{_settings.PageGenFolder}/{pageName}.cs";
            _genButton.clicked += OnGenButton;
            _forceGenButton.clicked += OnForceGenButton;
        }

        private void OnGenButton()
        {
            if (VerifyGenParameters() == false)
                return;
            
            GenerateCodes();
        }
        
        private void OnForceGenButton()
        {
            if (EditorUtility.DisplayDialog("强制代码生成", "可能会覆盖已存在的代码, 是否继续?", "确认", "取消"))
            {
                GenerateCodes();
            }
        }

        private bool VerifyGenParameters()
        {
            if (string.IsNullOrEmpty(_uiLayerDrop.value))
            {
                EditorUtility.DisplayDialog("生成失败", "UILayer 不能为空", "确认");
                return false;
            }

            if (string.IsNullOrEmpty(_loadPathField.value))
            {
                EditorUtility.DisplayDialog("生成失败", "LoadPath 不能为空", "确认");
                return false;
            }

            if (string.IsNullOrEmpty(_uiScriptPathField.value))
            {
                EditorUtility.DisplayDialog("生成失败", "UIScriptPath 不能为空", "确认");
                return false;
            }

            if (string.IsNullOrEmpty(_pageScriptPathField.value))
            {
                EditorUtility.DisplayDialog("生成失败", "PageScriptPath 不能为空", "确认");
                return false;
            }

            if (File.Exists(_pageScriptPathField.value))
            {
                EditorUtility.DisplayDialog("生成失败", "Page 脚本文件已存在", "确认");
                return false;
            }
            
            return true;
        }
        
        private void GenerateCodes()
        {
            BaseUIGenerator.Generate(_selectedPrefab);

            PageGenerator.Generate(_pageScriptPathField.value, new PageGenerator.GenData
            {
                UiLayer = _uiLayerDrop.value,
                LoadPath = _loadPathField.value,
                PageClassName = Path.GetFileNameWithoutExtension(_pageScriptPathField.value),
                UiClassName = Path.GetFileNameWithoutExtension(_uiScriptPathField.value),
                DepNamespaceSet = new HashSet<string>()
                {
                    _uiTypeType.Namespace, _uiLayerType.Namespace, 
                    typeof(UICodeGenAttribute).Namespace, typeof(IPageArg).Namespace
                }
            });
            
            UIInfoGenerator.GenerateWithNewInfo(new UIInfoGenerator.InfoItem
            {
                PageType = Path.GetFileNameWithoutExtension(_pageScriptPathField.value),
                Layer = _uiLayerDrop.value,
                LoadPath = _loadPathField.value
            }, _settings.PageNamespace);
            
            AssetDatabase.Refresh();
        }
    }
}