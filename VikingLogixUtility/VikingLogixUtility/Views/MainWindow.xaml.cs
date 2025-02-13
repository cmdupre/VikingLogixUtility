using System.ComponentModel;
using System.Reflection;
using System.Windows;
using VikingLogixUtility.L5xApp.ViewModels;
using VikingLogixUtility.Processors;
using VikingLogixUtility.ViewModels;

namespace VikingLogixUtility.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var version = Assembly.GetExecutingAssembly().GetName().Version;

            Title = "Viking Logix Utility :-: Techneaux Industrial Control Systems" +
                "  " +
                $"[v{version?.Major}.{version?.Minor}.{version?.Build}]";

            var vm = new MainWindowViewModel(
                new TagEditorViewModel(ViewUdtTab.ViewUdtEditor.ViewDataGrid, new TagProcessorUdt()),
                new TagEditorViewModel(ViewPdtTab.ViewPdtEditor.ViewDataGrid, new TagProcessorPdt()),
                new L5xTagExportViewModel());

            DataContext = vm;
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (DataContext is not MainWindowViewModel vm)
                return;

            if (!vm.IsRunning)
                e.Cancel = true;
        }
    }
}