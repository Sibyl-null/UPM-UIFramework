using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UIFramework.Runtime;
using UIFramework.Runtime.InfoContainer;
using UIFramework.Runtime.Page;
using UIFramework.Runtime.Utility;
using UnityEditor;
using UnityEngine;

namespace UIFramework.Editor.CodeGenerator
{
    /** 自动生成 UIInfo 代码 */
    internal static class UIInfoGenerator
    {
        internal class InfoItem
        {
            public string PageType;
            public string LayerOrder;
            public string LoadPath;

            public InfoItem(string pageType, string layerName, string loadPath)
            {
                UIRuntimeSettings settings = UIEditorUtility.LoadScriptableAsset<UIRuntimeSettings>();
                PageType = pageType;
                LayerOrder = settings.GetLayerOrder(layerName).ToString();
                LoadPath = loadPath;
            }
        }
        
        private class GenData
        {
            public List<string> Namespaces = new List<string>();
            public List<InfoItem> InfoItems = new List<InfoItem>();
        }
        
        internal static void GenerateDirectly()
        {
            GenData data = GetGenData();
            ProcessGenData(data);
            
            UIEditorSettings settings = UIEditorSettings.MustLoad();
            string code = UIEditorUtility.ScribanGenerateText(settings.UIInfoTemplate.text, data);
            UIEditorUtility.OverlayWriteTextFile(settings.UIInfoFilePath, code);
            
            Debug.Log("[UI] UIInfo 代码生成成功! " + settings.UIInfoFilePath);
        }

        internal static void GenerateByInitWindow(InfoItem infoItem, string pageNamespace)
        {
            GenData data = GetGenData();
            data.Namespaces.Add(pageNamespace);
            data.InfoItems.Add(infoItem);
            ProcessGenData(data);
            
            UIEditorSettings settings = UIEditorSettings.MustLoad();
            string code = UIEditorUtility.ScribanGenerateText(settings.UIInfoTemplate.text, data);
            UIEditorUtility.OverlayWriteTextFile(settings.UIInfoFilePath, code);
            
            Debug.Log("[UI] UIInfo 代码生成成功! " + settings.UIInfoFilePath);
        }

        private static GenData GetGenData()
        {
            UIEditorSettings settings = UIEditorSettings.MustLoad();
            string[] folders = settings.UIPrefabLoadFolders.Select(AssetDatabase.GetAssetPath).ToArray();

            GenData data = new GenData();
            data.Namespaces.Add(typeof(UIInfoContainer).Namespace);

            string[] guids = AssetDatabase.FindAssets("t:prefab", folders);
            foreach (string guid in guids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                GameObject uiPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);

                BasePage page = uiPrefab.GetComponent<BasePage>();
                if (page == null)
                    continue;

                UICodeGenAttribute attribute = page.GetType().GetCustomAttribute<UICodeGenAttribute>();
                if (attribute == null)
                    throw new Exception($"{page.GetType().Name} 脚本缺少 UICodeGenAttribute 特性");

                data.Namespaces.Add(page.GetType().Namespace);
                data.InfoItems.Add(new InfoItem(page.GetType().Name,
                    attribute.LayerName,
                    assetPath));
            }
            
            return data;
        }

        private static void ProcessGenData(GenData data)
        {
            data.Namespaces = data.Namespaces.ToHashSet().ToList();
            data.Namespaces.Sort();
            
            data.InfoItems.Sort((a, b) =>
            {
                int aLayerOrder = int.Parse(a.LayerOrder);
                int bLayerOrder = int.Parse(b.LayerOrder);
                
                if (aLayerOrder == bLayerOrder)
                    return string.Compare(a.PageType, b.PageType, StringComparison.Ordinal);
                
                return aLayerOrder.CompareTo(bLayerOrder);
            });
        }
    }
}