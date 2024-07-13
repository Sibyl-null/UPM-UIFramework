using UI.Runtime.Utility;

namespace UI.Runtime.Page
{
    public class PageBehaviourLogic
    {
        private readonly IPage _page;
        private readonly UIAsyncHandle _openHandle;
        private readonly UIAsyncHandle _closeHandle;
        
        public bool IsOpening { get; private set; }
        public bool IsPlayingAnim { get; private set; }
        
        public PageBehaviourLogic(IPage page)
        {
            _page = page;
            _openHandle = new UIAsyncHandle();
            _closeHandle = new UIAsyncHandle();
        }

        public void Create()
        {
            _page.OnInit();
            _page.SetVisible(false);
        }

        public UIAsyncHandle Open(IPageArg arg = null)
        {
            IsOpening = true;
            _page.OnPrepare(arg);
            
            _page.SetVisible(true);
            _page.SetInputActive(false);
            IsPlayingAnim = true;

            _openHandle.Reset();
            _openHandle.AddCompletedCallback(() =>
            {
                IsPlayingAnim = false;
                _page.SetInputActive(true);
                _page.AddEvent();
                _page.InAnimComplete();
            });

            _page.PlayInAnimation(_openHandle);
            return _openHandle;
        }

        public UIAsyncHandle Close(bool closeAnim)
        {
            _page.RemoveEvent();
            _page.SetInputActive(false);
            IsPlayingAnim = true;

            _closeHandle.Reset();
            _closeHandle.AddCompletedCallback(() =>
            {
                IsPlayingAnim = false;
                _page.SetInputActive(true);
                _page.SetVisible(false);
                _page.OnClear();

                IsOpening = false;
            });
            
            if (closeAnim)
                _page.PlayOutAnimation(_closeHandle);
            else
                _closeHandle.SetCompleted();
            
            return _closeHandle;
        }

        public void Destroy()
        {
            _page.OnRelease();
        }
    }
}