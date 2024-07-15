using System;
using System.Collections.Generic;
using System.Linq;
using UIFramework.Runtime;
using UIFramework.Runtime.InfoContainer;
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
            
            UILogger.Info("[UI] UIInfo 代码生成成功! " + settings.UIInfoFilePath);
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
                
                BaseUI baseUI = uiPrefab.GetComponent<BaseUI>();
                if (baseUI == null)
                    continue;
                
                data.NamespaceSet.Add(baseUI.GetType().Namespace);
                data.InfoItems.Add(new InfoItem
                {
                    PageType = uiPrefab.name.TrimUIEnd() + "Page",
                    Layer = baseUI.LayerName,
                    LoadPath = assetPath
                });
            }

            data.InfoItems.Sort((a, b) =>
                String.Compare(a.PageType, b.PageType, StringComparison.Ordinal));
            return data;
        }
    }
}