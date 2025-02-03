namespace VikingLogixUtility.Interfaces
{
    internal interface IUdtsLoadable
    {
        bool IsRunning { get; }
        void LoadUdts();
    }
}
