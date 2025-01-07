using System.Windows.Controls;
using VikingLogixUtility.Interfaces;

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

            if (DataContext is not IEditorViewModel vm)
                return;

            vm.Rows.Clear();
            vm.TagEditor?.Columns.Clear();
        }

        private void FilterText_TextChanged(object sender, TextChangedEventArgs e)
        {
            e.Handled = true;

            if (DataContext is not IEditorViewModel vm)
                return;

            vm.FilterTags();
        }
    }
}
