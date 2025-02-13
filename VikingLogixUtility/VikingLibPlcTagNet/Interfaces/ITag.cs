using VikingLibPlcTagNet.Settings;

namespace VikingLibPlcTagNet.Interfaces
{
    public interface ITag : IDisposable
    {
        TagPath Path { get; }
        string FQN { get; }
        int Id { get; }
        string? Value { get; }
        string? PreviousValue { get; }
        bool Changed { get; }
        void Write(string value, ILoggable logger);
        void Read(ILoggable? logger);
        void Toggle(ILoggable logger);
    }
}
