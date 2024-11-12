using UIFramework.Runtime.InfoContainer;
using UIFramework.Runtime.Utility;
using UnityEngine;

namespace UIFramework.Runtime.Page
{
    public interface IPageArg
    {
    }
    
    public interface IPage
    {
        bool IsOpening { get; }
        bool IsPlayingAnim { get; }
        bool InputActive { get; }
        int Layer { get; }

        void Create(UIInfo info, string sortingLayerName, GameObject go);
        UIAsyncHandle Open(IPageArg arg = null);
        UIAsyncHandle Close(bool closeAnim);
        void Destroy();
        
        void PlayInAnimation(UIAsyncHandle handle);
        void PlayOutAnimation(UIAsyncHandle handle);
        
        void SetInputActive(bool isActive);
        void SetVisible(bool isVisible);
        void SetOrder(int order);

        void OnInit();
        void OnPrepare(IPageArg arg = null);
        void OnClear();
        void OnRelease();
        void AddEvent();
        void RemoveEvent();
        void InAnimComplete();
        void OnTick();

        void OnEscape();
        bool CanConsumeEscape();
    }
}