using System.Collections.ObjectModel;
using VikingLogixUtility.ViewModels;

namespace VikingLogixUtility.Interfaces
{
    internal interface ITagEditorLoadable
    {
        bool IsRunning { get; }
        string Address { get; }
        ObservableCollection<DisplayStringViewModel> ScopeItems { get; }
        void LoadScopes();
        void LoadTagEditor();
    }
}
