// Ignore Spelling: Timestamp

namespace WebScrapeWidget.DataGathering.Interfaces;


/// <summary>
/// Interface, which shall be implemented
/// by all types of data sources used within the application.
/// </summary>
public interface IDataSource
{
    string Name { get; }
    bool WasDataGathered { get; }
    string GatheredData { get; }

    string DataUnit { get; }
    TimeSpan RefreshRate { get; }
    DateTime LastRefreshTimestamp { get; }

    /// <summary>
    /// Triggers the process of gathering data from invoked data source.
    /// </summary>
    /// <returns>
    /// Task related to data gathering process.
    /// </returns>
    Task GatherData();

    /// <summary>
    /// Adds a new object to subscribers list of data source.
    /// </summary>
    /// <param name="subscriber">
    /// Object, which shall be added to data source subscriber list.
    /// </param>
    void AddSubscriber(IDataSourceSubscriber subscriber);
}
