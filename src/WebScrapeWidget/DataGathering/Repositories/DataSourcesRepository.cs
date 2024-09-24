// Ignore Spelling: Timestamp

using System.Data;
using System.IO;
using System.Xml.Linq;
using WebScrapeWidget.DataGathering.DataSources;
using WebScrapeWidget.DataGathering.Interfaces;
using WebScrapeWidget.Utilities;


namespace WebScrapeWidget.DataGathering.Repositories;

/// <summary>
/// Repository, that provided access to a pool of data sources.
/// </summary>
/// <remarks>
/// Keep in mind, that initially data from all sources contained by the repository is not gathered
/// and attempt to access it will result in raised exception.
/// </remarks>
public sealed class DataSourcesRepository : IDataSourcesRepository
{
    #region Properties
    private readonly List<IDataSource> _dataSources;
    #endregion

    #region Auxiliary methods
    /// <summary>
    /// Creates a new data source basing on specified file. 
    /// </summary>
    /// <param name="filePath">
    /// Path to file containing data source definition.
    /// </param>
    /// <returns>
    /// Data source created basing on specified file.
    /// </returns>
    /// <exception cref="NotSupportedException">
    /// Thrown, when data source definition contained by specified file is not supported.
    /// </exception>
    private static IDataSource CreateDataSourceFromFile(string filePath)
    {
        FileSystemUtilities.ValidateFile(filePath);

        string fileExtension = Path.GetExtension(filePath);

        if (fileExtension == ".xml")
        {
            var xmlDocument = XDocument.Load(filePath);
            
            if (WebsiteElement.IsWebsiteElementDefinition(xmlDocument))
            {
                return WebsiteElement.FromXmlDocument(xmlDocument);
            }
        }
        
        string errorMessage = $"Data source creation basing on provided file is not supported: {filePath}";
        throw new NotSupportedException(errorMessage);
    }

    /// <summary>
    /// Creates special data sources.
    /// </summary>
    /// <returns>
    /// Collection containing new instances of special data sources.
    /// </returns>
    private static IDataSource[] CreateSpecialDataSources()
    {
        var processorUsage = new ProcessorUsage("processor-usage", TimeSpan.FromSeconds(2));
        var ramUsage = new RamUsage("ram-usage", TimeSpan.FromSeconds(3));

        return [processorUsage, ramUsage];
    }
    #endregion

    #region Class instantiation
    /// <summary>
    /// Creates data sources repository,
    /// which contains data sources created basing on files contained by specified directory.
    /// </summary>
    /// <param name="directoryPath">
    /// Path to directory, where files containing data sources definitions are stored.
    /// </param>
    /// <param name="searchRecursively">
    /// Flag, which specifies if provided directory shall be searched recursively.
    /// </param>
    /// <returns>
    /// Data sources repository,
    /// which contains data sources created basing on files contained by specified directory.
    /// </returns>
    public static DataSourcesRepository FromDirectory(string directoryPath, bool searchRecursively)
    {
        FileSystemUtilities.ValidateDirectory(directoryPath);

        var searchOption = (searchRecursively) ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

        IDataSource[] dataSources = Directory.EnumerateFiles(directoryPath, "*", searchOption)
            .Select(CreateDataSourceFromFile)
            .ToArray();

        return new DataSourcesRepository(dataSources);
    }

    /// <summary>
    /// Creates a new repository of data sources.
    /// </summary>
    /// <param name="dataSources">
    /// Collection of data sources, which shall be contained by created repository.
    /// Shall not contain data sources considered as special ones - those
    /// are added automatically to repository content.
    /// </param>
    private DataSourcesRepository(IEnumerable<IDataSource> dataSources)
    {
        _dataSources = new List<IDataSource>();

        IDataSource[] specialDataSources = CreateSpecialDataSources();
        AddDataSources(specialDataSources);

        AddDataSources(dataSources);
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
            DateTime currentTimestamp = DateTime.Now;

            Task[] dataGatheringTasks = _dataSources
                .Where(source => (currentTimestamp - source.LastRefreshTimestamp) >= source.RefreshRate)
                .Select(dataSource => dataSource.GatherData())
                .ToArray();

            await Task.WhenAll(dataGatheringTasks);

            await Task.Delay(1000);
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
    public IDataSource GetDataSource(string dataSourceName)
    {
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
    /// Removes not subscribed data sources from repository content.
    /// </summary>
    public void RemoveNotSubscribedDataSources()
    {
        _dataSources
            .Where(source => !source.IsSubscribed)
            .ToList()
            .ForEach(source => _dataSources.Remove(source));
    }

    /// <summary>
    /// Checks if repository contains data source with specified name.
    /// </summary>
    /// <param name="dataSourceName">
    /// Name of data source, which shall be checked.
    /// </param>
    /// <returns>
    /// True or false, depending on check result.
    /// </returns>
    private bool ContainsDataSource(string dataSourceName)
    {
        return _dataSources
                .Where(dataSource => dataSource.Name == dataSourceName)
                .Any();
    }

    /// <summary>
    /// Adds provided data source to repository.
    /// </summary>
    /// <param name="dataSource">
    /// Data source, which shall be added to repository.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown, when at least one reference-type argument is a null reference.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown, when at least one argument will be considered as invalid.
    /// </exception>
    private void AddDataSource(IDataSource dataSource)
    {
        if (dataSource is null)
        {
            string argumentName = nameof(dataSource);
            const string ErrorMessage = "Provided data source is a null reference:";
            throw new ArgumentNullException(argumentName, ErrorMessage);
        }

        if (ContainsDataSource(dataSource.Name))
        {
            string argumentName = nameof(dataSource);
            string errorMessage = $"Data source with specified name already exists in repository: {dataSource.Name}";
            throw new ArgumentException(errorMessage, argumentName);
        }

        _dataSources.Add(dataSource);
    }

    /// <summary>
    /// Adds provided data sources to repository.
    /// </summary>
    /// <param name="dataSources">
    /// Collection of data sources, which shall be added to repository.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown, when at least one reference-type argument is a null reference.
    /// </exception>
    private void AddDataSources(IEnumerable<IDataSource> dataSources)
    {
        if (dataSources is null)
        {
            string argumentName = nameof(dataSources);
            const string ErrorMessage = "Provided data sources collection is a null reference:";
            throw new ArgumentNullException(argumentName, ErrorMessage);
        }

        dataSources
            .ToList()
            .ForEach(AddDataSource);
    }
    #endregion
}
