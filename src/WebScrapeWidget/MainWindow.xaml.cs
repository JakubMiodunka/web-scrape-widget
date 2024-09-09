using System.Windows;

using WebScrapeWidget.CustomControls;
using WebScrapeWidget.DataGathering.Interfaces;
using WebScrapeWidget.DataGathering.Repositories;


namespace WebScrapeWidget;

//TODO: Doc-string.
public partial class MainWindow : Window
{
    #region Properties
    private readonly IDataSourcesRepository _dataSourcesRepository;
    #endregion

    //TODO: Doc-string. Re-factor.
    public MainWindow(string interfaceDefinitionFile, IDataSourcesRepository dataSourcesRepository)
    {
        var mainInterface = Interface.FromFile(interfaceDefinitionFile, dataSourcesRepository);

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