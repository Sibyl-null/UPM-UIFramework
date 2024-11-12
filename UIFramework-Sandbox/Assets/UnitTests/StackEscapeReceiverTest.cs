using System.Collections.Generic;
using NSubstitute;
using NUnit.Framework;
using UIFramework.Runtime.EscapeReceiver;
using UIFramework.Runtime.EventBus;
using UIFramework.Runtime.InfoContainer;
using UIFramework.Runtime.Page;
using UIFramework.Runtime.PageController;

namespace UnitTests
{
    public class StackEscapeReceiverTest
    {
        [Test]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        public void _07_ProcessEscape_With_Consume(int index)
        {
            // arrange
            IPageController controller = Substitute.For<IPageController>();
            IEventBus eventBus = Substitute.For<IEventBus>();
            StackEscapeReceiver strategy = new StackEscapeReceiver(controller, eventBus);

            List<IPage> pageList = new List<IPage>();
            for (int i = 0; i < 3; ++i)
            {
                IPage page = Substitute.For<IPage>();
                UIInfo info = new UIInfo(page.GetType(), default);

                page.InputActive.Returns(true);
                page.CanConsumeEscape().Returns(i <= index);
                controller.GetPage(info).Returns(page);

                strategy.Infos.Add(info);
                pageList.Add(page);
            }

            // act
            strategy.ProcessEscape();
            
            // assert
            for (int i = 0; i < 3; ++i)
            {
                IPage page = pageList[i];
                if (i == index)
                    page.Received(1).OnEscape();
                else
                    page.DidNotReceive().OnEscape();
            }
        }
        
        [Test]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        public void _08_ProcessEscape_With_InputActive(int index)
        {
            // arrange
            IPageController controller = Substitute.For<IPageController>();
            IEventBus eventBus = Substitute.For<IEventBus>();
            StackEscapeReceiver strategy = new StackEscapeReceiver(controller, eventBus);

            List<IPage> pageList = new List<IPage>();
            for (int i = 0; i < 3; ++i)
            {
                IPage page = Substitute.For<IPage>();
                UIInfo info = new UIInfo(page.GetType(), default);

                page.InputActive.Returns(i != index);
                page.CanConsumeEscape().Returns(false);
                controller.GetPage(info).Returns(page);

                strategy.Infos.Add(info);
                pageList.Add(page);
            }

            // act
            strategy.ProcessEscape();
            
            // assert
            for (int i = 0; i < 3; ++i)
            {
                IPage page = pageList[i];
                if (i <= index)
                    page.DidNotReceive().CanConsumeEscape();
                else
                    page.Received(1).CanConsumeEscape();
            }
        }
    }
}