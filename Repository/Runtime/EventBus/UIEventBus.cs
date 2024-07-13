using System;
using System.Collections.Generic;
using UI.Runtime.InfoContainer;

namespace UI.Runtime.EventBus
{
    public class UIEventBus : IEventBus
    {
        internal struct EventItem
        {
            public readonly Action<UIInfo> Action;
            public readonly int Order;

            public EventItem(Action<UIInfo> action, int order)
            {
                Action = action;
                Order = order;
            }
        }

        private readonly Dictionary<EventType, List<EventItem>> _itemDic = new Dictionary<EventType, List<EventItem>>();
        
        public void Release()
        {
            _itemDic.Clear();
        }
        
        public void Register(EventType eventType, Action<UIInfo> action, int order)
        {
            if (action == null)
                return;

            List<EventItem> items = GetOrAddItems(eventType);
            items.Add(new EventItem(action, order));
            items.Sort((a, b) => a.Order.CompareTo(b.Order));
        }
        
        public void Unregister(EventType eventType, Action<UIInfo> action)
        {
            if (action == null)
                return;
            
            List<EventItem> items = GetOrAddItems(eventType);
            int index = items.FindIndex(x => x.Action == action);
            if (index >= 0)
                items.RemoveAt(index);
        }

        public void Dispatch(EventType eventType, UIInfo uiInfo)
        {
            List<EventItem> items = GetOrAddItems(eventType);
            
            foreach (EventItem info in items)
            {
                info.Action?.Invoke(uiInfo);
            }
        }

        internal List<EventItem> GetOrAddItems(EventType eventType)
        {
            if (_itemDic.TryGetValue(eventType, out List<EventItem> items) == false)
            {
                items = new List<EventItem>();
                _itemDic.Add(eventType, items);
            }

            return items;
        }
    }
}