using System;
using UI.Runtime.InfoContainer;

namespace UI.Runtime.EventBus
{
    public interface IEventBus
    {
        void Register(EventType eventType, Action<UIInfo> action, int order = 100);
        void Unregister(EventType eventType, Action<UIInfo> action);
        void Dispatch(EventType eventType, UIInfo uiInfo);

        void Release();
    }
}