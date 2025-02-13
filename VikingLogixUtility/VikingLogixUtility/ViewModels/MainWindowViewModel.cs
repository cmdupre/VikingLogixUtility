using System.ComponentModel;
using VikingLogixUtility.Bases;
using VikingLogixUtility.L5xApp.ViewModels;

namespace VikingLogixUtility.ViewModels
{
    internal sealed class MainWindowViewModel : BaseNotifyPropertyChanged
    {
        public MainWindowViewModel(TagEditorViewModel udtTabVM, TagEditorViewModel pdtTabVM, L5xTagExportViewModel lL5xTagExportVM)
        {
            UdtTabVM = udtTabVM;
            PdtTabVM = pdtTabVM;
            L5xTagExportVM = lL5xTagExportVM;

            UdtTabVM.PropertyChanged += Child_PropertyChanged;
            PdtTabVM.PropertyChanged += Child_PropertyChanged;
            L5xTagExportVM.PropertyChanged += Child_PropertyChanged;
        }

        public TagEditorViewModel UdtTabVM { get; }
        public TagEditorViewModel PdtTabVM { get; }
        public L5xTagExportViewModel L5xTagExportVM { get; }

        public bool IsRunning =>
            !UdtTabVM.IsRunning &&
            !PdtTabVM.IsRunning &&
            !L5xTagExportVM.IsRunning;

        private void Child_PropertyChanged(object? sender, PropertyChangedEventArgs e) =>
            NotifyPropertyChanged(e.PropertyName ?? string.Empty);
    }
}
