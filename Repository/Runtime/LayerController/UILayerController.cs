using System;
using System.Collections.Generic;
using System.Linq;
using UI.Runtime.Page;
using UI.Runtime.Utility;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UI.Runtime.LayerController
{
    public class UILayerController : ILayerController
    {
        private class LayerItem
        {
            public readonly Transform Trans;
            public readonly List<IPage> Pages = new List<IPage>();

            public LayerItem(Transform trans)
            {
                Trans = trans;
            }
        }
        
        private readonly LayerControllerArg _arg;
        private readonly Dictionary<int, LayerItem> _items = new Dictionary<int, LayerItem>(10);

        public UILayerController(LayerControllerArg arg)
        {
            _arg = arg;
            
#if UNITY_EDITOR
            if (SortingLayer.layers.Count(x => x.name == _arg.SortingLayerName) == 0)
            {
                throw new Exception($"[UI] SortingLayer 不存在: {_arg.SortingLayerName}");
            }
#endif
        }

        public void Release()
        {
            foreach (var layer in _items.Values)
                Object.Destroy(layer.Trans.gameObject);
            
            _items.Clear();
        }

        public void AddPageInOrder(int layer, IPage target)
        {
            LayerItem item = GetOrAddLayerItem(layer);

            if (item.Pages.Contains(target))
            {
                UILogger.Warning($"[UI] {target.GetType().Name} 在 {UIUtility.LogUILayer(layer)} 层级中已存在"); 
                return;
            }
            
            item.Pages.Add(target);
            
            int baseOrder = layer;
            foreach (IPage page in item.Pages)
            {
                page.SetOrder(baseOrder);
                baseOrder += _arg.PageOrderRange;
            }
        }

        public void RemovePageInOrder(int layer, IPage target)
        {
            LayerItem item = GetOrAddLayerItem(layer);

            if (!item.Pages.Contains(target))
            {
                UILogger.Warning($"[UI] {target.GetType().Name} 在 {UIUtility.LogUILayer(layer)} 层级中不存在"); 
                return;
            }
            
            item.Pages.Remove(target);
            
            int baseOrder = layer;
            foreach (IPage page in item.Pages)
            {
                page.SetOrder(baseOrder);
                baseOrder += _arg.PageOrderRange;
            }
        }

        public Transform GetOrAddLayer(int layer)
        {
            if (_items.TryGetValue(layer, out LayerItem item))
                return item.Trans;
            
            return AddLayer(layer);
        }

        public List<IPage> GetPagesInLayer(int layer)
        {
            LayerItem item = GetOrAddLayerItem(layer);
            return item.Pages;
        }
        
        private Transform AddLayer(int layer)
        {
            GameObject go = new GameObject();
            go.gameObject.name = UIUtility.LogUILayer(layer);
            go.layer = _arg.GoLayerValue;
            
            RectTransform rectTrans = go.AddComponent<RectTransform>();
            rectTrans.NormalizeTransform();
            go.transform.SetParent(_arg.Root, false);

            Canvas canvas = go.AddComponent<Canvas>();
            canvas.overrideSorting = true;
            canvas.sortingLayerName = _arg.SortingLayerName;
            canvas.sortingOrder = layer;
            
            _items.Add(layer, new LayerItem(rectTrans));
            return go.transform;
        }

        private LayerItem GetOrAddLayerItem(int layer)
        {
            GetOrAddLayer(layer);
            return _items[layer];
        }
    }
}