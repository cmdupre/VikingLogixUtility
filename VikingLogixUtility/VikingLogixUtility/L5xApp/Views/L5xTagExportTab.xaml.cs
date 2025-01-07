using System.Windows.Controls;

namespace VikingLogixUtility.L5xApp.Views
{
    /// <summary>
    /// Interaction logic for L5xTagExportTab.xaml
    /// </summary>
    public partial class L5xTagExportTab : UserControl
    {
        public L5xTagExportTab()
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
