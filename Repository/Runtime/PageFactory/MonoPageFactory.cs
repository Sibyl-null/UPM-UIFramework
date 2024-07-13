using UI.Runtime.InfoContainer;
using UI.Runtime.LayerController;
using UI.Runtime.Page;
using UI.Runtime.ResLoader;
using UI.Runtime.Utility;
using UnityEngine;

namespace UI.Runtime.PageFactory
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
                UILogger.Error($"[UI] UIPrefab 实例化失败: {UIUtility.LogUIType(info.UIType)}");
                return null;
            }
            
#if UNITY_EDITOR
            go.name = UIUtility.LogUIType(info.UIType);
#endif

            IPage page = go.AddComponent(info.PageType) as IPage;
            if (page == null)
                return null;
            
            return page;
        }
    }
}