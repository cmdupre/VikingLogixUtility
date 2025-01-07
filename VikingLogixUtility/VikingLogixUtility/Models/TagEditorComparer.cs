using System.ComponentModel;
using VikingLogixUtility.ViewModels;

namespace VikingLogixUtility.Models
{
    internal sealed class TagEditorComparer(ListSortDirection direction, int index, bool forceAscending) : IComparer<RowViewModel>
    {
        public int Compare(RowViewModel? x, RowViewModel? y)
        {
            if (x is null || y is null)
                return 0;

            if (forceAscending)
                direction = ListSortDirection.Ascending;

            var string0 = x.Properties[index].ReadValue.ToString();
            var string1 = y.Properties[index].ReadValue.ToString();

            if (double.TryParse(string0, out var double0) &&
                double.TryParse(string1, out var double1))
                return CompareDoubles(double0, double1);

            return direction == ListSortDirection.Ascending
                ? string.Compare(string0, string1)
                : string.Compare(string1, string0);
        }

        private int CompareDoubles(double double0, double double1)
        {
            if (double0 == double1)
                return 0;

            if (double0 < double1)
                return direction == ListSortDirection.Ascending ? -1 : 1;

            return direction == ListSortDirection.Ascending ? 1 : -1;
        }
    }
}
