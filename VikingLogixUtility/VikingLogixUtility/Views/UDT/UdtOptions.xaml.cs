using System.Windows;
using System.Windows.Controls;
using VikingLogixUtility.ViewModels;

namespace VikingLogixUtility.Views.UDT
{
    /// <summary>
    /// Interaction logic for UdtOptions.xaml
    /// </summary>
    public partial class UdtOptions : UserControl
    {
        public UdtOptions()
        {
            InitializeComponent();
        }

        private void ScopeItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            e.Handled = true;

            if (DataContext is not TagEditorViewModel vm)
                return;

            if (vm.ScopeItemsSelectionChanged.CanExecute(null))
                vm.ScopeItemsSelectionChanged.Execute(null);
        }

        private void UdtItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            e.Handled = true;

            if (DataContext is not TagEditorViewModel vm)
                return;

            if (vm.UdtItemsSelectionChanged.CanExecute(null))
                vm.UdtItemsSelectionChanged.Execute(null);
        }

        private void ParameterItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            e.Handled = true;

            if (DataContext is not TagEditorViewModel vm)
                return;

            if (sender is not ListBox lb)
                return;

            vm.ParameterSelectedItems = lb.SelectedItems.Cast<DisplayStringViewModel>();
        }

        private void SelectAllButton_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;

            foreach (var item in ParameterItems.Items)
                ParameterItems.SelectedItems.Add(item);
        }

        private void SelectNoneButton_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;

            foreach (var item in ParameterItems.Items)
                ParameterItems.SelectedItems.Remove(item);
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
