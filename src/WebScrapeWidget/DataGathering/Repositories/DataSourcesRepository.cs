using System.IO;

using WebScrapeWidget.DataGathering.Interfaces;
using WebScrapeWidget.DataGathering.Models;
using WebScrapeWidget.Utilities;


namespace WebScrapeWidget.DataGathering.Repositories;

/// <summary>
/// repository, that provided access to a pool of data sources.
/// </summary>
public sealed class DataSourcesRepository
{
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
    public DataSourcesRepository(string directoryPath, bool recursiveSearch)
    {
        FileSystemUtilities.ValidateDirectory(directoryPath);

        DirectoryPath = directoryPath;
        _dataSources = CreateDataSources(recursiveSearch);

        GatherDataFromAllSources();
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
    /// <remarks>
    /// Data gathering is performed in parallel.
    /// </remarks>
    private void GatherDataFromAllSources()
    {
        Task[] dataGatheringtasks = _dataSources
            .Select(dataSource => dataSource.GatherData())
            .ToArray();

        Task.WaitAll(dataGatheringtasks);
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
