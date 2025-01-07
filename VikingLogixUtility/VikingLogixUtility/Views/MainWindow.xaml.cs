using System.Reflection;
using System.Windows;

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
        }
    }
}