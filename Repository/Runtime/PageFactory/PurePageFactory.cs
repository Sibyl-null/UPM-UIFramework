using System;
using UIFramework.Runtime.InfoContainer;
using UIFramework.Runtime.LayerController;
using UIFramework.Runtime.Page;
using UIFramework.Runtime.ResLoader;
using UnityEngine;

namespace UIFramework.Runtime.PageFactory
{
    public class PurePageFactory : IPageFactory
    {
        private readonly IUIResLoader _resLoader;
        private readonly ILayerController _layerController;
        
        public PurePageFactory(IUIResLoader resLoader, ILayerController layerController)
        {
            _resLoader = resLoader;
            _layerController = layerController;
        }
        
        public (IPage page, GameObject go) CreatePage(UIInfo info)
        {
            if (typeof(IPage).IsAssignableFrom(info.PageType) == false)
            {
                UILogger.Error($"[UI] {info.PageType.Name} 未实现 IPage 接口");
                return (null, null);
            }
            
            Transform parent = _layerController.GetOrAddLayer(info.Layer);
            
            GameObject go = _resLoader.LoadAndInstantiatePrefab(info.LoadPath, parent);
            if (go == null)
            {
                UILogger.Error($"[UI] UIPrefab 实例化失败: {info.PageType.Name}");
                return (null, null);
            }
            
#if UNITY_EDITOR
            go.name = info.PageType.Name;
#endif

            IPage page =Activator.CreateInstance(info.PageType) as IPage;
            if (page == null)
                return (null, null);

            return (page, go);
        }
    }
}