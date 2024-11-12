using System;
using UIFramework.Runtime.EscapeReceiver;
using UIFramework.Runtime.EventBus;
using UIFramework.Runtime.InfoContainer;
using UIFramework.Runtime.LayerController;
using UIFramework.Runtime.Page;
using UIFramework.Runtime.PageController;
using UIFramework.Runtime.PageFactory;
using UIFramework.Runtime.ResLoader;
using UIFramework.Runtime.Utility;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UIFramework.Runtime
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
        
        protected IUIResLoader ResLoader { get; set; }
        protected ILayerController LayerController { get; set; }
        protected IPageFactory PageFactory { get; set; }
        protected IPageController PageController { get; set; }
        protected IEscapeReceiver EscapeReceiver { get; set; }

        protected virtual void InitInternal(Canvas canvas, IUIResLoader resLoader)
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
        }

        protected virtual void ReleaseInternal()
        {
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

        public IPage GetPage(Type pageType)
        {
            if (!InfoContainer.TryGetInfo(pageType, out UIInfo info))
                return null;
            
            return PageController.GetPage(info);
        }

        public T GetPage<T>() where T : class, IPage
        {
            return GetPage(typeof(T)) as T;
        }

        public void CreatePage(Type pageType)
        {
            if (!InfoContainer.TryGetInfo(pageType, out UIInfo info))
                return;

            PageController.CreatePage(info);
        }

        public void CreatePage<T>() where T : class, IPage
        {
            CreatePage(typeof(T));
        }
        
        public void OpenPage(Type pageType, IPageArg arg = null)
        {
            if (!InfoContainer.TryGetInfo(pageType, out UIInfo info))
                return;

            PageController.OpenPage(info, arg);
        }
        
        public void OpenPage<T>(IPageArg arg = null) where T : class, IPage
        {
            OpenPage(typeof(T), arg);
        }
        
        public void ClosePage(Type pageType, bool closeAnim = true)
        {
            if (!InfoContainer.TryGetInfo(pageType, out UIInfo info))
                return;

            PageController.ClosePage(info, closeAnim);
        }
        
        public void ClosePage<T>(bool closeAnim = true) where T : class, IPage
        {
            ClosePage(typeof(T), closeAnim);
        }
        
        /** 禁止在 UI 的 OnTick 中移除 Page */
        public void DestroyPage(Type pageType, bool closeAnim = true)
        {
            if (!InfoContainer.TryGetInfo(pageType, out UIInfo info))
                return;

            UIAsyncHandle handle = PageController.DestroyPage(info, closeAnim);
            if (handle != null)
                handle.AddCompletedCallback(() => ResLoader.UnLoad(info.LoadPath));
        }
        
        /** 禁止在 UI 的 OnTick 中移除 Page */
        public void DestroyPage<T>(bool closeAnim = true) where T : class, IPage
        {
            DestroyPage(typeof(T), closeAnim);
        }

        public virtual void ReceiveEscape()
        {
            EscapeReceiver.ProcessEscape();
        }
    }
}