using System.Collections.ObjectModel;
using VikingLogixUtility.Common;
using VikingLogixUtility.ViewModels;

namespace VikingLogixUtility.Extensions
{
    internal static class RowExtensions
    {
        public static ObservableCollection<RowViewModel> Sort(this ObservableCollection<RowViewModel> rows, int index, bool forceAscending = false)
        {
            var rowsAsList = rows.ToList();

            rowsAsList.Sort(Helper.GetTagEditorComparer(rowsAsList, index, forceAscending));

            return new ObservableCollection<RowViewModel>(rowsAsList);
        }
    }
}
