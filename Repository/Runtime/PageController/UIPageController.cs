using System.Collections.Generic;
using System.Diagnostics;
using UI.Runtime.EventBus;
using UI.Runtime.InfoContainer;
using UI.Runtime.LayerController;
using UI.Runtime.Page;
using UI.Runtime.PageFactory;
using UI.Runtime.Utility;

namespace UI.Runtime.PageController
{
    public class UIPageController : IPageController
    {
        private readonly IPageFactory _pageFactory;
        private readonly ILayerController _layerController;
        private readonly IEventBus _eventBus;
        private readonly string _sortingLayerName;
        
        private readonly Dictionary<int, IPage> _pageDic = new Dictionary<int, IPage>(32);

        public UIPageController(IPageFactory pageFactory, ILayerController layerController, IEventBus eventBus, string sortingLayerName)
        {
            _pageFactory = pageFactory;
            _layerController = layerController;
            _eventBus = eventBus;
            _sortingLayerName = sortingLayerName;
        }

        public void Release()
        {
            foreach (var page in _pageDic.Values)
            {
                if (page.IsOpening)
                    page.Close(false);
                
                page.Destroy();
            }
            
            _pageDic.Clear();
        }
        
        public IPage GetPage(UIInfo info)
        {
            return _pageDic.GetValueOrDefault(info.UIType);
        }

        public IEnumerable<IPage> GetAllPages()
        {
            return _pageDic.Values;
        }

        public IPage CreatePage(UIInfo info)
        {
            IPage page = GetPage(info);
            if (page != null)
            {
                UILogger.Warning($"[UI] Page 已存在: {UIUtility.LogUIType(info.UIType)}");
                return page;
            }
            
            page = _pageFactory.CreatePage(info);
            if (page == null)
            {
                UILogger.Error($"[UI] Page 创建失败: {UIUtility.LogUIType(info.UIType)}");
                return null;
            }

            _pageDic.Add(info.UIType, page);
            
            _eventBus.Dispatch(EventType.CreateBefore, info);
            page.Create(info, _sortingLayerName);
            _eventBus.Dispatch(EventType.CreateAfter, info);
            
            return page;
        }

        public UIAsyncHandle OpenPage(UIInfo info, IPageArg arg = null)
        {
            IPage page = GetPage(info) ?? CreatePage(info);
            if (page == null)
            {
                return null;
            }

            if (page.IsOpening)
            {
                UILogger.Warning($"[UI] Page 已打开: {UIUtility.LogUIType(info.UIType)}");
                return null;
            }

            if (page.IsPlayingAnim)
            {
                UILogger.Warning($"[UI] Page 正在播放动画: {UIUtility.LogUIType(info.UIType)}");
                return null;
            }

            _layerController.AddPageInOrder(info.Layer, page);
            _eventBus.Dispatch(EventType.OpenBeforeAnim, info);
            
            UIAsyncHandle handle = page.Open(arg);
            handle.AddCompletedCallback(() => _eventBus.Dispatch(EventType.OpenAfterAnim, info));
            
            return handle;
        }

        public UIAsyncHandle ClosePage(UIInfo info, bool closeAnim)
        {
            IPage page = GetPage(info);
            if (page == null)
            {
                UILogger.Warning($"[UI] Page 不存在: {UIUtility.LogUIType(info.UIType)}");
                return null;
            }

            if (!page.IsOpening)
            {
                UILogger.Warning($"[UI] Page 已关闭: {UIUtility.LogUIType(info.UIType)}");
                return null;
            }
            
            if (page.IsPlayingAnim)
            {
                UILogger.Warning($"[UI] Page 正在播放动画: {UIUtility.LogUIType(info.UIType)}");
                return null;
            }

            _eventBus.Dispatch(EventType.CloseBeforeAnim, info);
            
            UIAsyncHandle handle = page.Close(closeAnim);
            handle.AddCompletedCallback(() =>
            {
                _layerController.RemovePageInOrder(info.Layer, page);
                _eventBus.Dispatch(EventType.CloseAfterAnim, info);
            });

            return handle;
        }

        public UIAsyncHandle DestroyPage(UIInfo info, bool closeAnim)
        {
            IPage page = GetPage(info);
            if (page == null)
            {
                UILogger.Warning($"[UI] Page 不存在: {UIUtility.LogUIType(info.UIType)}");
                return null;
            }
            
            if (page.IsPlayingAnim)
            {
                UILogger.Warning($"[UI] Page 正在播放动画: {UIUtility.LogUIType(info.UIType)}");
                return null;
            }

            if (page.IsOpening)
            {
                UIAsyncHandle handle = ClosePage(info, closeAnim);
                handle.AddCompletedCallback(() =>
                {
                    OnDestroyPage(page, info);
                });

                return handle;
            }
            
            OnDestroyPage(page, info);
            return new UIAsyncHandle(true);
        }

        private void OnDestroyPage(IPage page, UIInfo info)
        {
            _eventBus.Dispatch(EventType.DestroyBefore, info);
            page.Destroy();
            _eventBus.Dispatch(EventType.DestroyAfter, info);
            _pageDic.Remove(info.UIType);
        }
        

        [Conditional("UNITY_EDITOR")]
        internal void AddPageInDebug(int uiType, IPage page)
        {
            _pageDic.Add(uiType, page);
        }
    }
}