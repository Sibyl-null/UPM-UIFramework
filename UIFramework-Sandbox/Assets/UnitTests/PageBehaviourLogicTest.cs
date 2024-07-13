using NSubstitute;
using NUnit.Framework;
using UI.Runtime.Page;
using UI.Runtime.Utility;

namespace UnitTests
{
    public class PageBehaviourLogicTest
    {
        [Test]
        public void _01_Create_Behaviour_Call()
        {
            // arrange
            var page = Substitute.For<IPage>();
            var pageLogic = new PageBehaviourLogic(page);
            
            // act
            pageLogic.Create();
            
            // assert
            page.Received(1).OnInit();
            page.Received(1).SetVisible(false);
        }
        
        [Test]
        public void _02_Open_Behaviour_Call()
        {
            // arrange
            var page = Substitute.For<IPage>();
            var pageLogic = new PageBehaviourLogic(page);
            
            // act
            UIAsyncHandle handle = pageLogic.Open();
            handle.SetCompleted();

            // assert
            Received.InOrder(() =>
            {
                page.OnPrepare();
                page.SetVisible(true);
                page.SetInputActive(false);
                page.PlayInAnimation(handle);
                page.SetInputActive(true);
                page.AddEvent();
                page.InAnimComplete();
            });
            
            Assert.AreEqual(true, pageLogic.IsOpening);
            Assert.AreEqual(false, pageLogic.IsPlayingAnim);
        }
        
        [Test]
        public void _03_Close_NoAnim_Behaviour_Call()
        {
            // arrange
            var page = Substitute.For<IPage>();
            var pageLogic = new PageBehaviourLogic(page);
            
            // act
            UIAsyncHandle handle = pageLogic.Close(false);
            handle.SetCompleted();

            // assert
            Received.InOrder(() =>
            {
                page.RemoveEvent();
                page.SetInputActive(false);
                page.SetInputActive(true);
                page.SetVisible(false);
                page.OnClear();
            });
            
            Assert.AreEqual(false, pageLogic.IsOpening);
            Assert.AreEqual(false, pageLogic.IsPlayingAnim);
        }
        
        [Test]
        public void _04_Close_WithAnim_Behaviour_Call()
        {
            // arrange
            var page = Substitute.For<IPage>();
            var pageLogic = new PageBehaviourLogic(page);
            
            // act
            UIAsyncHandle handle = pageLogic.Close(true);
            handle.SetCompleted();

            // assert
            Received.InOrder(() =>
            {
                page.RemoveEvent();
                page.SetInputActive(false);
                page.PlayOutAnimation(handle);
                page.SetInputActive(true);
                page.SetVisible(false);
                page.OnClear();
            });
            
            Assert.AreEqual(false, pageLogic.IsOpening);
            Assert.AreEqual(false, pageLogic.IsPlayingAnim);
        }
            
        
        [Test]
        public void _05_Destroy_Behaviour_Call()
        {
            // arrange
            var page = Substitute.For<IPage>();
            var pageLogic = new PageBehaviourLogic(page);
            
            // act
            pageLogic.Destroy();
            
            // assert
            page.Received(1).OnRelease();
        }
    }
}