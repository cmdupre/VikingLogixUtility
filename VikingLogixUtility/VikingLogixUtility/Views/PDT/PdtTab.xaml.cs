using System.Windows.Controls;

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
    }
}
