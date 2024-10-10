using Sirenix.OdinInspector;
using UI.Core;
using UI.Panels;
using UnityEngine;

public class Launcher : MonoBehaviour
{
    public Canvas root;
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            UIManager.Instance?.ReceiveEscape();
        
        UIManager.Instance?.Tick();
    }
    
    [Button]
    private void CreateUIManager()
    {
        UIManager.Create(root);
    }

    [Button]
    private void DestroyUIManager()
    {
        UIManager.Destroy();
    }
    

    // -----------------------------------------------------------------------------

    [Button, BoxGroup("TestOne")]
    private void CreateTestOnePage()
    {
        UIManager.Instance.CreatePage<TestOnePage>();
    }
    
    [Button, BoxGroup("TestOne")]
    private void OpenTestOnePage()
    {
        UIManager.Instance.OpenPage<TestOnePage>();
    }
    
    [Button, BoxGroup("TestOne")]
    private void CloseTestOnePage()
    {
        UIManager.Instance.ClosePage<TestOnePage>();
    }
    
    [Button, BoxGroup("TestOne")]
    private void DestroyTestOnePage()
    {
        UIManager.Instance.DestroyPage<TestOnePage>();
    }
    
    [Button, BoxGroup("TestOne")]
    private void OpenQueueTestOnePage()
    {
        UIManager.Instance.OpenQueuePage<TestOnePage>();
    }
    
    [Button, BoxGroup("TestOne")]
    private void CloseTestOnePageNoAnim()
    {
        UIManager.Instance.ClosePage<TestOnePage>(closeAnim: false);
    }
    
    [Button, BoxGroup("TestOne")]
    private void DestroyTestOnePageNoAnim()
    {
        UIManager.Instance.DestroyPage<TestOnePage>(closeAnim: false);
    }
    
    // -----------------------------------------------------------------------------
    
    [Button, BoxGroup("TestTwo")]
    private void CreateTestTwoPage()
    {
        UIManager.Instance.CreatePage<TestTwoPage>();
    }
    
    [Button, BoxGroup("TestTwo")]
    private void OpenTestTwoPage()
    {
        UIManager.Instance.OpenPage<TestTwoPage>();
    }
    
    [Button, BoxGroup("TestTwo")]
    private void CloseTestTwoPage()
    {
        UIManager.Instance.ClosePage<TestTwoPage>();
    }
    
    [Button, BoxGroup("TestTwo")]
    private void DestroyTestTwoPage()
    {
        UIManager.Instance.DestroyPage<TestTwoPage>();
    }
    
    [Button, BoxGroup("TestTwo")]
    private void OpenQueueTestTwoPage()
    {
        UIManager.Instance.OpenQueuePage<TestTwoPage>();
    }
    
    [Button, BoxGroup("TestTwo")]
    private void CloseTestTwoPageNoAnim()
    {
        UIManager.Instance.ClosePage<TestTwoPage>(false);
    }
    
    [Button, BoxGroup("TestTwo")]
    private void DestroyTestTwoPageNoAnim()
    {
        UIManager.Instance.DestroyPage<TestTwoPage>(false);
    }
}
