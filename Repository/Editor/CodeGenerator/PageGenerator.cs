using System.Collections.Generic;
using UIFramework.Runtime;
using UIFramework.Runtime.Page;
using UnityEngine;

namespace UIFramework.Editor.CodeGenerator
{
    internal static class PageGenerator
    {
        private class GenData
        {
            public string SelfNamespace;
            public string PageClassName;
            public string UiClassName;
            public HashSet<string> DepNamespaceSet;
        }

        internal static void Generate(GameObject go)
        {
            UIEditorSettings settings = UIEditorSettings.MustLoad();
            
            GenData data = new GenData
            {
                SelfNamespace = $"{settings.RootNamespace}.{go.name.TrimUIEnd()}",
                PageClassName = go.name.TrimUIEnd() + "Page",
                UiClassName = go.name,
                DepNamespaceSet = new HashSet<string>()
                {
                    typeof(IPageArg).Namespace
                }
            };

            string code = UIEditorUtility.ScribanGenerateText(settings.PageTemplate.text, data);

            string savePath = $"{settings.RootGenFolder}/{go.name.TrimUIEnd()}/{data.PageClassName}.cs";
            UIEditorUtility.OverlayWriteTextFile(savePath, code);
            UILogger.Info("[UI] PageGenerator Finished");
        }
    }
}