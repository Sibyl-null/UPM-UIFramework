namespace UIFramework.Runtime.Utility
{
    public interface IUILogger
    {
        void Info(string message);
        void Warning(string message);
        void Error(string message);
        string UILayerToString(int layer);
    }
}