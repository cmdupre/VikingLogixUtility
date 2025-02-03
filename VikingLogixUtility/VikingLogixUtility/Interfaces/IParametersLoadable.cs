namespace VikingLogixUtility.Interfaces
{
    internal interface IParametersLoadable
    {
        bool IsRunning { get; }
        void LoadParameters();
    }
}
