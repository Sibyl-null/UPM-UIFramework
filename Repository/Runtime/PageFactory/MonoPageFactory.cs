using UIFramework.Runtime.InfoContainer;
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
        private readonly IUILogger _logger;
        private readonly ILayerController _layerController;
        
        public MonoPageFactory(IUIResLoader resLoader, IUILogger logger, ILayerController layerController)
        {
            _resLoader = resLoader;
            _logger = logger;
            _layerController = layerController;
        }
        
        public (IPage page, GameObject go) CreatePage(UIInfo info)
        {
            if (typeof(IPage).IsAssignableFrom(info.PageType) == false)
            {
                _logger.Error($"[UI] {info.PageType.Name} 未实现 IPage 接口");
                return (null, null);
            }
            
            GameObject prefab = _resLoader.Load(info.LoadPath);
            if (prefab == null)
            {
                _logger.Error($"[UI] UIPrefab 加载失败失败: {info.LoadPath}");
                return (null, null);
            }

            GameObject go = Object.Instantiate(prefab);
            if (go == null)
            {
                _logger.Error($"[UI] UIPrefab 实例化失败: {info.LoadPath}");
                return (null, null);
            }
            
#if UNITY_EDITOR
            go.name = info.PageType.Name;
#endif

            IPage page = go.GetComponent<IPage>();
            if (page == null)
            {
                _logger.Error($"[UI] {go.name} 未添加实现 IPage 的组件");
                return (null, null);
            }
            
            Transform parent = _layerController.GetOrAddLayer(page.Layer);
            go.transform.SetParent(parent, false);

            return (page, go);
        }
    }
}