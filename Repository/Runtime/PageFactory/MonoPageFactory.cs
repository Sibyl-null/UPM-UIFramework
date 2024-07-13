using UIFramework.Runtime.InfoContainer;
using UIFramework.Runtime.LayerController;
using UIFramework.Runtime.Page;
using UIFramework.Runtime.ResLoader;
using UnityEngine;

namespace UIFramework.Runtime.PageFactory
{
    public class MonoPageFactory : IPageFactory
    {
        private readonly IResLoader _resLoader;
        private readonly ILayerController _layerController;
        
        public MonoPageFactory(IResLoader resLoader, ILayerController layerController)
        {
            _resLoader = resLoader;
            _layerController = layerController;
        }
        
        public IPage CreatePage(UIInfo info)
        {
            if (typeof(IPage).IsAssignableFrom(info.PageType) == false)
            {
                UILogger.Error($"[UI] {info.PageType.Name} 未实现 IPage 接口");
                return null;
            }
            
            Transform parent = _layerController.GetOrAddLayer(info.Layer);
            
            GameObject go = _resLoader.LoadAndInstantiatePrefab(info.LoadPath, parent);
            if (go == null)
            {
                UILogger.Error($"[UI] UIPrefab 实例化失败: {info.PageType.Name}");
                return null;
            }
            
#if UNITY_EDITOR
            go.name = info.PageType.Name;
#endif

            IPage page = go.AddComponent(info.PageType) as IPage;
            if (page == null)
                return null;
            
            return page;
        }
    }
}