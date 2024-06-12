using System.IO;

using WebScrapeWidget.DataGathering.Interfaces;
using WebScrapeWidget.DataGathering.Models;
using WebScrapeWidget.Utilities;


namespace WebScrapeWidget.DataGathering.Repositories;

/// <summary>
/// Repository, that provided access to a pool of data sources.
/// </summary>
/// <remarks>
/// Repository is implemented as singleton.
/// To gain access to singleton instance it is necessary to initialize it using
/// DataSourcesRepository.InitializeSingleton method, which is asynchronous.
/// Result of access to not initialized singleton instance will cause towing an exception.
/// This approach was utilized to avoid deadlocks ex. with UI thread.
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
                const string ErrorMessage = "Singleton of data sources repository was not initialized:";
                throw new InvalidOperationException(ErrorMessage);
            }

            return s_instance;
        }
    }

    /// <summary>
    /// Initializes repository singleton instance.
    /// </summary>
    /// <returns>
    /// Task, which will be completed, when initialization
    /// of repository singleton instance will be finished.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown, when repository singleton instance is already initialized.
    /// </exception>
    public static async Task InitializeSingleton()
    {
        if (s_instance is not null)
        {
            const string ErrorMessage = "Singleton of data sources repository was already initialized:";
            throw new InvalidOperationException(ErrorMessage);
        }

        string directoryPath = AppConfig.Instance.DataSourcesStorage;
        bool recursiveSearch = AppConfig.Instance.DataSourcesStorageRecursiveSearch;

        var instance = new DataSourcesRepository(directoryPath, recursiveSearch);

        await instance.GatherDataFromAllSources();

        s_instance = instance;
    }

    private static DataSourcesRepository? s_instance;
    #endregion

    #region Properties
    public readonly string DirectoryPath;
    
    private readonly IDataSource[] _dataSources;
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
        _dataSources = CreateDataSources(recursiveSearch);
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

    /// <summary>
    /// Gathers data from all sources contained by repository.
    /// </summary>
    /// <returns>
    /// Task, which will be completed, when data will be gathered
    /// from all data sources contained by the repository.
    /// </returns>
    private async Task GatherDataFromAllSources()
    {
        Task[] dataGatheringtasks = _dataSources
            .Select(dataSource => dataSource.GatherData())
            .ToArray();

        await Task.WhenAll(dataGatheringtasks);
    }
    #endregion

    #region Interactions
    /// <summary>
    /// Checks if data source with provided identifier is contained by the repository.
    /// </summary>
    /// <param name="name">
    /// Data source name, which shall be checked.
    /// </param>
    /// <returns>
    /// True or false, depending on check result.
    /// </returns>
    public bool ContainsDataSource(string name)
    {
        return _dataSources
            .Where(dataSource => dataSource.Name == name)
            .Any();
    }

    /// <summary>
    /// Provides access to data gathered from data source,
    /// which has specified identifier assigned.
    /// </summary>
    /// <param name="name">
    /// Name of data source, from which gathered data shall be obtained.
    /// </param>
    /// <returns>
    /// Data gathered from data source, which has specified identified assigned.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown, when repository does not contain data source with specified identifier. 
    /// </exception>
    public string GetDataFromSource(string name)
    {
        if (!ContainsDataSource(name))
        {
            string errorMessage = $"Data source with provided name does not exist in repository: {name}";
            throw new ArgumentOutOfRangeException(errorMessage);
        }

        return _dataSources
            .Where(dataSource => dataSource.Name == name)
            .First()
            .GatheredData;
    }
    #endregion
}
