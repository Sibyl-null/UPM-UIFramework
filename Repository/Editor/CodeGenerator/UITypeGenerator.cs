using System;
using System.Collections.Generic;
using UIFramework.Runtime;

namespace UIFramework.Editor.CodeGenerator
{
    internal static class UITypeGenerator
    {
        private class GenData
        {
            public Dictionary<string, string> UiTypeDic = new Dictionary<string, string>();
        }

        internal static void Generate(string newUIType)
        {
            UIEditorSettings settings = UIEditorSettings.MustLoad();
            
            Type uiTypeType = Type.GetType(settings.UITypeAssemblyQualifiedName);
            if (uiTypeType == null)
                throw new Exception("[UI] EditorSettings: UITypeAssemblyQualifiedName is invalid.");
            
            int maxValue = -1;
            GenData data = new GenData();
            
            foreach (string name in Enum.GetNames(uiTypeType))
            {
                int value = (int)Enum.Parse(uiTypeType, name);
                maxValue = Math.Max(maxValue, value);
                
                data.UiTypeDic.Add(name, value.ToString());
            }
            
            if (string.IsNullOrEmpty(newUIType) == false)
                data.UiTypeDic.Add(newUIType, (maxValue + 1).ToString());

            string code = UIEditorUtility.ScribanGenerateText(settings.UITypeTemplate.text, data);
            UIEditorUtility.OverlayWriteTextFile(settings.UITypeFilePath, code);
            
            UILogger.Info("[UI] UIType 代码生成成功! " + settings.UIInfoFilePath);
        }
    }
}