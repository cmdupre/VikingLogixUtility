using System.Windows.Controls;
using VikingLogixUtility.ViewModels;

namespace VikingLogixUtility.Views.PDT
{
    /// <summary>
    /// Interaction logic for PdtSettings.xaml
    /// </summary>
    public partial class PdtSettings : UserControl
    {
        public PdtSettings()
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
    }
}
