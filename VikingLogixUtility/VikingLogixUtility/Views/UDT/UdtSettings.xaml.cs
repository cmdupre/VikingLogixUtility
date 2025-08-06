using System.Windows.Controls;
using VikingLogixUtility.ViewModels;

namespace VikingLogixUtility.Views.UDT
{
    /// <summary>
    /// Interaction logic for UdtSettings.xaml
    /// </summary>
    public partial class UdtSettings : UserControl
    {
        public UdtSettings()
        {
            InitializeComponent();
        }

        private void Address_TextChanged(object sender, TextChangedEventArgs e)
        {
            e.Handled = true;

            if (DataContext is not TagEditorViewModel vm)
                return;

            vm.ScopeItems.Clear();
        }

        private void Slot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            e.Handled = true;

            if (DataContext is not TagEditorViewModel vm)
                return;

            vm.ScopeItems.Clear();
        }
    }
}
