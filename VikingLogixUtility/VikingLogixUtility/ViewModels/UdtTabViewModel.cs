using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Input;
using VikingLibPlcTagNet.Data;
using VikingLibPlcTagNet.Settings;
using VikingLogixUtility.Bases;
using VikingLogixUtility.Commands;
using VikingLogixUtility.Common;
using VikingLogixUtility.Extensions;
using VikingLogixUtility.Interfaces;
using VikingLogixUtility.Models;

namespace VikingLogixUtility.ViewModels
{
    internal sealed class UdtTabViewModel : BaseNotifyPropertyChanged, ILoggable, IUdtTabViewModel, IDisposable
    {
        private PlcInfo? plcInfo = null;
        private bool isRunning = false;
        private Cursor saveCursor = Mouse.OverrideCursor;
        private string output = string.Empty;
        private string address = string.Empty;
        private ObservableCollection<DisplayStringViewModel> scopeItems = [];
        private ObservableCollection<DisplayStringViewModel> udtItems = [];
        private ObservableCollection<DisplayStringViewModel> parameterItems = [];
        private DisplayStringViewModel? scopeSelectedItem = null;
        private DisplayStringViewModel? udtSelectedItem = null;
        private IEnumerable<DisplayStringViewModel>? parameterSelectedItems = null;
        private DataGrid? tagEditor = null;
        private ObservableCollection<RowViewModel> rows = [];
        private ObservableCollection<RowViewModel> savedRows = [];
        private string filterText = string.Empty;
        private bool cancelRequested = false;

        public void Dispose() => plcInfo?.Dispose();

        public UdtTabViewModel()
        {
            var settings = Settings.Load();

            Address = settings.Address;
        }

        public ICommand ScopeItemsSelectionChanged => new LoadUdtsCommand(this);

        public ICommand UdtItemsSelectionChanged => new LoadParametersCommand(this);

        public ICommand ReadClicked => new ReadCommand(this);

        public ICommand WriteClicked => new WriteCommand(this);

        public ICommand CancelClicked => new CancelCommand(this);

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
                    Output = string.Empty;
                }
                else
                {
                    Mouse.OverrideCursor = saveCursor;
                }
            }
        }

        public bool IsNotRunning => !IsRunning;

        public string Output
        {
            get => output;

            private set
            {
                output = value;
                NotifyPropertyChanged();
            }
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

        public void Log(string message)
        {
            output += $"{message}\n";
            NotifyPropertyChanged(nameof(Output));
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

                AddParameterSelectionToHeader();
            }
        }

        public DataGrid? TagEditor
        {
            get => tagEditor;
            set => tagEditor = value;
        }

        public ObservableCollection<RowViewModel> Rows
        {
            get => rows;

            set
            {
                rows = value;
                NotifyPropertyChanged();
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

            Log("Loading scopes...");

            await Task.Run(() =>
            {
                try
                {
                    LoadScopesBackground();
                }
                catch (Exception ex)
                {
                    Log($"Error: {ex.Message}");
                }
            });

            Log("Done.");

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

            Log("Loading tag editor...");

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

                    Log($"{message}");
                }
            });

            Log("Done.");

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
                    Log($"Error: {ex.Message}");
                }
            });

            IsRunning = false;
        }

        public void FilterTags() => 
            Rows = GetFilteredRows();

        private ObservableCollection<RowViewModel> GetFilteredRows()
        {
            var filteredRows = new ObservableCollection<RowViewModel>();

            foreach (var savedRow in savedRows)
                if (savedRow.Properties.First().ReadValue.Contains(FilterText, StringComparison.InvariantCultureIgnoreCase))
                    filteredRows.Add(savedRow);

            return filteredRows;
        }

        private void LoadScopesBackground()
        {
            using var listing = TagListing.Create(new TagPath(Address));

            if (listing is null)
                return;

            if (Cancel())
                return;

            plcInfo = PlcInfo.Build(listing, Cancel);

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
                Log("Cancel requested...");

                CancelRequested = false;

                return true;
            }

            return false;
        }

        private void LoadTagEditorBackground(string? filterText = null)
        {
            if (TagEditor is null)
                return;

            if (plcInfo is null)
                return;

            if (ScopeSelectedItem is null)
                return;

            if (UdtSelectedItem is null)
                return;

            if (ParameterSelectedItems is null || !ParameterSelectedItems.Any())
                return;

            ClearTagEditor();

            // Temporary rows to delay adding collection to UI.
            var rows = new ObservableCollection<RowViewModel>();

            var tagNames = plcInfo.GetTagNamesFor(ScopeSelectedItem.Name, UdtSelectedItem.Name);

            foreach (var tagName in tagNames)
            {
                if (Cancel())
                {
                    AddParameterSelectionToHeader();
                    return;
                }

                var cells = new List<CellViewModel>()
                {
                    new(Constants.TagName, tagName, string.Empty, true)
                };

                foreach (var parameterSelectedItem in ParameterSelectedItems)
                {
                    var tag = plcInfo.GetTag(
                        ScopeSelectedItem.Name, tagName, UdtSelectedItem.Name, parameterSelectedItem.Name);

                    if (tag is null)
                        continue;

                    cells.Add(new(parameterSelectedItem.Name, tag.Value!, string.Empty));
                }

                if (cells.Count < 2)
                {
                    Log($"Unable to read tag {tagName}.");
                    continue;
                }

                rows.Add(new RowViewModel(cells));
            }

            Helper.AddTagEditorHeaders(TagEditor, rows);

            savedRows = rows.Sort(0, true);

            if (filterText is not null)
                FilterText = filterText;

            // Finally update UI.
            App.Current.Dispatcher.Invoke(() => Rows = GetFilteredRows());
        }

        private void ClearTagEditor()
        {
            if (TagEditor is null)
                return;

            App.Current.Dispatcher.Invoke(() =>
            {
                FilterText = string.Empty;
                Rows.Clear();
                TagEditor.Columns.Clear();
            });
        }

        private void WriteTagsBackground()
        {
            if (plcInfo is null)
                return;

            if (ScopeSelectedItem is null)
                return;

            if (UdtSelectedItem is null)
                return;

            foreach (var row in Rows)
            {
                if (row.Properties.First().ReadValue is not string tagName)
                    continue;

                foreach (var property in row.Properties.Skip(1))
                {
                    if (property.WriteValue is not string writeValueString)
                        continue;

                    if (string.IsNullOrWhiteSpace(writeValueString))
                        continue;

                    plcInfo.GetTag(ScopeSelectedItem.Name, tagName, UdtSelectedItem.Name, property.Name)?
                        .Write(writeValueString);
                }
            }
        }

        private void AddParameterSelectionToHeader()
        {
            if (ParameterSelectedItems is null)
                return;

            var cells = new List<CellViewModel>();

            foreach (var item in ParameterSelectedItems)
                cells.Add(new CellViewModel(item.DisplayName, string.Empty, string.Empty, true));

            var rows = new ObservableCollection<RowViewModel>
                {
                    new(cells),
                };

            Helper.AddTagEditorHeaders(TagEditor, rows);
        }
    }
}
