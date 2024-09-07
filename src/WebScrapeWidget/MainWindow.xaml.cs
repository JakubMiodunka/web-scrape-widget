using System.Windows;

using WebScrapeWidget.CustomControls;
using WebScrapeWidget.DataGathering.Interfaces;
using WebScrapeWidget.DataGathering.Repositories;


namespace WebScrapeWidget;

//TODO: Doc-string.
public partial class MainWindow : Window
{
    private readonly IDataSourcesRepository _dataSourcesRepository;
    
    //TODO: Doc-string. Re-factor.
    public MainWindow(IDataSourcesRepository dataSourcesRepository)
    {
        string interfaceDefinitoinFile = AppConfig.Instance.InterfaceDefinitionPath;

        var mainInterface = Interface.FromFile(interfaceDefinitoinFile, dataSourcesRepository);

        dataSourcesRepository.RemoveNotSubscribedDataSources();

        InitializeComponent();

        MainUserInterface.Content = mainInterface;
        _dataSourcesRepository = dataSourcesRepository;
    }

    //TODO: Doc-string.
    public async void GatherData(object sender, RoutedEventArgs eventData)
    {
        await _dataSourcesRepository.GatherDataFromAllSources();
    }
}