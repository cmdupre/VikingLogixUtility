namespace VikingLogixUtility.Interfaces
{
    internal interface ICancelable
    {
        bool IsRunning { get; }
        bool CancelRequested { set; }
    }
}
