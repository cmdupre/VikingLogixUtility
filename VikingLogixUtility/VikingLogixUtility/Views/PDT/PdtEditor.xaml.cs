using System.Windows.Controls;
using System.Windows.Input;
using VikingLogixUtility.ViewModels;

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

        private void ViewDataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (DataContext is not PdtTabViewModel vm)
                return;

            if (e.Key == Key.V && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                vm.TagEditorGridTable.PasteFromClipboard(vm.Logger);
                e.Handled = true;
            }

            if (e.Key == Key.C && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                vm.TagEditorGridTable.CopyToClipboard();
                e.Handled = true;
            }
        }
    }
}
