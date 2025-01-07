using System.Windows.Controls;

namespace VikingLogixUtility.Views.UDT
{
    /// <summary>
    /// Interaction logic for UdtTab.xaml
    /// </summary>
    public partial class UdtTab : UserControl
    {
        public UdtTab()
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
    }
}
