using System.Windows.Input;
using VikingLogixUtility.ViewModels;

namespace VikingLogixUtility.Interfaces
{
    internal interface IUdtTabViewModel : IEditorViewModel
    {
        public IEnumerable<DisplayStringViewModel>? ParameterSelectedItems { get; set; }
        public ICommand ScopeItemsSelectionChanged { get; }
        public ICommand UdtItemsSelectionChanged { get; }
        public void LoadUdts();
        public void LoadParameters();
    }
}
