using System.Windows;

using WebScrapeWidget.CustomControls;
using WebScrapeWidget.DataGathering.Repositories;


namespace WebScrapeWidget;

//TODO: Doc-string.
public partial class MainWindow : Window
{
    private readonly Interface _interface;
    
    //TODO: Doc-string.
    public MainWindow()
    {
        string interfaceDefinitoinFile = AppConfig.Instance.InterfaceDefinitionPath;
        var dataSourcesRepository = DataSourcesRepository.Instance;

        _interface = Interface.FromFile(interfaceDefinitoinFile, dataSourcesRepository);

        DataSourcesRepository.Instance.RemoveNotSubscribedDataSources();

        InitializeComponent();

        MainUserInterface.Content = _interface;
    }

    //TODO: Doc-string.
    public async void GatherData(object sender, RoutedEventArgs eventData)
    {
        var dataSourcesRepository = DataSourcesRepository.Instance;
        await dataSourcesRepository.GatherDataFromAllSources();
    }
}