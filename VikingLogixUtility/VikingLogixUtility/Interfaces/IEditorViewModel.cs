using System.Collections.ObjectModel;
using System.Windows.Controls;
using VikingLogixUtility.ViewModels;

namespace VikingLogixUtility.Interfaces
{
    internal interface IEditorViewModel
    {
        public bool IsRunning { get; }
        public string Address { get; }
        public ObservableCollection<DisplayStringViewModel> ScopeItems { get; }
        public DisplayStringViewModel? ScopeSelectedItem { get; }
        public void LoadScopes();
        public void LoadTagEditor();
        public DataGrid? TagEditor { get; set; }
        public void WriteTags();
        public ObservableCollection<RowViewModel> Rows { get; set; }
        public void FilterTags();
    }
}
