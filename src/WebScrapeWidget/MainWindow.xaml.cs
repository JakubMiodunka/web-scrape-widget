using System.Windows;

using WebScrapeWidget.CustomControls;


namespace WebScrapeWidget
{
    //TODO: Doc-string.
    public partial class MainWindow : Window
    {
        //TODO: Doc-string.
        public MainWindow()
        {
            var inteface = Interface.FromFile(AppConfig.Instance.InterfaceDefinitionPath);

            InitializeComponent();
            MainContent.Content = inteface;
        }
    }
}