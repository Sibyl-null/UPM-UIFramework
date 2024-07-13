using UI.Runtime;
using UI.Runtime.Page;
using UI.Runtime.Utility;
using UnityEngine;

namespace UI.Core
{
    public partial class UIManager : AbstractUIManager
    {
        public static UIManager Instance { get; private set; }
        
        public static void Create(Canvas canvas)
        {
            if (Instance != null)
            {
                UILogger.Error("[UI] UIManager has already been created");
                return;
            }
            
            UIUtility.SetUITypeLogger(uiType => ((UIType)uiType).ToString());
            UIUtility.SetUILayerLogger(uiLayer => ((UILayer)uiLayer).ToString());
            
            Instance = new UIManager();
            Instance.InitInternal(canvas, new UIResLoader());
            Instance.LoadInfos();
        }
        
        public static void Destroy()
        {
            Instance.ReleaseInternal();
            Instance = null;
        }
        
        
        // ------------------------------------------------------------------------
        // 接口封装
        // ------------------------------------------------------------------------

        public IPage GetPage(UIType uiType)
        {
            return GetPage((int)uiType);
        }

        public T GetPage<T>(UIType? uiType = null) where T : class, IPage
        {
            return GetPage<T>(uiType != null ? (int)uiType : null);
        }
        
        public void CreatePage(UIType uiType)
        {
            CreatePage((int)uiType);
        }

        public void CreatePage<T>(UIType? uiType = null) where T : class, IPage
        {
            CreatePage<T>(uiType != null ? (int)uiType : null);
        }
        
        public void OpenPage(UIType uiType, IPageArg arg = null)
        {
            OpenPage((int)uiType, arg);
        }
        
        public void OpenPage<T>(UIType? uiType = null, IPageArg arg = null) where T : class, IPage
        {
            OpenPage<T>(uiType != null ? (int)uiType : null, arg);
        }

        public void ClosePage(UIType uiType, bool closeAnim = true)
        {
            ClosePage((int)uiType, closeAnim);
        }
        
        public void ClosePage<T>(UIType? uiType = null, bool closeAnim = true) where T : class, IPage
        {
            ClosePage<T>(uiType != null ? (int)uiType : null, closeAnim);
        }

        public void DestroyPage(UIType uiType, bool closeAnim = true)
        {
            DestroyPage((int)uiType, closeAnim);
        }
        
        public void DestroyPage<T>(UIType? uiType = null, bool closeAnim = true) where T : class, IPage
        {
            DestroyPage<T>(uiType != null ? (int)uiType : null, closeAnim);
        }

        public void OpenQueuePage(UIType uiType, IPageArg arg = null, int policy = 0)
        {
            OpenQueuePage((int)uiType, arg, policy);
        }

        public void OpenQueuePage<T>(UIType? uiType = null, IPageArg arg = null, int policy = 0) where T : class, IPage
        {
            OpenQueuePage<T>(uiType != null ? (int)uiType : null, arg, policy);
        }
    }
}
