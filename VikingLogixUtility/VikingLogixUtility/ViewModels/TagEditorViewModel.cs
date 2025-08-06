using System.Collections.ObjectModel;
using System.Data;
using System.Windows;
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
    internal sealed class TagEditorViewModel : BaseNotifyPropertyChanged,
        IDisposable,
        ITagEditorLoadable,
        ICancelable,
        IExportable,
        IWriteable,
        IUdtsLoadable,
        IParametersLoadable
    {
        private readonly ITagEditable tagProcessor;
        private readonly TagEditorGridTable tagEditorGridTable;
        private readonly Logger logger = new();

        private bool isRunning = false;
        private bool cancelRequested = false;
        private int progressBarValue = 0;
        private int progressBarMaximum = 100;
        private int slot = 0;
        private string address = string.Empty;
        private string filterText = string.Empty;
        private ObservableCollection<DisplayStringViewModel> scopeItems = [];
        private ObservableCollection<DisplayStringViewModel> udtItems = [];
        private ObservableCollection<DisplayStringViewModel> parameterItems = [];
        private ObservableCollection<int> slotOptions = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16];
        private DisplayStringViewModel? scopeSelectedItem = null;
        private DisplayStringViewModel? udtSelectedItem = null;
        private PlcInfo? plcInfo = null;
        private Cursor saveCursor = Mouse.OverrideCursor;
        private Visibility progressBarVisibility = Visibility.Hidden;
        private IEnumerable<DisplayStringViewModel>? parameterSelectedItems = null;

        public ICommand ReadClicked => new ReadCommand(this);
        public ICommand WriteClicked => new WriteCommand(this);
        public ICommand CancelClicked => new CancelCommand(this);
        public ICommand ExportClicked => new TagEditorExportCommand(this);
        public ICommand UdtItemsSelectionChanged => new LoadParametersCommand(this);
        public ICommand ScopeItemsSelectionChanged => new LoadUdtsCommand(this);

        public void Dispose() => PlcInfo?.Dispose();

        public TagEditorViewModel(DataGrid viewDataGrid, ITagEditable tagProcessor)
        {
            var settings = Settings.Load();

            Address = settings.Address;

            tagEditorGridTable = new(viewDataGrid);

            logger.TextChanged +=
                (sender, e) => { NotifyPropertyChanged(nameof(Output)); };

            this.tagProcessor = tagProcessor;
        }

        public PlcInfo? PlcInfo => plcInfo;

        public TagEditorGridTable TagEditorGridTable => tagEditorGridTable;

        public ILoggable Logger => logger;

        public string Output => logger.Text;

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

        public ObservableCollection<int> SlotOptions
        {
            get => slotOptions;

            set
            {
                slotOptions = value;
                NotifyPropertyChanged();
            }
        }

        public string FilterText
        {
            get => filterText.SanitizeFilterText();

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

        public string Address
        {
            get => address;

            set
            {
                address = value;
                NotifyPropertyChanged();
            }
        }

        public int Slot
        {
            get => slot;

            set
            {
                slot = value;
                NotifyPropertyChanged();
            }
        }

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
                    ProgressBarValue = 0;
                }
                else
                {
                    Mouse.OverrideCursor = saveCursor;
                    ProgressBarVisibility = Visibility.Hidden;
                }
            }
        }

        public bool IsNotRunning => !IsRunning;

        public int ProgressBarValue
        {
            get => progressBarValue;

            set
            {
                progressBarValue = value;
                NotifyPropertyChanged();
            }
        }

        public Visibility ProgressBarVisibility
        {
            get => progressBarVisibility;

            set
            {
                progressBarVisibility = value;
                NotifyPropertyChanged();
            }
        }

        public int ProgressBarMaximum
        {
            get => progressBarMaximum;

            set
            {
                progressBarMaximum = value;
                NotifyPropertyChanged();
            }
        }

        public async void LoadScopes()
        {
            if (IsRunning)
                return;

            CancelRequested = false;
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
                    logger.Log("Writing tags...");
                    WriteTagsBackground();
                    logger.Log("Loading tag editor...");
                    LoadTagEditorBackground();
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

            if (PlcInfo is null ||
                ScopeSelectedItem is null)
                return;

            foreach (var udt in PlcInfo.GetTemplateNamesFor(ScopeSelectedItem.Name))
                UdtItems.Add(new DisplayStringViewModel(udt));

            UdtItems = [.. UdtItems.OrderBy(x => x)];
        }

        public void LoadParameters()
        {
            ClearTagEditor();
            ParameterItems.Clear();

            if (PlcInfo is null ||
                ScopeSelectedItem is null ||
                UdtSelectedItem is null)
                return;

            foreach (var parameter in PlcInfo.GetFieldNamesFor(ScopeSelectedItem.Name, UdtSelectedItem.Name))
                ParameterItems.Add(new DisplayStringViewModel(parameter));

            ParameterItems = [.. ParameterItems.OrderBy(x => x)];
        }

        public void FilterGridTable() =>
            TagEditorGridTable.Filter(FilterText);

        private void LoadScopesBackground()
        {
            using var listing = TagListing.Create(new TagPath(Address, Slot), logger);

            if (listing is null)
                return;

            if (Cancel())
                return;

            plcInfo = PlcInfo.Build(listing, logger, Cancel);

            if (PlcInfo is null)
                return;

            App.Current.Dispatcher.Invoke(() =>
                ScopeItems.Add(new DisplayStringViewModel(Constants.Controller)));

            foreach (var programName in PlcInfo.ProgramNames)
                App.Current.Dispatcher.Invoke(() =>
                    ScopeItems.Add(new DisplayStringViewModel(programName)));
        }

        public bool Cancel()
        {
            if (CancelRequested)
            {
                logger.Log("Cancel requested...");

                CancelRequested = false;

                return true;
            }

            return false;
        }

        public void ClearTagEditor() =>
            App.Current.Dispatcher.Invoke(TagEditorGridTable.Clear);

        public void ExportTags()
        {
            if (IsRunning)
                return;

            IsRunning = true;

            var sfd = TagEditorExporter.GetSaveFileDialog();

            if (!sfd.ShowDialog() ?? false)
            {
                IsRunning = false;
                return;
            }

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

        private void LoadTagEditorBackground()
        {
            CancelRequested = false;
            tagProcessor.Read(this);
        }

        private void WriteTagsBackground()
        {
            ProgressBarMaximum =
                TagEditorGridTable.WriteableColumnNames.Count() *
                TagEditorGridTable.VisibleRows.Count();

            ProgressBarVisibility = Visibility.Visible;

            foreach (string columnName in TagEditorGridTable.WriteableColumnNames)
            {
                foreach (DataRowView row in TagEditorGridTable.VisibleRows)
                {
                    ++ProgressBarValue;

                    var tagName = row[Constants.TagName].ToString();

                    if (string.IsNullOrWhiteSpace(tagName))
                        continue;

                    var writeValue = row[columnName].ToString();

                    if (string.IsNullOrWhiteSpace(writeValue))
                        continue;

                    tagProcessor.Write(this, tagName, columnName, writeValue);
                }
            }
        }
    }
}
