using System;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using NUnit.Framework;
using UIFramework.Runtime.EventBus;
using UIFramework.Runtime.InfoContainer;
using UIFramework.Runtime.Page;
using UIFramework.Runtime.PageController;
using UIFramework.Runtime.QueueDriver;
using UIFramework.Runtime.Utility;

namespace UnitTests
{
    public class QueueDriverTest
    {
        private IPageController _pageController;
        private IEventBus _eventBus;
        private UIQueueDriver _queueDriver;

        private Type OneMockPageType => typeof(IOneMockPage);
        private Type TwoMockPageType => typeof(ITwoMockPage);
        private Type ThreeMockPageType => typeof(IThreeMockPage);
        private Type FourMockPageType => typeof(IFourMockPage);
        
        [SetUp]
        public void Setup()
        {
            _pageController = Substitute.For<IPageController>();
            _eventBus = Substitute.For<IEventBus>();
            _queueDriver = new UIQueueDriver(_pageController, _eventBus);
        }

        [Test]
        public void _01_Ctor_Register_EventBus()
        {
            // assert
            _eventBus.Received(1).Register(EventType.CloseAfterAnim, _queueDriver.TryDequeueQueueInfo, Arg.Any<int>());
        }

        [Test]
        public void _02_EnqueueQueueInfo_With_EmptyQueue_OpenSuccess()
        {
            // arrange
            _pageController.OpenPage(default).ReturnsForAnyArgs(new UIAsyncHandle());
            UIInfo info = new UIInfo(OneMockPageType, default, default);
            IPageArg arg = Substitute.For<IPageArg>();
            
            // act
            _queueDriver.EnqueueQueueInfo(info, arg, 0);
            
            // assert
            _pageController.Received(1).OpenPage(info, arg);
            Assert.AreEqual(info.PageType, _queueDriver.NowPageType);
            Assert.AreEqual(0, _queueDriver.InfoList.Count);
        }

        [Test]
        public void _03_EnqueueQueueInfo_With_EmptyQueue_OpenFailure()
        {
            // arrange
            _pageController.OpenPage(default).ReturnsNullForAnyArgs();
            UIInfo info = new UIInfo(OneMockPageType, default, default);
            IPageArg arg = Substitute.For<IPageArg>();
            
            // act
            _queueDriver.EnqueueQueueInfo(info, arg, 0);
            
            // assert
            _pageController.Received(1).OpenPage(info, arg);
            Assert.AreEqual(null, _queueDriver.NowPageType);
            Assert.AreEqual(0, _queueDriver.InfoList.Count);
        }

        [Test]
        public void _04_EnqueueQueueInfo_With_PolicySort()
        {
            // arrange
            _pageController.OpenPage(default).ReturnsForAnyArgs(new UIAsyncHandle());
            _queueDriver.NowPageType = OneMockPageType;
            
            // act
            UIInfo info1 = new UIInfo(TwoMockPageType, default, default);
            UIInfo info2 = new UIInfo(ThreeMockPageType, default, default);
            UIInfo info3 = new UIInfo(FourMockPageType, default, default);
            
            _queueDriver.EnqueueQueueInfo(info1, null, 2);
            _queueDriver.EnqueueQueueInfo(info2, null, 1);
            _queueDriver.EnqueueQueueInfo(info3, null, 3);
            
            // assert
            Assert.AreEqual(3, _queueDriver.InfoList.Count);
            Assert.AreEqual(info3, _queueDriver.InfoList[0].UIInfo);
            Assert.AreEqual(info1, _queueDriver.InfoList[1].UIInfo);
            Assert.AreEqual(info2, _queueDriver.InfoList[2].UIInfo);
        }

        [Test]
        public void _05_TryDequeueQueueInfo_With_NowUIType_Is_Null()
        {
            // arrange
            _queueDriver.NowPageType = null;
            _queueDriver.InfoList.Add(new QueueInfo());
            
            // act
            _queueDriver.TryDequeueQueueInfo(new UIInfo(OneMockPageType, default, default));
            
            // assert
            Assert.AreEqual(1, _queueDriver.InfoList.Count);
            _pageController.DidNotReceiveWithAnyArgs().OpenPage(default);
        }
        
        [Test]
        public void _06_TryDequeueQueueInfo_With_NowUIType_Is_Not_Equal()
        {
            // arrange
            _queueDriver.NowPageType = OneMockPageType;
            _queueDriver.InfoList.Add(new QueueInfo());
            
            // act
            _queueDriver.TryDequeueQueueInfo(new UIInfo(TwoMockPageType, default, default));
            
            // assert
            Assert.AreEqual(1, _queueDriver.InfoList.Count);
            _pageController.DidNotReceiveWithAnyArgs().OpenPage(default);
        }

        [Test]
        public void _07_TryDequeueQueueInfo_With_NowUIType_Is_Equal()
        {
            // arrange
            _queueDriver.NowPageType = OneMockPageType;
            UIInfo info = new UIInfo(TwoMockPageType, default, default);
            _queueDriver.InfoList.Add(new QueueInfo(info, null, 0));

            // act
            _queueDriver.TryDequeueQueueInfo(new UIInfo(OneMockPageType, default, default));

            // assert
            Assert.AreEqual(0, _queueDriver.InfoList.Count);
            _pageController.Received(1).OpenPage(info);
        }

        [Test]
        public void _08_TryDequeueQueueInfo_With_OpenSuccess()
        {
            // arrange
            _queueDriver.NowPageType = OneMockPageType;
            UIInfo info = new UIInfo(TwoMockPageType, default, default);
            _queueDriver.InfoList.Add(new QueueInfo(info, null, 0));
            
            _pageController.OpenPage(default).ReturnsForAnyArgs(new UIAsyncHandle());

            // act
            _queueDriver.TryDequeueQueueInfo(new UIInfo(OneMockPageType, default, default));
            
            // assert
            Assert.AreEqual(0, _queueDriver.InfoList.Count);
            Assert.AreEqual(info.PageType, _queueDriver.NowPageType);
            _pageController.Received(1).OpenPage(info);
        }
        
        [Test]
        public void _09_TryDequeueQueueInfo_With_OpenFailure()
        {
            // arrange
            _queueDriver.NowPageType = OneMockPageType;
            UIInfo info = new UIInfo(TwoMockPageType, default, default);
            _queueDriver.InfoList.Add(new QueueInfo(info, null, 0));
            
            _pageController.OpenPage(default).ReturnsNullForAnyArgs();

            // act
            _queueDriver.TryDequeueQueueInfo(new UIInfo(OneMockPageType, default, default));
            
            // assert
            Assert.AreEqual(0, _queueDriver.InfoList.Count);
            Assert.AreEqual(null, _queueDriver.NowPageType);
            _pageController.Received(1).OpenPage(info);
        }
    }
}