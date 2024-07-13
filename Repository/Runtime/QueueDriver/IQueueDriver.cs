using UIFramework.Runtime.InfoContainer;
using UIFramework.Runtime.Page;

namespace UIFramework.Runtime.QueueDriver
{
    public interface IQueueDriver
    {
        void EnqueueQueueInfo(UIInfo uiInfo, IPageArg arg, int policy);
        void Release();
    }
}