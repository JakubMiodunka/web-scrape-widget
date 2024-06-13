using System.Windows;

using WebScrapeWidget.CustomControls;
using WebScrapeWidget.DataGathering.Repositories;


namespace WebScrapeWidget
{
    //TODO: Doc-string.
    public partial class MainWindow : Window
    {
        private readonly Interface _interface;
        
        //TODO: Doc-string.
        public MainWindow()
        {
            _interface = Interface.FromFile(AppConfig.Instance.InterfaceDefinitionPath);

            InitializeComponent();

            DataScrollViewer.Content = _interface;
            LastUpdateTextBlockUpdate();
        }

        //TODO: Doc-string.
        public async void UpdateData(object sender, RoutedEventArgs eventData)
        {
            var dataSourcesRepository = DataSourcesRepository.Instance;
            await dataSourcesRepository.GatherDataFromAllSources();

            _interface.UpdateContent();
            LastUpdateTextBlockUpdate();
        }

        //TODO: Doc-string.
        private void LastUpdateTextBlockUpdate()
        {
            var dataSourcesRepository = DataSourcesRepository.Instance; ;
            string isoTimestamp = dataSourcesRepository.UpdateTimestamp.ToString("s");

            LastUpdateTextBlock.Text = $"Last update: {isoTimestamp}";
        }
    }
}