using UI.Runtime.EscapeReceiver;
using UI.Runtime.EventBus;
using UI.Runtime.InfoContainer;
using UI.Runtime.LayerController;
using UI.Runtime.Page;
using UI.Runtime.PageController;
using UI.Runtime.PageFactory;
using UI.Runtime.QueueDriver;
using UI.Runtime.ResLoader;
using UI.Runtime.Utility;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.Runtime
{
    public abstract class AbstractUIManager
    {
        public Canvas UICanvas { get; private set; }
        public CanvasScaler UIScaler { get; private set; }
        public RectTransform Root { get; private set; }
        public EventSystem EventSystem { get; private set; }
        
        public UIRuntimeSettings Settings { get; protected set; }
        public UIInfoContainer InfoContainer { get; protected set; }

        public IEventBus EventBus { get; protected set; }
        
        protected IResLoader ResLoader { get; set; }
        protected ILayerController LayerController { get; set; }
        protected IPageFactory PageFactory { get; set; }
        protected IPageController PageController { get; set; }
        protected IEscapeReceiver EscapeReceiver { get; set; }
        protected IQueueDriver QueueDriver { get; set; }

        protected virtual void InitInternal(Canvas canvas, IResLoader resLoader)
        {
            UICanvas = canvas;
            UIScaler = canvas.GetComponent<CanvasScaler>();
            Root = canvas.GetComponent<RectTransform>();
            EventSystem = EventSystem.current;

            ResLoader = resLoader;
            Settings = resLoader.LoadSettings();
            EventBus = new UIEventBus();
            InfoContainer = new UIInfoContainer();
            LayerController = new UILayerController(new LayerControllerArg(Root, Settings));
            PageFactory = new MonoPageFactory(ResLoader, LayerController);
            PageController = new UIPageController(PageFactory, LayerController, EventBus, Settings.SortingLayerName);
            EscapeReceiver = new StackEscapeReceiver(PageController, EventBus);
            QueueDriver = new UIQueueDriver(PageController, EventBus);
        }

        protected virtual void ReleaseInternal()
        {
            QueueDriver.Release();
            EscapeReceiver.Release();
            PageController.Release();
            LayerController.Release();
            InfoContainer.Release();
            EventBus.Release();
        }

        public virtual void SetInputActive(bool flag)
        {
            EventSystem.enabled = flag;
        }

        protected IPage GetPage(int uiType)
        {
            if (!InfoContainer.TryGetInfo(uiType, out UIInfo info))
                return null;
            
            return PageController.GetPage(info);
        }

        protected T GetPage<T>(int? uiType = null) where T : class, IPage
        {
            if (!InfoContainer.TryGetInfo(typeof(T), uiType, out UIInfo info))
                return null;
            
            return PageController.GetPage(info) as T;
        }

        protected void CreatePage(int uiType)
        {
            if (!InfoContainer.TryGetInfo(uiType, out UIInfo info))
                return;

            PageController.CreatePage(info);
        }

        protected void CreatePage<T>(int? uiType = null) where T : class, IPage
        {
            if (!InfoContainer.TryGetInfo(typeof(T), uiType, out UIInfo info))
                return;

            PageController.CreatePage(info);
        }

        protected void OpenPage(int uiType, IPageArg arg = null)
        {
            if (!InfoContainer.TryGetInfo(uiType, out UIInfo info))
                return;

            PageController.OpenPage(info, arg);
        }
        
        protected void OpenPage<T>(int? uiType = null, IPageArg arg = null) where T : class, IPage
        {
            if (!InfoContainer.TryGetInfo(typeof(T), uiType, out UIInfo info))
                return;

            PageController.OpenPage(info, arg);
        }

        protected void ClosePage(int uiType, bool closeAnim = true)
        {
            if (!InfoContainer.TryGetInfo(uiType, out UIInfo info))
                return;

            PageController.ClosePage(info, closeAnim);
        }
        
        protected void ClosePage<T>(int? uiType = null, bool closeAnim = true) where T : class, IPage
        {
            if (!InfoContainer.TryGetInfo(typeof(T), uiType, out UIInfo info))
                return;

            PageController.ClosePage(info, closeAnim);
        }
        
        /** 禁止在 UI 的 OnTick 中移除 Page */
        protected void DestroyPage(int uiType, bool closeAnim = true)
        {
            if (!InfoContainer.TryGetInfo(uiType, out UIInfo info))
                return;

            UIAsyncHandle handle = PageController.DestroyPage(info, closeAnim);
            if (handle != null)
                handle.AddCompletedCallback(() => ResLoader.UnLoad(info.LoadPath));
        }
        
        protected void DestroyPage<T>(int? uiType = null, bool closeAnim = true) where T : class, IPage
        {
            if (!InfoContainer.TryGetInfo(typeof(T), uiType, out UIInfo info))
                return;

            UIAsyncHandle handle = PageController.DestroyPage(info, closeAnim);
            if (handle != null)
                handle.AddCompletedCallback(() => ResLoader.UnLoad(info.LoadPath));
        }
        
        protected void OpenQueuePage(int uiType, IPageArg arg = null, int policy = 0)
        {
            if (!InfoContainer.TryGetInfo(uiType, out UIInfo info))
                return;
            
            QueueDriver.EnqueueQueueInfo(info, arg, policy);
        }
        
        protected void OpenQueuePage<T>(int? uiType = null, IPageArg arg = null, int policy = 0) where T : class, IPage
        {
            if (!InfoContainer.TryGetInfo(typeof(T), uiType, out UIInfo info))
                return;
            
            QueueDriver.EnqueueQueueInfo(info, arg, policy);
        }
        
        public virtual void Tick()
        {
            foreach (IPage page in PageController.GetAllPages())
            {
                if (page.IsOpening && !page.IsPlayingAnim)
                    page.OnTick();
            }
        }

        public virtual void ReceiveEscape()
        {
            EscapeReceiver.ProcessEscape();
        }
    }
}