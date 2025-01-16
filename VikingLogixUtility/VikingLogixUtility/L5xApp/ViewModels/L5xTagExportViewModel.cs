using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;
using VikingLogixUtility.Bases;
using VikingLogixUtility.L5xApp.Commands;
using VikingLogixUtility.L5xApp.Interfaces;

namespace VikingLogixUtility.L5xApp.ViewModels
{
    internal sealed class L5xTagExportViewModel : BaseNotifyPropertyChanged, ITagExportViewModel
    {
        private const short MaxWarning = 10;

        private bool isRunning = false;
        private bool excludeMemberTags = false;
        private Cursor saveCursor = Mouse.OverrideCursor;
        private string l5xFilename = string.Empty;
        private string output = string.Empty;
        private ObservableCollection<ScopeItemViewModel> scopeListBox = [];
        private ScopeItemViewModel? scopeSelectedItem = null;
        private short ignoreCount = 0;

        public L5xTagExportViewModel()
        {
            ScopeListBox.Add(new ScopeItemViewModel("<Select an L5X File...>"));
        }

        public ICommand ClickL5XFile => new ProcessL5XCommand(this);
        public ICommand ClickExport => new ExportCommand(this);
        public ICommand ClickSelect => new SelectScopesCommand(this);

        public bool IsRunning
        {
            get => isRunning;

            set
            {
                isRunning = value;
                NotifyPropertyChanged();
                CommandManager.InvalidateRequerySuggested();

                if (IsRunning)
                {
                    saveCursor = Mouse.OverrideCursor;
                    Mouse.OverrideCursor = Cursors.Wait;
                    Output = string.Empty;
                    ignoreCount = 0;
                }
                else
                {
                    Mouse.OverrideCursor = saveCursor;
                }
            }
        }

        public string L5XFilename
        {
            get => Path.GetFileName(l5xFilename);

            set
            {
                l5xFilename = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(L5XFilenameToolTip));
            }
        }

        public string L5XFilenameToolTip => l5xFilename;

        public ObservableCollection<ScopeItemViewModel> ScopeListBox
        {
            get => scopeListBox;

            set
            {
                scopeListBox = value;
                NotifyPropertyChanged();
            }
        }

        public ScopeItemViewModel? ScopeSelectedItem
        {
            get => scopeSelectedItem;

            set
            {
                scopeSelectedItem = value;
                NotifyPropertyChanged();
            }
        }

        public bool ExcludeMemberTags
        {
            get => excludeMemberTags;

            set
            {
                excludeMemberTags = value;
                NotifyPropertyChanged();
            }
        }

        public string Output
        {
            get => output;

            private set
            {
                output = value;
                NotifyPropertyChanged();
            }
        }

        public void Log(string message)
        {
            if (message.Contains("unable to parse address for tag") ||
                message.Contains("multiple mapping rungs"))
            {
                ++ignoreCount;

                if (ignoreCount > MaxWarning)
                    return;

                if (ignoreCount == MaxWarning)
                    message += "\n[Ignoring further address warnings...]";
            }

            output += $"{message}\n";
            NotifyPropertyChanged(nameof(Output));
        }
    }
}
