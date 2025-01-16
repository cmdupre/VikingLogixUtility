using L5Sharp.Core;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Data;
using VikingLogixUtility.L5xApp.RegEx;
using VikingLogixUtility.Models;
using VikingLogixUtility.ViewModels;

namespace VikingLogixUtility.Common
{
    internal static class Helper
    {
        public static IComparer<RowViewModel> GetTagEditorComparer(IList<RowViewModel> rows, int index, bool forceAscending)
        {
            // Try to auto determine ascending/descending.

            var string0 = string.Empty;
            var string1 = string.Empty;

            int i = 0;
            while (i < (rows.Count - 1) && (string0 == string1))
            {
                string0 = rows[i].Properties[index].ReadValue.ToString();
                string1 = rows[i + 1].Properties[index].ReadValue.ToString();
                ++i;
            }

            var direction = string.Compare(string0, string1) < 0
                ? ListSortDirection.Descending
                : ListSortDirection.Ascending;

            if (double.TryParse(string0, out var double0) &&
                double.TryParse(string1, out var double1))
                direction = double0 < double1
                    ? ListSortDirection.Descending
                    : ListSortDirection.Ascending;

            return new TagEditorComparer(direction, index, forceAscending);
        }

        /// <summary>
        /// Used to get addresses when I/O is mapped in rungs.
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static IEnumerable<string> GetMappingRungs(L5X content) => content
            .Query<Rung>()
            .Select(r => r.Text.ToString())
            .Where(t => RegularExpressions.MappingFilter().IsMatch(t));

        public static void AddTagEditorHeaders(DataGrid? dataGrid, IEnumerable<RowViewModel> rows)
        {
            if (dataGrid is null)
                return;

            var columns = rows
                .First()
                .Properties
                .Select((x, i) => new { x.Name, Index = i });

            foreach (var column in columns)
            {
                App.Current.Dispatcher.Invoke(() =>
                {
                    dataGrid.Columns.Add(new CustomBoundColumn("CustomTemplate")
                    {
                        // Replace single underscore with double, DataGrid thinks it's an access key.
                        Header = column.Name.Replace("_", "__"),
                        Binding = new Binding($"Properties[{column.Index}]")
                    });
                });
            }
        }
    }
}
