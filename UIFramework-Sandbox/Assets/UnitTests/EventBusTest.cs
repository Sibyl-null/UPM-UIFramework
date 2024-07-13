using System;
using System.Collections.Generic;
using NSubstitute;
using NUnit.Framework;
using UIFramework.Runtime.EventBus;
using UIFramework.Runtime.InfoContainer;

namespace UnitTests
{
    public class EventBusTest
    {
        [Test]
        public void _01_RegisterEvent()
        {
            // arrange
            UIEventBus eventBus = new UIEventBus();
            List<UIEventBus.EventItem> items = eventBus.GetOrAddItems(EventType.CreateBefore);
            
            Action<UIInfo> action1 = Substitute.For<Action<UIInfo>>();
            Action<UIInfo> action2 = Substitute.For<Action<UIInfo>>();
            Action<UIInfo> action3 = Substitute.For<Action<UIInfo>>();
            
            // act
            eventBus.Register(EventType.CreateBefore, action1, 3);
            eventBus.Register(EventType.CreateBefore, action2, 1);
            eventBus.Register(EventType.CreateBefore, action3, 2);
            eventBus.Register(EventType.CreateBefore, null, 0);
            
            // assert
            Assert.AreEqual(3, items.Count);
            Assert.AreEqual(action2, items[0].Action);
            Assert.AreEqual(action3, items[1].Action);
            Assert.AreEqual(action1, items[2].Action);
        }
        
        [Test]
        public void _02_UnregisterEvent_With_NullAction()
        {
            // arrange
            UIEventBus eventBus = new UIEventBus();
            List<UIEventBus.EventItem> items = eventBus.GetOrAddItems(EventType.CreateBefore);
            
            Action<UIInfo> action1 = Substitute.For<Action<UIInfo>>();
            Action<UIInfo> action2 = Substitute.For<Action<UIInfo>>();
            
            items.Add(new UIEventBus.EventItem(action1, 0));
            items.Add(new UIEventBus.EventItem(action2, 1));
            
            // act
            eventBus.Unregister(EventType.CreateBefore, null);
            
            // assert
            Assert.AreEqual(2, items.Count);
            Assert.AreEqual(action1, items[0].Action);
            Assert.AreEqual(action2, items[1].Action);
        }

        [Test]
        public void _03_UnregisterEvent_With_NotExistAction()
        {
            // arrange
            UIEventBus eventBus = new UIEventBus();
            List<UIEventBus.EventItem> items = eventBus.GetOrAddItems(EventType.CreateBefore);
            
            Action<UIInfo> action1 = Substitute.For<Action<UIInfo>>();
            Action<UIInfo> action2 = Substitute.For<Action<UIInfo>>();
            
            items.Add(new UIEventBus.EventItem(action1, 0));
            items.Add(new UIEventBus.EventItem(action2, 1));
            
            // act
            eventBus.Unregister(EventType.CreateBefore, Substitute.For<Action<UIInfo>>());
            
            // assert
            Assert.AreEqual(2, items.Count);
            Assert.AreEqual(action1, items[0].Action);
            Assert.AreEqual(action2, items[1].Action);
        }

        [Test]
        public void _04_UnregisterEvent_With_ExistAction()
        {
            // arrange
            UIEventBus eventBus = new UIEventBus();
            List<UIEventBus.EventItem> items = eventBus.GetOrAddItems(EventType.CreateBefore);
            
            Action<UIInfo> action1 = Substitute.For<Action<UIInfo>>();
            Action<UIInfo> action2 = Substitute.For<Action<UIInfo>>();
            Action<UIInfo> action3 = Substitute.For<Action<UIInfo>>();
            
            items.Add(new UIEventBus.EventItem(action1, 0));
            items.Add(new UIEventBus.EventItem(action2, 1));
            items.Add(new UIEventBus.EventItem(action3, 2));
            
            // act
            eventBus.Unregister(EventType.CreateBefore, action2);
            
            // assert
            Assert.AreEqual(2, items.Count);
            Assert.AreEqual(action1, items[0].Action);
            Assert.AreEqual(action3, items[1].Action);
        }

        [Test]
        public void _05_DispatchEvent()
        {
            // arrange
            UIEventBus eventBus = new UIEventBus();
            List<UIEventBus.EventItem> items = eventBus.GetOrAddItems(EventType.CreateBefore);
            
            Action<UIInfo> action1 = Substitute.For<Action<UIInfo>>();
            Action<UIInfo> action2 = Substitute.For<Action<UIInfo>>();
            Action<UIInfo> action3 = Substitute.For<Action<UIInfo>>();
            
            items.Add(new UIEventBus.EventItem(action1, 0));
            items.Add(new UIEventBus.EventItem(action2, 1));
            items.Add(new UIEventBus.EventItem(action3, 2));

            UIInfo uiInfo = new UIInfo(default, default, default);

            // act
            eventBus.Dispatch(EventType.CreateBefore, uiInfo);
            
            // assert
            Received.InOrder(() =>
            {
                action1.Invoke(uiInfo);
                action2.Invoke(uiInfo);
                action3.Invoke(uiInfo);
            });
        }
    }
}