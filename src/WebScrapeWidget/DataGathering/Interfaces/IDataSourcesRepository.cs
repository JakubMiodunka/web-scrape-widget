namespace WebScrapeWidget.DataGathering.Interfaces;

/// <summary>
/// Interface, which shall be implemented
/// by types of data sources repositories.
/// </summary>
public interface IDataSourcesRepository
{
    /// <summary>
    /// Searches through repository for data source with specified name.
    /// </summary>
    /// <param name="dataSourceName">
    /// Name of data source, which shall be returned.
    /// </param>
    /// <returns>
    /// Data source contained by the repository, with specified name.
    /// </returns>
    IDataSource GetDataSource(string dataSourceName);
}
