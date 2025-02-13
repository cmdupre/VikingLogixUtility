using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using VikingLogixUtility.ViewModels;

namespace VikingLogixUtility.Views.PDT
{
    /// <summary>
    /// Interaction logic for PdtTab.xaml
    /// </summary>
    public partial class PdtTab : UserControl
    {
        public PdtTab()
        {
            InitializeComponent();
        }

        private void Output_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is not TextBox textbox)
                return;

            e.Handled = true;

            textbox.ScrollToEnd();
        }

        private void PdtTab_Loaded(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).Closing += WindowClosing;
        }

        private void WindowClosing(object? sender, CancelEventArgs e)
        {
            if (DataContext is not TagEditorViewModel vm)
                return;

            vm.Dispose();
        }
    }
}
