using System.Windows;
using System.Windows.Controls;
using VikingLogixUtility.Extensions;
using VikingLogixUtility.Interfaces;

namespace VikingLogixUtility.Views.PDT
{
    /// <summary>
    /// Interaction logic for PdtEditor.xaml
    /// </summary>
    public partial class PdtEditor : UserControl
    {
        public PdtEditor()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            e.Handled = true;

            if (DataContext is not IEditorViewModel vm)
                return;

            vm.TagEditor = TagEditor;
        }

        private void TagEditor_Sorting(object sender, DataGridSortingEventArgs e)
        {
            e.Handled = true;

            if (DataContext is not IEditorViewModel vm)
                return;

            vm.Rows = vm.Rows.Sort(e.Column.DisplayIndex);
        }
    }
}
