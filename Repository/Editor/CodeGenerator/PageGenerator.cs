using System;
using System.Collections.Generic;
using UIFramework.Runtime.Page;
using UIFramework.Runtime.Utility;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace UIFramework.Editor.CodeGenerator
{
    internal static class PageGenerator
    {
        private class GenData
        {
            public string SelfNamespace;
            public string PageClassName;
            public HashSet<string> DepNamespaceSet;
        }

        internal static void Generate(GameObject go)
        {
            UIEditorSettings settings = UIEditorSettings.MustLoad();
            
            GenData data = new GenData
            {
                SelfNamespace = $"{settings.PageNamespace}",
                PageClassName = go.name,
                DepNamespaceSet = new HashSet<string>()
                {
                    typeof(IPageArg).Namespace,
                }
            };

            string code = UIEditorUtility.ScribanGenerateText(settings.PageTemplate.text, data);

            string savePath = $"{settings.PageGenFolder}/{data.PageClassName}.cs";
            UIEditorUtility.OverlayWriteTextFile(savePath, code);
            
            string prefabPath = AssetDatabase.GetAssetPath(go);
            EditorPrefs.SetString(AutoMountKey, AssetDatabase.AssetPathToGUID(prefabPath));
            
            Debug.Log("[UI] PageGenerator Finished");
        }
        
        
        // ------------------------------------------------------------------------
        // 脚本自动挂载
        // ------------------------------------------------------------------------
        
        private const string AutoMountKey = "UIFramework_AutoMountKey";
        
        [DidReloadScripts]
        private static void AutoMountPageComponent()
        {
            if (string.IsNullOrEmpty(EditorPrefs.GetString(AutoMountKey, "")))
                return;
            
            string guid = EditorPrefs.GetString(AutoMountKey);
            MountPageComponent(guid);
            EditorPrefs.SetString(AutoMountKey, "");
        }

        private static void MountPageComponent(string guid)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (go == null)
            {
                Debug.LogError("[UI] Page 自动挂载未找到预设文件 " + path);
                return;
            }

            // 规定: Page 脚本名必须与 Prefab 名一致
            MonoScript script = UIEditorUtility.LoadMonoScriptAsset(go.name);
            if (script == null)
            {
                Debug.LogError("[UI] Page 脚本未找到，挂载失败: " + go.name);
                return;
            }
            
            Type scriptType = script.GetClass();
            TryAddComponent(go, scriptType);
            
            EditorUtility.SetDirty(go);
            AssetDatabase.SaveAssetIfDirty(go);
            
            UILogger.Info($"[UI] Page 脚本自动挂载成功: {scriptType.Name}.cs");
        }

        private static void TryAddComponent(GameObject go, Type type)
        {
            Component component = go.GetComponent(type);
            if (component == null)
                go.AddComponent(type);
        }
    }
}