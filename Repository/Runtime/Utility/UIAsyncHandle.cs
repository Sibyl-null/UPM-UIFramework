using System;

namespace UI.Runtime.Utility
{
    public class UIAsyncHandle
    {
        private Action _onCompleted;
        private bool _isCompleted;
        
        public UIAsyncHandle(){}

        public UIAsyncHandle(bool isCompleted)
        {
            _isCompleted = isCompleted;
        }

        public void AddCompletedCallback(Action action)
        {
            if (action == null)
                return;

            if (_isCompleted)
            {
                action.Invoke();
                return;
            }

            _onCompleted += action;
        }

        public void SetCompleted()
        {
            _onCompleted?.Invoke();
            _onCompleted = null;
            _isCompleted = true;
        }

        public void Reset()
        {
            if (_onCompleted != null)
            {
                UILogger.Warning("[UI] Reset called on an incomplete async handle");
            }
            
            _isCompleted = false;
            _onCompleted = null;
        }
    }
}