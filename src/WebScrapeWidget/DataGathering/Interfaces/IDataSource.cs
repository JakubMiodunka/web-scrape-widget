namespace WebScrapeWidget.DataGathering.Interfaces;

/// <summary>
/// Interface, which shall be implemented
/// by all types of data sources used within the application.
/// </summary>
public interface IDataSource
{
    bool WasDataGathered { get; }
    string GatheredData { get; }

    /// <summary>
    /// Triggers the process of gathering data from invoked data source.
    /// </summary>
    /// <returns>
    /// Returns data gathered from web source.
    /// </returns>
    Task GatherData();
}
