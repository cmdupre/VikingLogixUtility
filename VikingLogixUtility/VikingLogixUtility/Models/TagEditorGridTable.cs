using System.ComponentModel;
using System.Data;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using VikingLibPlcTagNet.Interfaces;
using VikingLogixUtility.Common;
using VikingLogixUtility.Extensions;
using VikingLogixUtility.ViewModels;

namespace VikingLogixUtility.Models
{
    internal sealed class TagEditorGridTable
    {
        private readonly DataGrid grid;
        private DataTable? table;

        /// <summary>
        /// Construct with default columns for predefined types.
        /// (Tag-Name, Value)
        /// </summary>
        /// <param name="viewDataGrid"></param>
        public TagEditorGridTable(DataGrid viewDataGrid)
        {
            grid = viewDataGrid;
            grid.AutoGenerateColumns = true;
            grid.CanUserAddRows = false;
            grid.CanUserResizeRows = false;
            grid.CanUserDeleteRows = true;
            grid.CanUserReorderColumns = false;
            grid.SelectionUnit = DataGridSelectionUnit.CellOrRowHeader;
            grid.EnableColumnVirtualization = true;
            grid.EnableRowVirtualization = true;
            grid.FrozenColumnCount = 2;
            grid.Sorting += Grid_Sorting;

            SetColumns(Constants.Value);
        }

        /// <summary>
        /// Sets read and write columns from a single column name.
        /// </summary>
        /// <param name="columns"></param>
        public void SetColumns(params string[] columns)
        {
            table = new DataTable();
            table.Columns.Add(Constants.Index);
            table.Columns.Add(Constants.TagName);

            foreach (var column in columns)
            {
                table.Columns.Add($"{column.Replace("_", "__")} {Constants.Read}");
                table.Columns.Add($"{column.Replace("_", "__")} {Constants.Write}");
            }

            grid.AutoGeneratingColumn += (s, e) =>
            {
                if (e.PropertyName == Constants.Index)
                {
                    e.Column.Visibility = Visibility.Hidden;
                    e.Column.IsReadOnly = true;
                    return;
                }

                if (e.PropertyName == Constants.TagName)
                {
                    e.Column.IsReadOnly = true;
                    ((DataGridBoundColumn)e.Column).CellStyle = GetReadonlyCellStyle(Brushes.Black);
                }

                if (e.PropertyName.Contains(Constants.Read))
                {
                    e.Column.IsReadOnly = true;
                    ((DataGridBoundColumn)e.Column).CellStyle = GetReadonlyCellStyle(Brushes.DarkGray);
                }

                e.Column.Visibility = grid.Items.Count < 1
                    ? Visibility.Hidden
                    : Visibility.Visible;
            };

            RefreshGrid(null);
        }

        public void Clear()
        {
            table?.Clear();
            RefreshGrid(null);
        }

        public void AddRow(string tagName, params string[] items)
        {
            if (table is null)
                return;

            if (string.IsNullOrWhiteSpace(tagName))
                return;

            // read and write columns for each item, plus hidden index and tag name columns
            var testItemsLength = (items.Length * 2) + 2;

            if (testItemsLength != table.Columns.Count)
                return;

            var newItems = new List<string>
            {
                table.Rows.Count.ToString(),
                tagName,
            };

            foreach (var item in items)
            {
                newItems.Add(item);
                newItems.Add(string.Empty);
            }

            table.Rows.Add([.. newItems]);

            RefreshGrid(null);
        }

        public void CopyToClipboard()
        {
            var textBuilder = new StringBuilder();

            var selectedCellsByRow = grid
                .SelectedCells
                .GroupBy(cellInfo => cellInfo.Item);

            foreach (var row in selectedCellsByRow)
            {
                var rowItem = row.Key;
                var rowValues = new List<string>();

                foreach (var cell in row)
                {
                    //if (cell.Column.IsReadOnly)
                    //    continue;

                    var cellValue = (cell.Column.GetCellContent(cell.Item) as TextBlock)?.Text ?? string.Empty;
                    rowValues.Add(cellValue);
                }

                textBuilder.AppendLine(string.Join("\t", rowValues));
            }

            Clipboard.SetText(textBuilder.ToString());
        }

        public void PasteFromClipboard(ILoggable? logger = null)
        {
            if (table is null)
                return;

            var text = Clipboard.GetText();

            if (string.IsNullOrWhiteSpace(text))
                return;

            var rowIndexXrf = new List<int>();

            foreach (DataRowView item in grid.Items)
            {
                var itemValue = item.Row.ItemArray[0];

                if (itemValue is not string itemString)
                {
                    logger?.Log($"Unable to cast grid item (hidden index) to string: {itemValue}");
                    return;
                }

                if (!int.TryParse(itemString, out int itemInt))
                {
                    logger?.Log($"Unable to cast grid item (hidden index) to Int32: {itemString}");
                    return;
                }

                rowIndexXrf.Add(itemInt);
            }

            var lines = text
                .Replace("\r", "")
                .TrimEnd('\n')
                .Split('\n');

            var rowIndex = grid.Items.IndexOf(grid.SelectedCells[0].Item);

            foreach (var line in lines)
            {
                var cells = line.Split('\t');
                var cellsIndex = 0;
                var columnIndex = grid.SelectedCells[0].Column.DisplayIndex;

                while (cellsIndex < cells.Length)
                {
                    if (columnIndex >= grid.Columns.Count)
                        break;

                    if (grid.Columns[columnIndex].IsReadOnly)
                    {
                        ++columnIndex;
                        continue;
                    }

                    table.Rows[rowIndexXrf[rowIndex]][columnIndex] = cells[cellsIndex];

                    ++cellsIndex;
                    ++columnIndex;
                }

                ++rowIndex;

                if (rowIndex >= table.Rows.Count)
                    break;
            }
        }

        public void Filter(string tagNameFilter) =>
            RefreshGrid($"[{Constants.TagName}] LIKE '%{tagNameFilter}%'");

        /// <summary>
        /// Gets the filtered view table for export.
        /// </summary>
        public DataTable GetExportTable()
        {
            var exportColumnNames = ReadonlyColumnNames;
            var exportRows = VisibleRows;

            var exportTable = new DataTable();

            foreach (var exportColumnName in exportColumnNames)
                exportTable.Columns.Add(exportColumnName.Raw());

            foreach (var exportRow in exportRows)
            {
                var row = new List<string>();

                foreach (var exportColumnName in exportColumnNames)
                    row.Add(exportRow[exportColumnName].ToString() ?? string.Empty);

                exportTable.Rows.Add([.. row]);
            }

            return exportTable;
        }

        /// <summary>
        /// Gets the visible (filtered) rows.
        /// </summary>
        public IEnumerable<DataRowView> VisibleRows =>
            grid.Items.Cast<DataRowView>();

        /// <summary>
        /// Gets a list of writeable column names.
        /// </summary>
        public IEnumerable<string> WriteableColumnNames =>
            table?
            .Columns
            .Cast<DataColumn>()
            .Select(c => c.ColumnName)
            .Where(n => n.Contains(Constants.Write))
            ?? [];

        /// <summary>
        /// Gets a list of readonly column names.
        /// </summary>
        public IEnumerable<string> ReadonlyColumnNames =>
            table?
            .Columns
            .Cast<DataColumn>()
            .Select(c => c.ColumnName)
            .Where(n => n != Constants.Index)
            .Where(n => !n.Contains(Constants.Write))
            ?? [];

        private static Style GetReadonlyCellStyle(Brush foregroundColor)
        {
            var style = new Style(typeof(DataGridCell));
            style.Setters.Add(new Setter(Control.BackgroundProperty, Brushes.WhiteSmoke));
            style.Setters.Add(new Setter(Control.ForegroundProperty, foregroundColor));
            return style;
        }

        private void RefreshGrid(string? filter)
        {
            DataView? view = table?.DefaultView;

            if (view is null)
                return;

            view.RowFilter = filter;

            grid.ItemsSource = null;
            grid.ItemsSource = view;
        }

        private void Grid_Sorting(object sender, DataGridSortingEventArgs e)
        {
            if (table is null)
                return;

            var sortDirection = e.Column.SortDirection == ListSortDirection.Ascending
                ? ListSortDirection.Descending
                : ListSortDirection.Ascending;

            var rows = table
                .AsEnumerable()
                .ToList();

            IEnumerable<DataRow> orderedRows;

            if (sortDirection == ListSortDirection.Ascending)
            {
                orderedRows = rows
                    .OrderBy(dataRow =>
                    new DisplayStringViewModel(dataRow.Field<string>(e.Column.SortMemberPath) ?? string.Empty));
            }
            else
            {
                orderedRows = rows
                    .OrderByDescending(dataRow =>
                    new DisplayStringViewModel(dataRow.Field<string>(e.Column.SortMemberPath) ?? string.Empty));
            }

            var orderedTable = table.Clone();

            foreach (var orderedRow in orderedRows)
                orderedTable.ImportRow(orderedRow);

            grid.ItemsSource = null;
            grid.ItemsSource = orderedTable.DefaultView;

            grid.Columns
                .First(c => c.SortMemberPath == e.Column.SortMemberPath)
                .SortDirection = sortDirection;

            e.Handled = true;
        }
    }
}
