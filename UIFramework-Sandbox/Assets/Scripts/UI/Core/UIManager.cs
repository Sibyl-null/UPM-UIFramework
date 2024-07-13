using UIFramework.Runtime;
using UIFramework.Runtime.Utility;
using UnityEngine;

namespace UI.Core
{
    public partial class UIManager : AbstractUIManager
    {
        public static UIManager Instance { get; private set; }
        
        public static void Create(Canvas canvas)
        {
            if (Instance != null)
            {
                UILogger.Error("[UI] UIManager has already been created");
                return;
            }
            
            UIUtility.SetUILayerLogger(uiLayer => ((UILayer)uiLayer).ToString());
            
            Instance = new UIManager();
            Instance.InitInternal(canvas, new UIResLoader());
            Instance.LoadInfos();
        }
        
        public static void Destroy()
        {
            Instance.ReleaseInternal();
            Instance = null;
        }
    }
}
