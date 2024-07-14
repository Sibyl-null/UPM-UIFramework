using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UIFramework.Runtime;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace UIFramework.Editor.CodeGenerator
{
    internal static class BaseUIGenerator
    {
        private struct FieldData
        {
            public readonly string TypeName;
            public readonly string FieldName;
            public readonly string GoName;

            public FieldData(string typeName, string fieldName, string goName)
            {
                TypeName = typeName;
                FieldName = fieldName;
                GoName = goName;
            }
        }

        private class GenData
        {
            public string PrefabName;
            public string SelfNamespace;
            public HashSet<string> Namespaces = new HashSet<string>();
            public Dictionary<string, string> GoNamePathMap = new Dictionary<string, string>();
            public List<FieldData> Fields = new List<FieldData>();
        }
        
        internal static void Generate(GameObject selectedGo)
        {
            string guid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(selectedGo));

            VerifySetting();
            GenData data = CollectGenerateData(selectedGo);
            bool codeChanged = GenerateAndSaveCode(data, guid);

            if (codeChanged == false)
                AutoMountBaseUI();
        }
        
        private static void VerifySetting()
        {
            UIEditorSettings settings = UIEditorSettings.MustLoad();
            if (string.IsNullOrEmpty(settings.RootGenFolder))
                throw new Exception("[UI] EditorSettings 未设置 RootGenFolder");
            
            if (string.IsNullOrEmpty(settings.RootNamespace))
                throw new Exception("[UI] EditorSettings 未设置 RootNamespace");
            
            if (settings.BaseUITemplate == null)
                throw new Exception("[UI] EditorSettings 未设置 BaseUI 代码模板文件");
        }
        
        private static GenData CollectGenerateData(GameObject selectedGo)
        {
            UIEditorSettings settings = UIEditorSettings.MustLoad();
            
            MonoScript script = UIEditorUtility.LoadMonoScriptAsset(selectedGo.name);
            string selfNamespace = script != null
                ? script.GetClass().Namespace
                : $"{settings.RootNamespace}.{selectedGo.name.TrimUIEnd()}";
            
            GenData data = new GenData();
            data.PrefabName = selectedGo.name;
            data.SelfNamespace = selfNamespace;
            data.Namespaces.Add(typeof(BaseUI).Namespace);
            data.Namespaces.Add(typeof(UIRaycast).Namespace);
            data.Namespaces.Add("UnityEngine");
            data.Namespaces.Add("UnityEngine.UI");
            
            List<GameObject> goList = new List<GameObject>();
            GetAllGameObjects(selectedGo.transform, ref goList);

            bool shouldGenGameObject = settings.GenSupports.Count(x => x.TypeName == nameof(GameObject)) > 0;
            
            foreach (GameObject go in goList)
            {
                CollectSingleGameObject(go, data, shouldGenGameObject);
            }
            
            return data;
        }

        private static void CollectSingleGameObject(GameObject go, GenData data, bool shouldGenGameObject)
        {
            if (go.CompareTag(UIMenuItems.AutoTag) == false)
                return;

            if (go.name.Contains(' ') || go.name.Contains('(') || go.name.Contains(')'))
                throw new Exception("[UI] BaseUIGenerator - 对象名包含非法字符 " + go.name);
                
            if (data.GoNamePathMap.ContainsKey(go.name))
                throw new Exception("[UI] BaseUIGenerator - 存在对象重名 " + go.name);
            
            UIEditorSettings settings = UIEditorSettings.MustLoad();
            UIEditorSettings.GenItem goGemItem =
                settings.GenSupports.FirstOrDefault(x => x.TypeName == nameof(GameObject));
            
            data.GoNamePathMap.Add(go.name, GetChildFindPath(go.transform));

            Component[] components = go.GetComponents<Component>();
            foreach (Component component in components)
            {
                foreach (UIEditorSettings.GenItem genItem in settings.GenSupports
                             .Where(item => item.TypeName == component.GetType().Name))
                {
                    data.Namespaces.Add(component.GetType().Namespace);
                    data.Fields.Add(new FieldData(genItem.TypeName, go.name + genItem.GenSuffix, go.name));
                    break;
                }
            }

            if (shouldGenGameObject)
            {
                data.Namespaces.Add(typeof(GameObject).Namespace);
                data.Fields.Add(new FieldData(nameof(GameObject), go.name + goGemItem.GenSuffix, go.name));
            }
        }

        private static bool GenerateAndSaveCode(GenData data, string guid)
        {
            UIEditorSettings settings = UIEditorSettings.MustLoad();
            
            string code = UIEditorUtility.ScribanGenerateText(settings.BaseUITemplate.text, data);
            
            MonoScript script = UIEditorUtility.LoadMonoScriptAsset(data.PrefabName);
            string filePath = script != null
                ? AssetDatabase.GetAssetPath(script)
                : $"{settings.RootGenFolder}/{data.PrefabName.TrimUIEnd()}/{data.PrefabName}.cs";

            bool codeChanged = script == null || script.text != code;
            
            UIEditorUtility.OverlayWriteTextFile(filePath, code);
            
            EditorPrefs.SetString(AutoMountKey, guid);
            UILogger.Info("[UI] BaseUI 代码生成成功! " + filePath);

            return codeChanged;
        }

        private static void GetAllGameObjects(Transform parent, ref List<GameObject> result)
        {
            result.Add(parent.gameObject);
            foreach (Transform childTrans in parent)
                GetAllGameObjects(childTrans, ref result);
        }

        private static string GetChildFindPath(Transform child)
        {
            string findPath = child.name;
            
            while (child.parent != null && child.parent.parent != null)
            {
                child = child.parent;
                findPath = child.name + "/" + findPath;
            }

            return findPath;
        }
        
        
        // ------------------------------------------------------------------------
        // BaseUI 脚本自动挂载
        // ------------------------------------------------------------------------
        
        private const string AutoMountKey = "AutoMountKey";
        
        [DidReloadScripts]
        private static void AutoMountBaseUI()
        {
            if (string.IsNullOrEmpty(EditorPrefs.GetString(AutoMountKey, "")) == false)
            {
                string guid = EditorPrefs.GetString(AutoMountKey);
                MountBaseUIComponent(guid);
                EditorPrefs.SetString(AutoMountKey, "");
            }
        }

        private static void MountBaseUIComponent(string guid)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (go == null)
            {
                UILogger.Error("[UI] BaseUIGenerator 未找到预设文件 " + path);
                return;
            }

            // 规定: BaseUI 脚本名必须与 Prefab 名一致
            MonoScript script = UIEditorUtility.LoadMonoScriptAsset(go.name);
            if (script == null)
            {
                UILogger.Error("[UI] BaseUI 脚本未找到，挂载失败: " + go.name);
                return;
            }
            
            Type scriptType = script.GetClass();
            Component component = GetOrAddComponent(go, scriptType);
            
            MethodInfo methodInfo =
                scriptType.GetMethod("BindComponent", BindingFlags.Instance | BindingFlags.NonPublic);
            if (methodInfo == null)
            {
                UILogger.Error("[UI] BaseUI 脚本中未找到 BindComponent 方法，挂载失败: " + scriptType.Name);
                return;
            }
            
            methodInfo.Invoke(component, new object[] { });
            EditorUtility.SetDirty(go);
            AssetDatabase.SaveAssetIfDirty(go);
            
            UILogger.Info($"[UI] BaseUI 脚本添加绑定成功: {scriptType.Name}.cs");
        }

        private static Component GetOrAddComponent(GameObject go, Type type)
        {
            Component component = go.GetComponent(type);
            if (component == null)
                component = go.AddComponent(type);
            
            return component;
        }
    }
}