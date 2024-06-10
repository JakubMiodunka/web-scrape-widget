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


namespace WebScrapeWidget
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {      
        public MainWindow()
        {
            const string filePath = @"C:\Users\Jakub Miodunka\Desktop\WebScrapeWidget\interface.xml";
            var inteface = Interface.FromFile(filePath);

            

            InitializeComponent();
            scroll.Content = inteface;
        }
    }
}