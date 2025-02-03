using System.Collections.ObjectModel;
using System.Data;
using System.Windows.Controls;
using System.Windows.Input;
using VikingLibPlcTagNet.Data;
using VikingLibPlcTagNet.Interfaces;
using VikingLibPlcTagNet.Settings;
using VikingLogixUtility.Bases;
using VikingLogixUtility.Commands;
using VikingLogixUtility.Common;
using VikingLogixUtility.Extensions;
using VikingLogixUtility.Interfaces;
using VikingLogixUtility.Models;
using VikingLogixUtility.Processors;

namespace VikingLogixUtility.ViewModels
{
    internal sealed class UdtTabViewModel : BaseNotifyPropertyChanged,
        IDisposable,
        IUdtsLoadable,
        IParametersLoadable,
        ITagEditorLoadable,
        ICancelable,
        IExportable,
        IWriteable
    {
        private readonly Logger logger = new();
        private readonly TagEditorGridTable tagEditorGridTable;
        private PlcInfo? plcInfo = null;
        private bool isRunning = false;
        private Cursor saveCursor = Mouse.OverrideCursor;
        private string address = string.Empty;
        private ObservableCollection<DisplayStringViewModel> scopeItems = [];
        private ObservableCollection<DisplayStringViewModel> udtItems = [];
        private ObservableCollection<DisplayStringViewModel> parameterItems = [];
        private DisplayStringViewModel? scopeSelectedItem = null;
        private DisplayStringViewModel? udtSelectedItem = null;
        private IEnumerable<DisplayStringViewModel>? parameterSelectedItems = null;
        private string filterText = string.Empty;
        private bool cancelRequested = false;

        public void Dispose() => plcInfo?.Dispose();

        public UdtTabViewModel(DataGrid viewDataGrid)
        {
            var settings = Settings.Load();

            Address = settings.Address;

            tagEditorGridTable = new(viewDataGrid);

            logger.TextChanged +=
                ((sender, e) => { NotifyPropertyChanged(nameof(Output)); });
        }

        public ICommand ScopeItemsSelectionChanged => new LoadUdtsCommand(this);
        public ICommand UdtItemsSelectionChanged => new LoadParametersCommand(this);
        public ICommand ReadClicked => new ReadCommand(this);
        public ICommand WriteClicked => new WriteCommand(this);
        public ICommand CancelClicked => new CancelCommand(this);
        public ICommand ExportClicked => new TagEditorExportCommand(this);

        public TagEditorGridTable TagEditorGridTable => tagEditorGridTable;
        public ILoggable Logger => logger;

        public bool IsRunning
        {
            get => isRunning;

            set
            {
                isRunning = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(IsNotRunning));
                CommandManager.InvalidateRequerySuggested();

                if (IsRunning)
                {
                    saveCursor = Mouse.OverrideCursor;
                    Mouse.OverrideCursor = Cursors.Wait;
                    logger.Clear();
                }
                else
                {
                    Mouse.OverrideCursor = saveCursor;
                }
            }
        }

        public bool IsNotRunning => !IsRunning;

        public string Output => logger.Text;

        public string Address
        {
            get => address;

            set
            {
                address = value;
                NotifyPropertyChanged();
            }
        }

        public ObservableCollection<DisplayStringViewModel> ScopeItems
        {
            get => scopeItems;

            set
            {
                scopeItems = value;
                NotifyPropertyChanged();
            }
        }

        public ObservableCollection<DisplayStringViewModel> UdtItems
        {
            get => udtItems;

            set
            {
                udtItems = value;
                NotifyPropertyChanged();
            }
        }

        public ObservableCollection<DisplayStringViewModel> ParameterItems
        {
            get => parameterItems;

            set
            {
                parameterItems = value;
                NotifyPropertyChanged();
            }
        }

        public DisplayStringViewModel? ScopeSelectedItem
        {
            get => scopeSelectedItem;

            set
            {
                scopeSelectedItem = value;
                NotifyPropertyChanged();
            }
        }

        public DisplayStringViewModel? UdtSelectedItem
        {
            get => udtSelectedItem;

            set
            {
                udtSelectedItem = value;
                NotifyPropertyChanged();
            }
        }

        public IEnumerable<DisplayStringViewModel>? ParameterSelectedItems
        {
            get => parameterSelectedItems;

            set
            {
                parameterSelectedItems = value;
                NotifyPropertyChanged();

                ClearTagEditor();

                if (value is null || !value.Any())
                    return;
            }
        }

        public string FilterText
        {
            get => filterText;

            set
            {
                filterText = value;
                NotifyPropertyChanged();
            }
        }

        public bool CancelRequested
        {
            get => cancelRequested;
            set => cancelRequested = value;
        }

        public async void LoadScopes()
        {
            if (IsRunning)
                return;

            IsRunning = true;

            logger.Log("Loading PLC data...");

            await Task.Run(() =>
            {
                try
                {
                    LoadScopesBackground();
                }
                catch (Exception ex)
                {
                    logger.Log($"Error: {ex.Message}");
                }
            });

            logger.Log("Done.");

            IsRunning = false;
        }

        public void LoadUdts()
        {
            UdtItems.Clear();
            ParameterItems.Clear();

            if (plcInfo is null ||
                scopeSelectedItem is null)
                return;

            foreach (var udt in plcInfo.GetTemplateNamesFor(scopeSelectedItem.Name))
                UdtItems.Add(new DisplayStringViewModel(udt));

            UdtItems = UdtItems.Sort();
        }

        public void LoadParameters()
        {
            ClearTagEditor();
            ParameterItems.Clear();

            if (plcInfo is null ||
                ScopeSelectedItem is null ||
                UdtSelectedItem is null)
                return;

            foreach (var parameter in plcInfo.GetFieldNamesFor(ScopeSelectedItem.Name, UdtSelectedItem.Name))
                ParameterItems.Add(new DisplayStringViewModel(parameter));

            ParameterItems = ParameterItems.Sort();
        }

        public async void LoadTagEditor()
        {
            if (IsRunning)
                return;

            IsRunning = true;

            logger.Log("Loading tag editor...");

            await Task.Run(() =>
            {
                try
                {
                    LoadTagEditorBackground();
                }
                catch (Exception ex)
                {
                    var message = ex.Message;

                    if (message == "Sequence contains no elements")
                        message = "No Tags Available.";

                    logger.Log($"{message}");
                }
            });

            logger.Log("Done.");

            IsRunning = false;
        }

        public async void WriteTags()
        {
            if (IsRunning)
                return;

            IsRunning = true;

            await Task.Run(() =>
            {
                try
                {
                    WriteTagsBackground();
                    LoadTagEditorBackground(FilterText);
                }
                catch (Exception ex)
                {
                    logger.Log($"Error: {ex.Message}");
                }
            });

            IsRunning = false;
        }

        public void FilterTags() => TagEditorGridTable.Filter(FilterText);

        private void LoadScopesBackground()
        {
            using var listing = TagListing.Create(logger, new TagPath(Address));

            if (listing is null)
                return;

            if (Cancel())
                return;

            plcInfo = PlcInfo.Build(logger, listing, Cancel);

            if (plcInfo is null)
                return;

            App.Current.Dispatcher.Invoke(() =>
                ScopeItems.Add(new DisplayStringViewModel(Constants.Controller)));

            foreach (var programName in plcInfo.ProgramNames)
                App.Current.Dispatcher.Invoke(() =>
                    ScopeItems.Add(new DisplayStringViewModel(programName)));
        }

        private bool Cancel()
        {
            if (CancelRequested)
            {
                logger.Log("Cancel requested...");

                CancelRequested = false;

                return true;
            }

            return false;
        }

        private void LoadTagEditorBackground(string? filterText = null)
        {
            if (plcInfo is null)
                return;

            if (ScopeSelectedItem is null)
                return;

            if (UdtSelectedItem is null)
                return;

            if (ParameterSelectedItems is null || !ParameterSelectedItems.Any())
                return;

            ClearTagEditor();

            App.Current.Dispatcher.Invoke(() =>
                TagEditorGridTable.SetColumns([.. ParameterSelectedItems.Select(p => p.Name)]));

            var tagNames = plcInfo.GetTagNamesFor(ScopeSelectedItem.Name, UdtSelectedItem.Name);

            foreach (var tagName in tagNames)
            {
                if (Cancel())
                    return;

                var tagValues = new List<string>();

                foreach (var parameterSelectedItem in ParameterSelectedItems)
                {
                    var tag = plcInfo.GetTag(
                        logger, ScopeSelectedItem.Name, tagName, UdtSelectedItem.Name, parameterSelectedItem.Name);

                    //if (tag is TagReadonly)
                    //    continue;

                    tagValues.Add(tag?.Value ?? "[ERROR]");
                }

                App.Current.Dispatcher.Invoke(() =>
                    TagEditorGridTable.AddRow(tagName, [.. tagValues]));
            }

            if (filterText is not null)
            {
                FilterText = filterText;

                App.Current.Dispatcher.Invoke(() =>
                    TagEditorGridTable.Filter(FilterText));
            }
        }

        private void ClearTagEditor()
        {
            FilterText = string.Empty;

            App.Current.Dispatcher.Invoke(() =>
                TagEditorGridTable.Clear());
        }

        private void WriteTagsBackground()
        {
            if (plcInfo is null)
                return;

            if (ScopeSelectedItem is null)
                return;

            if (UdtSelectedItem is null)
                return;

            foreach (string columnName in TagEditorGridTable.WriteableColumnNames)
            {
                foreach (DataRowView row in TagEditorGridTable.VisibleRows)
                {
                    var tagName = row[Constants.TagName].ToString();

                    if (string.IsNullOrWhiteSpace(tagName))
                        continue;

                    var writeValue = row[columnName].ToString();

                    if (string.IsNullOrWhiteSpace(writeValue))
                        continue;

                    plcInfo.GetTag(logger, ScopeSelectedItem.Name, tagName, UdtSelectedItem.Name, TagEditorGridTable.GetRawColumnName(columnName))?
                        .Write(logger, writeValue);
                }
            }
        }

        public void ExportTags()
        {
            if (IsRunning)
                return;

            IsRunning = true;

            var sfd = TagEditorExporter.GetSaveFileDialog();

            if (!sfd.ShowDialog() ?? false)
                return;

            try
            {
                TagEditorExporter.Geaux(TagEditorGridTable, sfd.FileName);
            }
            catch (Exception ex)
            {
                logger.Log($"Error: {ex.Message}");
            }

            logger.Log($"Exported to: {sfd.FileName}");

            IsRunning = false;
        }
    }
}
