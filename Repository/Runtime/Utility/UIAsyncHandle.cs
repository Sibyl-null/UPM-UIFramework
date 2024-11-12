using System;

namespace UIFramework.Runtime.Utility
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
            _isCompleted = false;
            _onCompleted = null;
        }
    }
}