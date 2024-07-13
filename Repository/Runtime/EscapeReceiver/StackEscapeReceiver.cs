using System.Collections.Generic;
using UIFramework.Runtime.EventBus;
using UIFramework.Runtime.InfoContainer;
using UIFramework.Runtime.Page;
using UIFramework.Runtime.PageController;
using UIFramework.Runtime.Utility;

namespace UIFramework.Runtime.EscapeReceiver
{
    public class StackEscapeReceiver : IEscapeReceiver
    {
        private const int EventOrder = 0;
        
        private readonly IPageController _pageController;
        private readonly StackList<UIInfo> _infoStack = new StackList<UIInfo>();

        internal List<UIInfo> Infos => _infoStack.GetList();

        public StackEscapeReceiver(IPageController pageController, IEventBus eventBus)
        {
            _pageController = pageController;
            
            eventBus.Register(EventType.OpenBeforeAnim, OnOpenPageBeforeAnim, EventOrder);
            eventBus.Register(EventType.CloseAfterAnim, OnClosePageAfterAnim, EventOrder);
        }
        
        public void Release()
        {
            _infoStack.Clear();
        }

        private void OnOpenPageBeforeAnim(UIInfo info)
        {
            _infoStack.Push(info);
        }
        
        private void OnClosePageAfterAnim(UIInfo info)
        {
            _infoStack.Remove(info);
        }

        public void ProcessEscape()
        {
            List<UIInfo> infos = _infoStack.GetList();
            if (_infoStack.Count == 0)
            {
                UILogger.Info("[UI] 栈中无页面, 不处理返回键");
                return;
            }

            for (int i = infos.Count - 1; i >= 0; i--)
            {
                IPage page = _pageController.GetPage(infos[i]);
                
                if (page == null)
                {
                    UILogger.Error($"[UI] 页面不存在: {infos[i].PageType.Name}" );
                    return;
                }
                
                if (page.InputActive == false)
                {
                    UILogger.Info($"[UI] {infos[i].PageType.Name} 页面禁用输入中，不处理返回键");
                    return;
                }
                
                if (page.CanConsumeEscape())
                {
                    page.OnEscape();
                    return;
                }
            }
        }
    }
}