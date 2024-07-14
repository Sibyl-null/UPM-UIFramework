using System.Collections.Generic;
using UIFramework.Runtime;

namespace UIFramework.Editor.CodeGenerator
{
    internal static class PageGenerator
    {
        internal class GenData
        {
            public string UiLayer;
            public string LoadPath;
            public string PageClassName;
            public string UiClassName;
            public HashSet<string> DepNamespaceSet;
        }

        internal static void Generate(string savePath, GenData data)
        {
            UIEditorSettings settings = UIEditorSettings.MustLoad();
            string code = UIEditorUtility.ScribanGenerateText(settings.PageTemplate.text, data);
            
            UIEditorUtility.OverlayWriteTextFile(savePath, code);
            UILogger.Info("[UI] PageGenerator Finished");
        }
    }
}