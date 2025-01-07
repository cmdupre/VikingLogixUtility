namespace VikingLogixUtility.ViewModels
{
    internal sealed class DisplayStringViewModel(string name)
    {
        public string Name => name.Contains('[') && name.Contains(']')
            ? string.Empty
            : name;

        public string DisplayName => name;
    }
}
