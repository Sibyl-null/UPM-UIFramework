using System;
using UnityEngine;

namespace UIFramework.Runtime.Utility
{
    public static class UILogger
    {
        private static Action<string> _infoReplaceAction;
        private static Action<string> _warningReplaceAction;
        private static Action<string> _errorReplaceAction;
        
        public static void Info(string message)
        {
            if (_infoReplaceAction != null)
            {
                _infoReplaceAction.Invoke(message);
                return;
            }
            
            Debug.Log(message);
        }
        
        public static void Warning(string message)
        {
            if (_warningReplaceAction != null)
            {
                _warningReplaceAction.Invoke(message);
                return;
            }
            
            Debug.LogWarning(message);
        }
        
        public static void Error(string message)
        {
            if (_errorReplaceAction != null)
            {
                _errorReplaceAction.Invoke(message);
                return;
            }
            
            Debug.LogError(message);
        }

        public static void RegisterInfoReplaceAction(Action<string> action)
        {
            _infoReplaceAction = action;
        }
        
        public static void RegisterWarningReplaceAction(Action<string> action)
        {
            _warningReplaceAction = action;
        }
        
        public static void RegisterErrorReplaceAction(Action<string> action)
        {
            _errorReplaceAction = action;
        }
    }
}