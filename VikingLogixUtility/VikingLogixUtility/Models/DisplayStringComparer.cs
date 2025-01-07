using VikingLogixUtility.ViewModels;

namespace VikingLogixUtility.Models
{
    internal sealed class DisplayStringComparer() : IComparer<DisplayStringViewModel>
    {
        public int Compare(DisplayStringViewModel? x, DisplayStringViewModel? y)
        {
            if (x is null || y is null)
                return 0;

            return string.Compare(x.DisplayName, y.DisplayName);
        }
    }
}
