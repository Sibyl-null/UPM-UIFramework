using System;
using System.Collections.Generic;
using UIFramework.Runtime.EventBus;
using UIFramework.Runtime.InfoContainer;
using UIFramework.Runtime.Page;
using UIFramework.Runtime.PageController;
using UIFramework.Runtime.Utility;

namespace UIFramework.Runtime.QueueDriver
{
    public struct QueueInfo
    {
        public readonly UIInfo UIInfo;
        public readonly IPageArg Arg;
        public readonly int Policy;

        public QueueInfo(UIInfo uiInfo, IPageArg arg, int policy)
        {
            UIInfo = uiInfo;
            Arg = arg;
            Policy = policy;
        }
    }
    
    public class UIQueueDriver : IQueueDriver
    {
        private const int EventOrder = 10;
        
        private readonly IPageController _pageController;
        private readonly List<QueueInfo> _infoList = new List<QueueInfo>();

        internal Type NowPageType { get; set; }
        internal List<QueueInfo> InfoList => _infoList;

        public UIQueueDriver(IPageController pageController, IEventBus eventBus)
        {
            _pageController = pageController;
            eventBus.Register(EventType.CloseAfterAnim, TryDequeueQueueInfo, EventOrder);
        }
        
        public void Release()
        {
            NowPageType = null;
            _infoList.Clear();
        }

        public void EnqueueQueueInfo(UIInfo uiInfo, IPageArg arg, int policy)
        {
            if (NowPageType == null && _infoList.Count == 0)
            {
                UIAsyncHandle handle = _pageController.OpenPage(uiInfo, arg);
                if (handle != null)
                    NowPageType = uiInfo.PageType;
                
                return;
            }

            _infoList.Add(new QueueInfo(uiInfo, arg, policy));
            _infoList.Sort((x, y) => y.Policy.CompareTo(x.Policy));
        }

        internal void TryDequeueQueueInfo(UIInfo info)
        {
            if (NowPageType == null || NowPageType != info.PageType)
                return;

            NowPageType = null;
            if (_infoList.Count == 0)
                return;

            QueueInfo queueInfo = _infoList[0];
            _infoList.RemoveAt(0);

            UIAsyncHandle handle = _pageController.OpenPage(queueInfo.UIInfo, queueInfo.Arg);
            if (handle != null)
                NowPageType = queueInfo.UIInfo.PageType;
        }
    }
}