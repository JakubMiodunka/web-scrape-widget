using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WebScrapeWidget.CustomControls;
using WebScrapeWidget.DataGathering.Repositories;


namespace WebScrapeWidget
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {      
        public MainWindow()
        {
            var inteface = Interface.FromFile(AppConfig.Instance.InterfaceDefinitionPath);

            InitializeComponent();
            Content.Content = inteface;
        }
    }
}