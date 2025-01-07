using System.Windows;
using System.Windows.Controls;
using VikingLogixUtility.Extensions;
using VikingLogixUtility.Interfaces;

namespace VikingLogixUtility.Views.UDT
{
    /// <summary>
    /// Interaction logic for UdtEditor.xaml
    /// </summary>
    public partial class UdtEditor : UserControl
    {
        public UdtEditor()
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
