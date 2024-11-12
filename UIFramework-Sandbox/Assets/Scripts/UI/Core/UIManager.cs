using UIFramework.Runtime;
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
                Debug.LogError("[UI] UIManager has already been created");
                return;
            }
            
            Instance = new UIManager();
            Instance.InitInternal(canvas, new UIResLoader(), new UILogger());
            Instance.LoadInfos();
        }
        
        public static void Destroy()
        {
            Instance.ReleaseInternal();
            Instance = null;
        }
    }
}
