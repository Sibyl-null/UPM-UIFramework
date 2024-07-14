using System;
using System.IO;
using Scriban;
using Scriban.Runtime;
using UIFramework.Runtime;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace UIFramework.Editor
{
    public static class UIEditorUtility
    {
        /** 使用 Scriban 模板生成文本 */
        public static string ScribanGenerateText(string templateText, object data)
        {
            ScriptObject scriptObject = new ScriptObject();
            scriptObject.Import(data);

            TemplateContext context = new TemplateContext();
            context.PushGlobal(scriptObject);

            Template template = Template.Parse(templateText);
            if (template.HasErrors)
            {
                foreach (var error in template.Messages)
                    UILogger.Error(error.ToString());

                throw new Exception("[UI] 文本生成失败，Scriban 模板解析出错");
            }
            
            return template.Render(context);
        }
        
        /** 覆盖写入文本文件 */
        public static void OverlayWriteTextFile(string filePath, string text)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                UILogger.Error("[UI] filePath 为空");
                return;
            }
            
            string dirPath = Path.GetDirectoryName(filePath);
            
            if (!string.IsNullOrEmpty(dirPath) && !Directory.Exists(dirPath))
                Directory.CreateDirectory(dirPath);
            
            File.WriteAllText(filePath, text);
        }
        
        /** 加载 UXml 资源, 类名与 UXml 文件名必须一致 */
        internal static VisualTreeAsset LoadUXml<T>() where T : class
        {
            Type windowType = typeof(T);
            
            string[] guids = AssetDatabase.FindAssets(windowType.Name);
            if (guids.Length == 0)
                throw new Exception($"Not found any assets : {windowType.Name}");

            foreach (string assetGuid in guids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(assetGuid);
                Type assetType = AssetDatabase.GetMainAssetTypeAtPath(assetPath);
                
                if (assetType == typeof(VisualTreeAsset))
                {
                    VisualTreeAsset treeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(assetPath);
                    return treeAsset;
                }
            }
            
            throw new Exception($"Not found UXml file : {windowType.Name}");
        }

        /** 通过类名，加载 MonoScript 资源 */
        internal static MonoScript LoadMonoScriptAsset(string typeName)
        {
            string[] guids = AssetDatabase.FindAssets(typeName);
            if (guids.Length == 0)
                return null;
            
            foreach (string guid in guids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                Type assetType = AssetDatabase.GetMainAssetTypeAtPath(assetPath);

                if (assetType == typeof(MonoScript))
                {
                    MonoScript script = AssetDatabase.LoadAssetAtPath<MonoScript>(assetPath);
                    return script;
                }
            }
            
            return null;
        }

        /** 加载 SO 资源, 类名与 SO 文件名必须一致 */
        internal static T LoadScriptableAsset<T>() where T : ScriptableObject
        {
            Type soType = typeof(T);
            
            string[] guids = AssetDatabase.FindAssets(soType.Name);
            if (guids.Length == 0)
                throw new Exception($"Not found any assets : {soType.Name}");

            foreach (string assetGuid in guids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(assetGuid);
                Type assetType = AssetDatabase.GetMainAssetTypeAtPath(assetPath);
                
                if (assetType == typeof(T))
                {
                    T soAsset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
                    return soAsset;
                }
            }
            
            throw new Exception($"Not found So file : {soType.Name}");
        }
        
        internal static string TrimUIEnd(this string str)
        {
            return str.TrimEnd("UI".ToCharArray());
        }
    }
}