// Ignore Spelling: Timestamp

namespace WebScrapeWidget.DataGathering.Interfaces;


/// <summary>
/// Interface, which shall be implement by all types, that shall be able to subscribe to data source.
/// </summary>
public interface IDataSourceSubscriber
{
    /// <summary>
    /// Shall be invoked by subscribed data source, when new data will be gathered.
    /// </summary>
    /// <param name="gatheredData">
    /// New value of data contained by subscribed data source.
    /// </param>
    /// <param name="refreshTimestamp">
    /// Timestamp, when data contained by subscribed data source was refreshed.
    /// </param>
    public void Notify(string gatheredData, DateTime refreshTimestamp);
}
