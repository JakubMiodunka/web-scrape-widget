// Ignore Spelling: Timestamp

using Moq;
using WebScrapeWidget.CustomControls;
using WebScrapeWidget.DataGathering.DataSources;
using WebScrapeWidget.DataGathering.Interfaces;


namespace WebScrapeWidgetTests.DataGathering.DataSources;

/// <summary>
/// Test fixture for DataSource type.
/// </summary>
[TestOf(typeof(DataSource))]
[Author("Jakub Miodunka")]
public sealed class DataSourceTests
{
    #region Default values
    public const string DefaultDataSourceName = "Data source name";
    public const string DefaultDataSourceDescription = "Data source description";
    public static readonly TimeSpan DefaultDataSourceRefreshRate = TimeSpan.FromHours(1);
    public static readonly DateTime DefaultDataSourceLastRefreshTimestamp = new DateTime(2024, 9, 12);
    #endregion

    #region Fakes creation
    /// <summary>
    /// Creates fake implementation of data source according to specified arguments.
    /// </summary>
    /// <param name="name">
    /// Name of created data source.
    /// </param>
    /// <param name="description">
    /// Description of created data source.
    /// </param>
    /// <param name="refreshRate">
    /// Refresh rate of created data source.
    /// </param>
    /// <param name="lastRefreshTimestamp">
    /// Last refresh timestamps of created data source.
    /// </param>
    /// <returns>
    /// Fake implementation of data source according to specified arguments.
    /// </returns>
    public static Mock<IDataSource> CreateFakeDataSource(string name, string description, TimeSpan refreshRate, DateTime lastRefreshTimestamp)
    {
        var fakeDataSource = new Mock<IDataSource>();
        fakeDataSource.Setup(dataSource => dataSource.Name).Returns(name);
        fakeDataSource.Setup(dataSource => dataSource.Description).Returns(description);
        fakeDataSource.Setup(dataSource => dataSource.RefreshRate).Returns(refreshRate);
        fakeDataSource.Setup(dataSource => dataSource.LastRefreshTimestamp).Returns(lastRefreshTimestamp);
        fakeDataSource.Setup(dataSource => dataSource.AddSubscriber(It.IsAny<Entry>()));

        return fakeDataSource;
    }

    /// <summary>
    /// Creates fake implementation of default data source.
    /// </summary>
    /// <returns>
    /// Fake implementation of default data source.
    /// </returns>
    public static Mock<IDataSource> CreateFakeDefaultDataSource()
    {
        return CreateFakeDataSource(DefaultDataSourceName, DefaultDataSourceDescription, DefaultDataSourceRefreshRate, DefaultDataSourceLastRefreshTimestamp);
    }
    #endregion
}
