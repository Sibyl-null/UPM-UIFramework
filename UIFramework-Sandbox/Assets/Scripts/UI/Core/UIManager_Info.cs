// <auto-generated>
// 通过模版自动生成，仅允许手动删除 AddInfo 项以修复编译错误。
// </auto-generated>

using System;
using UIFramework.Runtime.InfoContainer;
using UI.TestOne;

namespace UI.Core
{
    public partial class UIManager
    {
        private void AddInfo(Type pageType, int layer, string loadPath)
        {
            InfoContainer.AddInfo(new UIInfo(pageType, layer, loadPath));
        }
        
        private void LoadInfos()
        {
            AddInfo(typeof(TestOnePage), Settings.LayerOrders["Dialog"], "Assets/Resources/TestOneUI.prefab");
        }
    }
}