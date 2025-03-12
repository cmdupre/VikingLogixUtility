using System.Windows.Controls;
using VikingLogixUtility.ViewModels;

namespace VikingLogixUtility.Views.PDT
{
    /// <summary>
    /// Interaction logic for PdtOptions.xaml
    /// </summary>
    public partial class PdtOptions : UserControl
    {
        public PdtOptions()
        {
            InitializeComponent();
        }

        private void ScopeItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            e.Handled = true;

            if (DataContext is not TagEditorViewModel vm)
                return;

            vm.TagEditorGridTable.Clear();
        }

        private void FilterText_TextChanged(object sender, TextChangedEventArgs e)
        {
            e.Handled = true;

            if (DataContext is not TagEditorViewModel vm)
                return;

            vm.FilterGridTable();
        }
    }
}
