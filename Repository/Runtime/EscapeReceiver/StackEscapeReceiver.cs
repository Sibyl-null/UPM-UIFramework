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

        private readonly IUILogger _logger;
        private readonly IPageController _pageController;
        private readonly StackList<UIInfo> _infoStack = new();

        internal List<UIInfo> Infos => _infoStack.GetList();

        public StackEscapeReceiver(IUILogger logger, IPageController pageController, IEventBus eventBus)
        {
            _logger = logger;
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
                _logger.Info("[UI] 栈中无页面, 不处理返回键");
                return;
            }

            UIInfo info = infos[^1];
            IPage page = _pageController.GetPage(info);
            
            if (page == null)
            {
                _logger.Error($"[UI] 页面不存在: {info.PageType.Name}" );
                return;
            }
                
            if (page.InputActive == false)
            {
                _logger.Info($"[UI] {info.PageType.Name} 页面禁用输入中，不处理返回键");
                return;
            }
                
            if (page.CanConsumeEscape)
                page.OnEscape();
        }
    }
}