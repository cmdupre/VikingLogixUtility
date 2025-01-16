using VikingLogixUtility.ViewModels;

namespace VikingLogixUtility.Models
{
    internal sealed class DisplayStringComparer() : IComparer<DisplayStringViewModel>
    {
        public int Compare(DisplayStringViewModel? x, DisplayStringViewModel? y)
        {
            if (x is null || y is null)
                return 0;

            if (double.TryParse(x.DisplayName, out var double0) &&
                double.TryParse(y.DisplayName, out var double1))
                return CompareDoubles(double0, double1);

            return string.Compare(x.DisplayName, y.DisplayName);
        }

        private static int CompareDoubles(double double0, double double1)
        {
            if (double0 < double1) return -1;
            if (double1 < double0) return 1;
            return 0;
        }
    }
}
