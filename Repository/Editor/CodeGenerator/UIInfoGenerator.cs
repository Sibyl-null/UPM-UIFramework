using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UIFramework.Runtime;
using UIFramework.Runtime.InfoContainer;
using UIFramework.Runtime.Page;
using UnityEditor;

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
            
            Type uiLayerType = Type.GetType(settings.UILayerAssemblyQualifiedName);
            if (uiLayerType == null)
                throw new Exception("[UI] EditorSettings: UILayerAssemblyQualifiedName is invalid.");

            GenData data = new GenData();
            data.NamespaceSet.Add(uiLayerType.Namespace);
            data.NamespaceSet.Add(typeof(UIInfoContainer).Namespace);

            List<Type> types = TypeCache.GetTypesWithAttribute<UICodeGenAttribute>().ToList();
            foreach (Type type in types)
            {
                if (typeof(IPage).IsAssignableFrom(type) == false)
                    throw new Exception($"UICodeGenAttribute 只能用在实现 IPage 接口的类上: {type.FullName}");
                
                data.NamespaceSet.Add(type.Namespace);
                data.InfoItems.Add(new InfoItem
                {
                    PageType = type.Name,
                    Layer = Enum.GetName(uiLayerType, type.GetCustomAttribute<UICodeGenAttribute>().Layer),
                    LoadPath = type.GetCustomAttribute<UICodeGenAttribute>().LoadPath
                });
            }

            data.InfoItems.Sort((a, b) =>
                String.Compare(a.PageType, b.PageType, StringComparison.Ordinal));
            return data;
        }
    }
}