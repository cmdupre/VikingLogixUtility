using VikingLibPlcTagNet.Settings;

namespace VikingLibPlcTagNet.Tags
{
    public interface ITag : IDisposable
    {
        TagPath Path { get; }
        string Name { get; }
        int Id { get; }
        object Value { get; }
        object PreviousValue { get; }
        bool Changed { get; }
        void Write(object value);
        void Read();
        void Toggle();
    }
}
