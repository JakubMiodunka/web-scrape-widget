// Ignore Spelling: Timestamp

using System.Data;
using System.IO;

using WebScrapeWidget.DataGathering.DataSources;
using WebScrapeWidget.DataGathering.Interfaces;
using WebScrapeWidget.Utilities;


namespace WebScrapeWidget.DataGathering.Repositories;

/// <summary>
/// Repository, that provided access to a pool of data sources.
/// </summary>
/// <remarks>
/// Keep in mind, that initially data from all sources contained by the repository is not gathered
/// and attempt to accedes it will result in raised exception.
/// To gather data from sources use DataSourcesRepository.GatherDataFromAllSources method.
/// </remarks>
public sealed class DataSourcesRepository
{
    #region Singleton
    public static DataSourcesRepository Instance
    {
        get
        {
            if (s_instance is null)
            {
                string directoryPath = AppConfig.Instance.DataSourcesStorage;
                bool recursiveSearch = AppConfig.Instance.DataSourcesStorageRecursiveSearch;

                s_instance = new DataSourcesRepository(directoryPath, recursiveSearch);
            }

            return s_instance;
        }
    }

    private static DataSourcesRepository? s_instance;
    #endregion

    #region Properties
    public readonly string DirectoryPath;
    
    private readonly List<IDataSource> _dataSources;
    #endregion

    #region Class instantiation
    /// <summary>
    /// Creates a new data source basing on provided *.xml file. 
    /// </summary>
    /// <param name="filePath">
    /// Path to *.xml file containing data source definition.
    /// </param>
    /// <returns>
    /// Data source created basing on provided *.xml file.
    /// </returns>
    /// <exception cref="NotSupportedException">
    /// Thrown, when data source definition contained by provided *.xml file is not supported.
    /// </exception>
    private static IDataSource CreateDataSourceFromFile(string filePath)
    {
        FileSystemUtilities.ValidateFile(filePath, ".xml");

        if (WebsiteElement.IsWebsiteElementDefinition(filePath))
        {
            return WebsiteElement.FromFile(filePath);
        }

        string errorMessage = 
            $"Data source creation basing on provided file is not supported: {filePath}";
        throw new NotSupportedException(errorMessage);
    }

    /// <summary>
    /// Creates a new repository of data sources.
    /// </summary>
    /// <param name="directoryPath">
    /// Path to directory, where *.xml files containing data sources definitions
    /// are stored.
    /// </param>
    /// /// <param name="recursiveSearch">
    /// Flag, which specifies if provided directory shall be searched recursively.
    /// </param>
    private DataSourcesRepository(string directoryPath, bool recursiveSearch)
    {
        FileSystemUtilities.ValidateDirectory(directoryPath);

        DirectoryPath = directoryPath;
        _dataSources = [.. CreateSpecialDataSources(), .. CreateDataSources(recursiveSearch)];
    }

    /// <summary>
    /// Creates special data sources.
    /// </summary>
    /// <returns></returns>
    private IDataSource[] CreateSpecialDataSources()
    {
        var processorUsage = new ProcessorUsage("processor-usage", TimeSpan.FromSeconds(1));
        var ramUsage = new RamUsage("ram-usage", TimeSpan.FromSeconds(3));

        return [processorUsage, ramUsage];
    }

    /// <summary>
    /// Creates data sources from *.xml files contained by directory,
    /// to which repository is referring to.
    /// </summary>
    /// <param name="recursiveSearch">
    /// Flag, which specifies if provided directory shall be searched recursively.
    /// </param>
    /// <returns>
    /// Data sources created basing on *.xml files contained by directory,
    /// to which repository is referring to.
    /// </returns>
    private IDataSource[] CreateDataSources(bool recursiveSearch)
    {
        var searchOption  = (recursiveSearch) ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

        return Directory.EnumerateFiles(DirectoryPath, "*.xml", searchOption)
            .Select(CreateDataSourceFromFile)
            .ToArray();
    }
    #endregion

    #region Data gathering
    /// <summary>
    /// Starts the process of infinite, periodic data gathering
    /// from all sources cantoned by repository.
    /// </summary>
    /// <remarks>
    /// Refresh rate of every data source is individual.
    /// Maximal possible refresh rate for 
    /// a data source is 1 second - refresh rates not divisible by 1 second will be rounded.
    /// </remarks>
    /// <returns>
    /// Task corresponding to status periodic data gathering process,
    /// which never be completed as process is infinite.
    /// </returns>
    public async Task StartPeriodicDataGathering()
    {
        while (true)
        {
            await Task.Delay(1000);

            DateTime currentTimestamp = DateTime.Now;

            _dataSources
                .Where(source => (currentTimestamp - source.LastRefreshTimestamp) >= source.RefreshRate)
                .ToList()
                .ForEach(dataSource => dataSource.GatherData());
        }
    }

    /// <summary>
    /// Gathers data from all sources contained by repository.
    /// </summary>
    /// <returns>
    /// Task, which will be completed, when data will be gathered
    /// from all data sources contained by the repository.
    /// </returns>
    public async Task GatherDataFromAllSources()
    {
        Task[] dataGatheringtasks = _dataSources
            .Select(dataSource => dataSource.GatherData())
            .ToArray();

        await Task.WhenAll(dataGatheringtasks);
    }
    #endregion

    #region Interactions
    /// <summary>
    /// Searches through repository for data source with specified name.
    /// </summary>
    /// <param name="dataSourceName">
    /// Name of data source, which shall be returned.
    /// </param>
    /// <returns>
    /// Data source contained by the repository, with specified name.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown, when at least one reference-type argument is a null reference.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown, when repository does not contain data source with specified name. 
    /// </exception>
    public IDataSource GetDataSource(string dataSourceName)
    {
        if (dataSourceName is null)
        {
            string argumentName = nameof(dataSourceName);
            const string ErrorMessage = "Provided data source name is a null reference:";
            throw new ArgumentNullException(argumentName, ErrorMessage);
        }

        try
        {
            return _dataSources
                .Where(dataSource => dataSource.Name == dataSourceName)
                .First();
        }
        catch (InvalidOperationException)
        {
            string errorMessage = $"Data source with provided name does not exist in repository: {dataSourceName}";
            throw new ArgumentOutOfRangeException(errorMessage);
        }
    }
    
    /// <summary>
    /// Removes not subscribed data sources from repository.
    /// </summary>
    public void RemoveNotSubscribedDataSources()
    {
        _dataSources
            .Where(source => !source.IsSubscribed)
            .ToList()
            .ForEach(source => _dataSources.Remove(source));
    }
    #endregion
}
