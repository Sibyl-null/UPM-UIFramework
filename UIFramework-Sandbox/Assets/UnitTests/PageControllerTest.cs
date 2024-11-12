using System;
using NSubstitute;
using NUnit.Framework;
using UIFramework.Runtime.EventBus;
using UIFramework.Runtime.InfoContainer;
using UIFramework.Runtime.LayerController;
using UIFramework.Runtime.Page;
using UIFramework.Runtime.PageController;
using UIFramework.Runtime.PageFactory;
using UIFramework.Runtime.Utility;
using UnityEngine;
using EventType = UIFramework.Runtime.EventBus.EventType;

namespace UnitTests
{
    public class PageControllerTest
    {
        private const string SortingLayerName = "Default";
        
        private IPageFactory _pageFactory;
        private ILayerController _layerController;
        private UIPageController _pageController;
        private IEventBus _eventBus;
        
        private Type OneMockPageType => typeof(IOneMockPage);
        
        [SetUp]
        public void Setup()
        {
            IPage page = SubstitutePage();
            _pageFactory = Substitute.For<IPageFactory>();
            _pageFactory.CreatePage(default)
                .ReturnsForAnyArgs((page, new GameObject()));
            
            _layerController = Substitute.For<ILayerController>();
            _layerController.GetOrAddLayer(default)
                .ReturnsForAnyArgs(new GameObject().transform);
            
            _eventBus = Substitute.For<IEventBus>();

            _pageController = new UIPageController(_pageFactory, _layerController, _eventBus, SortingLayerName);
        }

        private static IPage SubstitutePage()
        {
            IPage page = Substitute.For<IPage>();
            page.Open().ReturnsForAnyArgs(new UIAsyncHandle());
            page.Close(default).ReturnsForAnyArgs(new UIAsyncHandle());
            return page;
        }

        [Test]
        public void _01_OpenPage_With_Page_FirstOpen()
        {
            // arrange
            UIInfo info = new UIInfo(OneMockPageType, default);
            
            // act
            UIAsyncHandle handle = _pageController.OpenPage(info);
            handle.SetCompleted();

            // assert
            IPage page = _pageController.GetPage(info);
            Assert.AreNotEqual(null, page);
            Assert.AreNotEqual(null, handle);
            
            _layerController.Received(1).AddPageInOrder(page);
            
            Received.InOrder(() =>
            {
                _eventBus.Dispatch(EventType.CreateBefore, info);
                page.Create(info, SortingLayerName, Arg.Any<GameObject>());
                _eventBus.Dispatch(EventType.CreateAfter, info);
                
                _eventBus.Dispatch(EventType.OpenBeforeAnim, info);
                page.Received(1).Open();
                _eventBus.Dispatch(EventType.OpenAfterAnim, info);
            });
        }

        [Test]
        public void _02_OpenPage_With_Page_CloseState()
        {
            // arrange
            IPage page = SubstitutePage();
            page.IsOpening.Returns(false);
            page.IsPlayingAnim.Returns(false);
            
            UIInfo info = new UIInfo(page.GetType(), default);
            _pageController.AddPageInDebug(page);

            // act
            UIAsyncHandle handle = _pageController.OpenPage(info);
            handle.SetCompleted();
            
            // assert
            Assert.AreNotEqual(null, handle);
            
            page.DidNotReceiveWithAnyArgs().Create(default, default, default);
            _layerController.Received(1).AddPageInOrder(page);
            
            _eventBus.DidNotReceive().Dispatch(EventType.CreateBefore, info);
            _eventBus.DidNotReceive().Dispatch(EventType.CreateAfter, info);
            
            Received.InOrder(() =>
            {
                _eventBus.Dispatch(EventType.OpenBeforeAnim, info);
                page.Open();
                _eventBus.Dispatch(EventType.OpenAfterAnim, info);
            });
        }

        [Test]
        public void _03_OpenPage_With_Page_OpenState()
        {
            // arrange
            IPage page = SubstitutePage();
            page.IsOpening.Returns(true);
            page.IsPlayingAnim.Returns(false);
            
            UIInfo info = new UIInfo(page.GetType(), default);
            _pageController.AddPageInDebug(page);

            // act
            UIAsyncHandle handle = _pageController.OpenPage(info);
            
            // assert
            Assert.AreEqual(null, handle);
            
            page.DidNotReceiveWithAnyArgs().Create(default, default, default);
            page.DidNotReceiveWithAnyArgs().Open();
            _layerController.DidNotReceiveWithAnyArgs().AddPageInOrder(default);
            
            _eventBus.DidNotReceive().Dispatch(EventType.CreateBefore, info);
            _eventBus.DidNotReceive().Dispatch(EventType.CreateAfter, info);
            _eventBus.DidNotReceive().Dispatch(EventType.OpenBeforeAnim, info);
            _eventBus.DidNotReceive().Dispatch(EventType.OpenAfterAnim, info);
        }
        
        [Test]
        public void _04_OpenPage_With_Page_AnimState()
        {
            // arrange
            IPage page = SubstitutePage();
            page.IsOpening.Returns(false);
            page.IsPlayingAnim.Returns(true);
            
            UIInfo info = new UIInfo(page.GetType(), default);
            _pageController.AddPageInDebug(page);

            // act
            UIAsyncHandle handle = _pageController.OpenPage(info);
            
            // assert
            Assert.AreEqual(null, handle);
            
            page.DidNotReceiveWithAnyArgs().Create(default, default, default);
            page.DidNotReceiveWithAnyArgs().Open();
            _layerController.DidNotReceiveWithAnyArgs().AddPageInOrder(default);
            
            _eventBus.DidNotReceive().Dispatch(EventType.CreateBefore, info);
            _eventBus.DidNotReceive().Dispatch(EventType.CreateAfter, info);
            _eventBus.DidNotReceive().Dispatch(EventType.OpenBeforeAnim, info);
            _eventBus.DidNotReceive().Dispatch(EventType.OpenAfterAnim, info);
        }
        
        // --------------------------------------------------------------------------------

        [Test]
        public void _05_ClosePage_With_Page_NotExist()
        {
            // arrange
            UIInfo info = new UIInfo(OneMockPageType, default);
            
            // act
            UIAsyncHandle handle = _pageController.ClosePage(info, default);
            
            // assert
            Assert.AreEqual(null, handle);
            _layerController.DidNotReceiveWithAnyArgs().RemovePageInOrder(default);
            _eventBus.DidNotReceive().Dispatch(EventType.CloseBeforeAnim, info);
            _eventBus.DidNotReceive().Dispatch(EventType.CloseAfterAnim, info);
        }

        [Test]
        public void _06_ClosePage_With_Page_OpenState()
        {
            // arrange
            IPage page = SubstitutePage();
            page.IsOpening.Returns(true);
            page.IsPlayingAnim.Returns(false);
            
            UIInfo info = new UIInfo(page.GetType(), default);
            _pageController.AddPageInDebug(page);

            // act
            UIAsyncHandle handle = _pageController.ClosePage(info, default);
            handle.SetCompleted();
            
            // assert
            page.DidNotReceiveWithAnyArgs().Destroy();
            _layerController.Received(1).RemovePageInOrder(page);
            
            Received.InOrder(() =>
            {
                _eventBus.Dispatch(EventType.CloseBeforeAnim, info);
                page.Close(default);
                _eventBus.Dispatch(EventType.CloseAfterAnim, info);
            });
        }

        [Test]
        public void _07_ClosePage_With_Page_CloseState()
        {
            // arrange
            IPage page = SubstitutePage();
            page.IsOpening.Returns(false);
            page.IsPlayingAnim.Returns(false);
            
            UIInfo info = new UIInfo(page.GetType(), default);
            _pageController.AddPageInDebug(page);

            // act
            UIAsyncHandle handle = _pageController.ClosePage(info, default);
            
            // assert
            Assert.AreEqual(null, handle);
            
            page.DidNotReceiveWithAnyArgs().Close(default);
            page.DidNotReceiveWithAnyArgs().Destroy();
            _layerController.DidNotReceiveWithAnyArgs().RemovePageInOrder(default);
            _eventBus.DidNotReceive().Dispatch(EventType.CloseBeforeAnim, info);
            _eventBus.DidNotReceive().Dispatch(EventType.CloseAfterAnim, info);
        }

        [Test]
        public void _08_ClosePage_With_Page_AnimState()
        {
            // arrange
            IPage page = SubstitutePage();
            page.IsOpening.Returns(true);
            page.IsPlayingAnim.Returns(true);
            
            UIInfo info = new UIInfo(page.GetType(), default);
            _pageController.AddPageInDebug(page);

            // act
            UIAsyncHandle handle = _pageController.ClosePage(info, default);
            
            // assert
            Assert.AreEqual(null, handle);
            
            page.DidNotReceiveWithAnyArgs().Close(default);
            page.DidNotReceiveWithAnyArgs().Destroy();
            _layerController.DidNotReceiveWithAnyArgs().RemovePageInOrder(default);
            _eventBus.DidNotReceive().Dispatch(EventType.CloseBeforeAnim, info);
            _eventBus.DidNotReceive().Dispatch(EventType.CloseAfterAnim, info);
        }
        
        // --------------------------------------------------------------------------------

        [Test]
        public void _09_DestroyPage_With_Page_NotExist()
        {
            // arrange
            UIInfo info = new UIInfo(OneMockPageType, default);
            
            // act
            UIAsyncHandle handle = _pageController.DestroyPage(info, default);
            
            // assert
            Assert.AreEqual(null, handle);
            _layerController.DidNotReceiveWithAnyArgs().RemovePageInOrder(default);
            
            _eventBus.DidNotReceive().Dispatch(EventType.CloseBeforeAnim, info);
            _eventBus.DidNotReceive().Dispatch(EventType.CloseAfterAnim, info);
            _eventBus.DidNotReceive().Dispatch(EventType.DestroyBefore, info);
            _eventBus.DidNotReceive().Dispatch(EventType.DestroyAfter, info);
        }

        [Test]
        public void _10_DestroyPage_With_Page_AnimState()
        {
            // arrange
            IPage page = SubstitutePage();
            page.IsPlayingAnim.Returns(true);
            
            UIInfo info = new UIInfo(page.GetType(), default);
            _pageController.AddPageInDebug(page);

            // act
            UIAsyncHandle handle = _pageController.DestroyPage(info, default);
            
            // assert
            Assert.AreEqual(null, handle);
            
            page.DidNotReceiveWithAnyArgs().Close(default);
            page.DidNotReceiveWithAnyArgs().Destroy();
            _layerController.DidNotReceiveWithAnyArgs().RemovePageInOrder(default);
            
            _eventBus.DidNotReceive().Dispatch(EventType.CloseBeforeAnim, info);
            _eventBus.DidNotReceive().Dispatch(EventType.CloseAfterAnim, info);
            _eventBus.DidNotReceive().Dispatch(EventType.DestroyBefore, info);
            _eventBus.DidNotReceive().Dispatch(EventType.DestroyAfter, info);
        }

        [Test]
        public void _11_DestroyPage_With_Page_OpenState()
        {
            // arrange
            IPage page = SubstitutePage();
            page.IsOpening.Returns(true);
            page.IsPlayingAnim.Returns(false);
            
            UIInfo info = new UIInfo(page.GetType(), default);
            _pageController.AddPageInDebug(page);

            // act
            UIAsyncHandle handle = _pageController.DestroyPage(info, default);
            handle.SetCompleted();
            
            // assert
            Assert.AreNotEqual(null, handle);
            
            _layerController.Received(1).RemovePageInOrder(page);
            
            Received.InOrder(() =>
            {
                _eventBus.Dispatch(EventType.CloseBeforeAnim, info);
                page.Close(default);
                _eventBus.Dispatch(EventType.CloseAfterAnim, info);
                
                _eventBus.Dispatch(EventType.DestroyBefore, info);
                page.Destroy();
                _eventBus.Dispatch(EventType.DestroyAfter, info);
            });
        }

        [Test]
        public void _12_DestroyPage_With_Page_CloseState()
        {
            // arrange
            IPage page = SubstitutePage();
            page.IsOpening.Returns(false);
            page.IsPlayingAnim.Returns(false);
            
            UIInfo info = new UIInfo(page.GetType(), default);
            _pageController.AddPageInDebug(page);

            // act
            UIAsyncHandle handle = _pageController.DestroyPage(info, default);
            
            // assert
            Assert.AreNotEqual(null, handle);
            
            page.DidNotReceiveWithAnyArgs().Close(default);
            _layerController.DidNotReceiveWithAnyArgs().RemovePageInOrder(default);
            
            _eventBus.DidNotReceive().Dispatch(EventType.CloseBeforeAnim, info);
            _eventBus.DidNotReceive().Dispatch(EventType.CloseAfterAnim, info);
            
            Received.InOrder(() =>
            {
                _eventBus.Dispatch(EventType.DestroyBefore, info);
                page.Received(1).Destroy();
                _eventBus.Dispatch(EventType.DestroyAfter, info);
            });
        }
        
        // --------------------------------------------------------------------------------

        [Test]
        public void _13_CreatePage_With_Page_NotExist()
        {
            // arrange
            UIInfo info = new UIInfo(OneMockPageType, default);
            
            // act
            IPage page = _pageController.CreatePage(info);
            
            // assert
            Assert.AreEqual(page, _pageController.GetPage(info));
            
            Received.InOrder(() =>
            {
                _eventBus.Dispatch(EventType.CreateBefore, info);
                page.Create(info, SortingLayerName, Arg.Any<GameObject>());
                _eventBus.Dispatch(EventType.CreateAfter, info);
            });
        }
        
        [Test]
        public void _14_CreatePage_With_Page_Exist()
        {
            // arrange
            IPage page = SubstitutePage();
            
            UIInfo info = new UIInfo(page.GetType(), default);
            _pageController.AddPageInDebug(page);
            
            // act
            IPage newPage = _pageController.CreatePage(info);
            
            // assert
            Assert.AreEqual(page, newPage);
            
            page.DidNotReceiveWithAnyArgs().Create(default, default, default);
            _eventBus.DidNotReceive().Dispatch(EventType.CreateBefore, info);
            _eventBus.DidNotReceive().Dispatch(EventType.CreateAfter, info);
        }
    }
}
