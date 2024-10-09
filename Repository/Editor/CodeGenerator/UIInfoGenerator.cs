using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
            public string Layer;
            public string LoadPath;
        }
        
        private class GenData
        {
            public HashSet<string> NamespaceSet = new HashSet<string>();
            public List<InfoItem> InfoItems = new List<InfoItem>();
        }
        
        internal static void Generate()
        {
            GenData data = GetGenData();
            
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
            data.NamespaceSet.Add(typeof(UIInfoContainer).Namespace);

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

                data.NamespaceSet.Add(page.GetType().Namespace);
                data.InfoItems.Add(new InfoItem
                {
                    PageType = page.GetType().Name,
                    Layer = attribute.LayerName,
                    LoadPath = assetPath
                });
            }

            data.InfoItems.Sort((a, b) =>
                String.Compare(a.PageType, b.PageType, StringComparison.Ordinal));
            return data;
        }
    }
}