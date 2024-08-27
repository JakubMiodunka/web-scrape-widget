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
    /// <param name="sender">
    /// Instance of data source, which sends the notification.
    /// </param>
    public void Notify(IDataSource sender);
}
