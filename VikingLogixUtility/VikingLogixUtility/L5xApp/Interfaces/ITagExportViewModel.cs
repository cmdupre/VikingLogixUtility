using System.Collections.ObjectModel;
using VikingLogixUtility.Interfaces;
using VikingLogixUtility.L5xApp.ViewModels;

namespace VikingLogixUtility.L5xApp.Interfaces
{
    internal interface ITagExportViewModel : ILoggable
    {
        public bool IsRunning { get; set; }
        public bool ExcludeMemberTags { get; set; }
        public string L5XFilename { get; set; }
        public string L5XFilenameToolTip { get; }
        public ObservableCollection<ScopeItemViewModel> ScopeListBox { get; set; }
    }
}
