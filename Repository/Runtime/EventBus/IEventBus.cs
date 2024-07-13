using System;
using UIFramework.Runtime.InfoContainer;

namespace UIFramework.Runtime.EventBus
{
    public interface IEventBus
    {
        void Register(EventType eventType, Action<UIInfo> action, int order = 100);
        void Unregister(EventType eventType, Action<UIInfo> action);
        void Dispatch(EventType eventType, UIInfo uiInfo);

        void Release();
    }
}