using UI.Runtime.InfoContainer;
using UI.Runtime.Page;

namespace UI.Runtime.QueueDriver
{
    public interface IQueueDriver
    {
        void EnqueueQueueInfo(UIInfo uiInfo, IPageArg arg, int policy);
        void Release();
    }
}