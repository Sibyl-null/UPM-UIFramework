using System.Collections.Generic;
using UI.Runtime.EventBus;
using UI.Runtime.InfoContainer;
using UI.Runtime.Page;
using UI.Runtime.PageController;
using UI.Runtime.Utility;

namespace UI.Runtime.EscapeReceiver
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
                    UILogger.Error($"[UI] 页面不存在: {UIUtility.LogUIType(infos[i].UIType)}" );
                    return;
                }
                
                if (page.InputActive == false)
                {
                    UILogger.Info($"[UI] {UIUtility.LogUIType(infos[i].UIType)} 页面禁用输入中，不处理返回键");
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