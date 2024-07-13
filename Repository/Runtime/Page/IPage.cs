using UI.Runtime.InfoContainer;
using UI.Runtime.Utility;

namespace UI.Runtime.Page
{
    public interface IPageArg
    {
    }
    
    public interface IPage
    {
        bool IsOpening { get; }
        bool IsPlayingAnim { get; }
        bool InputActive { get; }

        void Create(UIInfo info, string sortingLayerName);
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