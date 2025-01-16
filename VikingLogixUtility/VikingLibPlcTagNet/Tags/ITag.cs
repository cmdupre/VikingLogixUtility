using VikingLibPlcTagNet.Settings;

namespace VikingLibPlcTagNet.Tags
{
    public interface ITag : IDisposable
    {
        TagPath Path { get; }
        string FQN { get; }
        int Id { get; }
        string? Value { get; }
        string? PreviousValue { get; }
        bool Changed { get; }

        void Write(string value);
        void Read();
        void Toggle();
    }
}
