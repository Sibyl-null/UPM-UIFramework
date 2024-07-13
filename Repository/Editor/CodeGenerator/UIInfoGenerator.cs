using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UIFramework.Runtime;
using UIFramework.Runtime.InfoContainer;
using UnityEditor;

namespace UIFramework.Editor.CodeGenerator
{
    /** 自动生成 UIInfo 代码 */
    internal static class UIInfoGenerator
    {
        internal class InfoItem
        {
            public string UiType;
            public string Layer;
            public string PageType;
            public string LoadPath;
        }
        
        private class GenData
        {
            public HashSet<string> NamespaceSet = new HashSet<string>();
            public List<InfoItem> InfoItems = new List<InfoItem>();
        }

        internal static void GenerateWithNewInfo(InfoItem infoItem, string pageNamespace)
        {
            GenData data = GetGenData();
            data.NamespaceSet.Add(pageNamespace);
            data.InfoItems.Add(infoItem);
            
            UIEditorSettings settings = UIEditorSettings.MustLoad();
            string code = UIEditorUtility.ScribanGenerateText(settings.UIInfoTemplate.text, data);
            UIEditorUtility.OverlayWriteTextFile(settings.UIInfoFilePath, code);
            
            UILogger.Info("[UI] UIInfo 代码生成成功! " + settings.UIInfoFilePath);
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
            
            Type uiTypeType = Type.GetType(settings.UITypeAssemblyQualifiedName);
            if (uiTypeType == null)
                throw new Exception("[UI] EditorSettings: UITypeAssemblyQualifiedName is invalid.");
            
            Type uiLayerType = Type.GetType(settings.UILayerAssemblyQualifiedName);
            if (uiLayerType == null)
                throw new Exception("[UI] EditorSettings: UILayerAssemblyQualifiedName is invalid.");
            
            int[] uiTypes = (int[])Enum.GetValues(uiTypeType);
            List<Type> pageTypes = TypeCache.GetTypesWithAttribute<UICodeGenAttribute>().ToList();

            GenData data = new GenData();
            data.NamespaceSet.Add(uiTypeType.Namespace);
            data.NamespaceSet.Add(uiLayerType.Namespace);
            data.NamespaceSet.Add(typeof(UIInfoContainer).Namespace);
            
            foreach (Type pageType in pageTypes)
            {
                List<UICodeGenAttribute> genAttributes = pageType.GetCustomAttributes<UICodeGenAttribute>().ToList();
                foreach (UICodeGenAttribute genAttribute in genAttributes)
                {
                    if (uiTypes.Contains(genAttribute.UIType) == false)
                        continue;
                    
                    data.NamespaceSet.Add(pageType.Namespace);
                    data.InfoItems.Add(new InfoItem
                    {
                        UiType = Enum.GetName(uiTypeType, genAttribute.UIType),
                        Layer = Enum.GetName(uiLayerType, genAttribute.Layer),
                        PageType = pageType.Name,
                        LoadPath = genAttribute.LoadPath
                    });
                }
            }
            
            data.InfoItems.Sort((a, b) =>
            {
                int x = (int)Enum.Parse(uiTypeType, a.UiType);
                int y = (int)Enum.Parse(uiTypeType, b.UiType);
                return x.CompareTo(y);
            });

            return data;
        }
    }
}