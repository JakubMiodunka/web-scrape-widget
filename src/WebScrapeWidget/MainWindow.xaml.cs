using System.Windows;
using System.Xml.Linq;
using WebScrapeWidget.CustomControls;
using WebScrapeWidget.DataGathering.Interfaces;
using WebScrapeWidget.DataGathering.Repositories;
using WebScrapeWidget.Utilities;


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
        FileSystemUtilities.ValidateFile(interfaceDefinitionFile, ".xml");

        var interfaceDefinition = XDocument.Load(interfaceDefinitionFile);

        var mainInterface = Interface.FromXmlDocument(interfaceDefinition, dataSourcesRepository);

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