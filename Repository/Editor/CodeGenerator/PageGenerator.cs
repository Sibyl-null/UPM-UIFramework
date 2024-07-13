using System.Collections.Generic;
using UI.Runtime;

namespace UI.Editor.CodeGenerator
{
    internal static class PageGenerator
    {
        internal class GenData
        {
            public string UiType;
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