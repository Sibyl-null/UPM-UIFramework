using System.Collections.Generic;
using NSubstitute;
using NUnit.Framework;
using UIFramework.Runtime.LayerController;
using UIFramework.Runtime.Page;
using UnityEngine;

namespace UnitTests
{
    public class LayerControllerTest
    {
        private Transform _root;

        private const int BottomLayer = 0;
        private const int MiddleLayer = 1000;
        private const int TopLayer = 2000;
        
        [SetUp]
        public void Setup()
        {
            GameObject go = new GameObject();
            _root = go.transform;
        }

        [Test]
        [TestCase(0)]
        [TestCase(5)]
        public void _01_AddPageInOrder_With_PageNotExist(int count)
        {
            // arrange
            var arg = new LayerControllerArg(_root, 5, "Default", 50);
            UILayerController controller = new UILayerController(arg);

            List<IPage> pagesInLayer = controller.GetPagesInLayer(BottomLayer);
            for (int i = 0; i < count; ++i)
                pagesInLayer.Add(Substitute.For<IPage>());

            // act
            controller.AddPageInOrder(BottomLayer, Substitute.For<IPage>());
            
            // assert
            Assert.AreEqual(count + 1, pagesInLayer.Count);
            
            int order = BottomLayer;
            foreach (var page in pagesInLayer)
            {
                page.Received(1).SetOrder(order);
                order += arg.PageOrderRange;
            }
        }
        
        [Test]
        public void _02_AddPageInOrder_With_PageExist()
        {
            // arrange
            var arg = new LayerControllerArg(_root, 5, "Default", 50);
            UILayerController controller = new UILayerController(arg);

            List<IPage> pagesInLayer = controller.GetPagesInLayer(BottomLayer);
            var target = Substitute.For<IPage>();

            pagesInLayer.Add(Substitute.For<IPage>());
            pagesInLayer.Add(Substitute.For<IPage>());
            pagesInLayer.Add(target);
            pagesInLayer.Add(Substitute.For<IPage>());
            
            // act
            controller.AddPageInOrder(BottomLayer, target);
            
            // assert
            Assert.AreEqual(4, pagesInLayer.Count);
            
            foreach (var page in pagesInLayer)
            {
                page.DidNotReceiveWithAnyArgs().SetOrder(default);
            }
        }

        [Test]
        public void _03_RemovePageInOrder_With_PageNotExist()
        {
            // arrange
            var arg = new LayerControllerArg(_root, 5, "Default", 50);
            UILayerController controller = new UILayerController(arg);

            List<IPage> pagesInLayer = controller.GetPagesInLayer(BottomLayer);
            pagesInLayer.Add(Substitute.For<IPage>());
            pagesInLayer.Add(Substitute.For<IPage>());
            pagesInLayer.Add(Substitute.For<IPage>());
            
            // act
            controller.RemovePageInOrder(BottomLayer, Substitute.For<IPage>());
            
            // assert
            Assert.AreEqual(3, pagesInLayer.Count);
            
            foreach (var page in pagesInLayer)
            {
                page.DidNotReceiveWithAnyArgs().SetOrder(default);
            }
        }
        
        [Test]
        public void _04_RemovePageInOrder_With_PageExist()
        {
            // arrange
            var arg = new LayerControllerArg(_root, 5, "Default", 50);
            UILayerController controller = new UILayerController(arg);

            List<IPage> pagesInLayer = controller.GetPagesInLayer(BottomLayer);
            var target = Substitute.For<IPage>();
            
            pagesInLayer.Add(Substitute.For<IPage>());
            pagesInLayer.Add(Substitute.For<IPage>());
            pagesInLayer.Add(target);
            pagesInLayer.Add(Substitute.For<IPage>());
            
            // act
            controller.RemovePageInOrder(BottomLayer, target);
            
            // assert
            Assert.AreEqual(3, pagesInLayer.Count);
            
            int order = BottomLayer;
            foreach (var page in pagesInLayer)
            {
                page.Received(1).SetOrder(order);
                order += arg.PageOrderRange;
            }
        } 
    }
}
