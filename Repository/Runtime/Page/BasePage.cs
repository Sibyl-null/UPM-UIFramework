using UIFramework.Runtime.InfoContainer;
using UIFramework.Runtime.Utility;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UIFramework.Runtime.Page
{
    [RequireComponent(typeof(Canvas), typeof(GraphicRaycaster))]
    public abstract class BasePage : MonoBehaviour, IPage
    {
        private PageBehaviourLogic _behaviourLogic;
        
        public bool IsOpening => _behaviourLogic.IsOpening;
        public bool IsPlayingAnim => _behaviourLogic.IsPlayingAnim;
        public bool InputActive => EventSystem.current.enabled && !UIRaycast.raycastTarget;

        protected UIInfo UIInfo { get; private set; }
        protected Canvas Canvas { get; private set; }
        protected GraphicRaycaster GraphicRaycaster { get; private set; }
        protected UIRaycast UIRaycast { get; private set; }
        
        public abstract int Layer { get; }
        
        
        // ------------------------------------------------------------------------
        // 生命周期
        // ------------------------------------------------------------------------
        
        protected virtual void OnInit()
        {
        }
        
        protected virtual void OnPrepare(IPageArg arg = null)
        {
        }

        protected virtual void OnClear()
        {
        }

        protected virtual void OnRelease()
        {
        }
        
        protected virtual void AddEvent()
        {
        }

        protected virtual void RemoveEvent()
        {
        }

        protected virtual void InAnimComplete()
        {
        }

        public virtual void OnTick()
        {
        }

        public virtual void OnEscape()
        {
        }

        public virtual bool CanConsumeEscape()
        {
            return true;
        }
        
        protected virtual void PlayInAnimation(UIAsyncHandle handle)
        {
            handle.SetCompleted();
        }

        protected virtual void PlayOutAnimation(UIAsyncHandle handle)
        {
            handle.SetCompleted();
        }
        
        public virtual void SetInputActive(bool isActive)
        {
            UIRaycast.raycastTarget = !isActive;
        }

        public virtual void SetVisible(bool isVisible)
        {
            Canvas.enabled = isVisible;
            GraphicRaycaster.enabled = isVisible;
        }
        
        public virtual void SetOrder(int order)
        {
            Canvas.sortingOrder = order;
        }


        // ------------------------------------------------------------------------
        // IPage 接口实现
        // ------------------------------------------------------------------------
        
        void IPage.Create(UIInfo info, string sortingLayerName, GameObject go)
        {
            UIInfo = info;
            Canvas = go.GetComponent<Canvas>();
            GraphicRaycaster = go.GetComponent<GraphicRaycaster>();
            UIRaycast = go.transform.Find("Raycast").GetComponent<UIRaycast>();
            
            GetComponent<RectTransform>().NormalizeTransform();
            Canvas.overrideSorting = true;
            Canvas.sortingLayerName = sortingLayerName;

            _behaviourLogic = new PageBehaviourLogic(this);
            _behaviourLogic.Create();
        }

        UIAsyncHandle IPage.Open(IPageArg arg)
        {
            return _behaviourLogic.Open(arg);
        }

        UIAsyncHandle IPage.Close(bool closeAnim)
        {
            return _behaviourLogic.Close(closeAnim);
        }

        void IPage.Destroy()
        {
            _behaviourLogic.Destroy();
            Object.Destroy(gameObject);
        }

        void IPage.PlayInAnimation(UIAsyncHandle handle)
        {
            PlayInAnimation(handle);
        }

        void IPage.PlayOutAnimation(UIAsyncHandle handle)
        {
            PlayOutAnimation(handle);
        }
        
        void IPage.OnInit()
        {
            OnInit();
        }

        void IPage.OnPrepare(IPageArg arg)
        {
            OnPrepare(arg);
        }

        void IPage.OnClear()
        {
            OnClear();
        }

        void IPage.OnRelease()
        {
            OnRelease();
        }

        void IPage.AddEvent()
        {
            AddEvent();
        }

        void IPage.RemoveEvent()
        {
            RemoveEvent();
        }

        void IPage.InAnimComplete()
        {
            InAnimComplete();
        }
    }
}