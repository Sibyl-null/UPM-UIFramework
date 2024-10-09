﻿using UIFramework.Runtime.InfoContainer;
using UIFramework.Runtime.LayerController;
using UIFramework.Runtime.Page;
using UIFramework.Runtime.ResLoader;
using UIFramework.Runtime.Utility;
using UnityEngine;

namespace UIFramework.Runtime.PageFactory
{
    public class MonoPageFactory : IPageFactory
    {
        private readonly IUIResLoader _resLoader;
        private readonly ILayerController _layerController;
        
        public MonoPageFactory(IUIResLoader resLoader, ILayerController layerController)
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

            IPage page = go.GetComponent<IPage>();
            if (page == null)
            {
                UILogger.Error($"[UI] {go.name} 未添加实现 IPage 组件");
                return (null, null);
            }

            return (page, go);
        }
    }
}