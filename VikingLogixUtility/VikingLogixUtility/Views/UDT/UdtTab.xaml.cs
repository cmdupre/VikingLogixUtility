using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using VikingLogixUtility.ViewModels;

namespace VikingLogixUtility.Views.UDT
{
    /// <summary>
    /// Interaction logic for UdtTab.xaml
    /// </summary>
    public partial class UdtTab : UserControl
    {
        private readonly UdtTabViewModel viewModel = new();

        public UdtTab()
        {
            InitializeComponent();

            DataContext = viewModel;
        }

        private void Output_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is not TextBox textbox)
                return;

            e.Handled = true;

            textbox.ScrollToEnd();
        }

        private void UdtTab_Loaded(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).Closing += WindowClosing;
        }

        private void WindowClosing(object? sender, CancelEventArgs e)
        {
            viewModel.Dispose();
        }
    }
}
