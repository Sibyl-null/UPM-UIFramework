using UI.Runtime.InfoContainer;
using UI.Runtime.Utility;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.Runtime.Page
{
    public abstract class BasePage : MonoBehaviour, IPage
    {
        private PageBehaviourLogic _behaviourLogic;
        
        public bool IsOpening => _behaviourLogic.IsOpening;
        public bool IsPlayingAnim => _behaviourLogic.IsPlayingAnim;
        public bool InputActive => EventSystem.current.enabled && !BaseUI.Raycast.raycastTarget;

        public UIInfo UIInfo { get; private set; }
        public BaseUI BaseUI { get; private set; }
        
        
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
            BaseUI.Raycast.raycastTarget = !isActive;
        }

        public virtual void SetVisible(bool isVisible)
        {
            BaseUI.Canvas.enabled = isVisible;
            BaseUI.Raycaster.enabled = isVisible;
        }
        
        public virtual void SetOrder(int order)
        {
            BaseUI.Canvas.sortingOrder = order;
        }


        // ------------------------------------------------------------------------
        // IPage 接口实现
        // ------------------------------------------------------------------------
        
        void IPage.Create(UIInfo info, string sortingLayerName)
        {
            UIInfo = info;
            
            BaseUI = GetComponent<BaseUI>();
            BaseUI.GetComponent<RectTransform>().NormalizeTransform();
            BaseUI.Canvas.overrideSorting = true;
            BaseUI.Canvas.sortingLayerName = sortingLayerName;

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
            GameObject.Destroy(BaseUI.gameObject);
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