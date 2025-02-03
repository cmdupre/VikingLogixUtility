namespace VikingLibPlcTagNet.Interfaces
{
    public interface ILoggable
    {
        void Log(string message);
        void Clear();
        string Text { get; }
        event EventHandler<EventArgs> TextChanged;
    }
}