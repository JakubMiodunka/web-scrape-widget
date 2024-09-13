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
    public const string DefaultName = "Data source name";
    public const string DefaultDescription = "Data source description";
    public static readonly TimeSpan DefaultRefreshRate = TimeSpan.FromHours(1);
    public static readonly DateTime DefaultLastRefreshTimestamp = new DateTime(2024, 9, 12);
    public const string DefaultGatheredData = "Data gathered by data source";
    public const string DefaultDataUnit = "Data unit of gathered data";
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
    public static Mock<IDataSource> CreateFakeDataSource(
        string name,
        string description,
        TimeSpan refreshRate,
        DateTime lastRefreshTimestamp,
        bool wasDataGathered,
        string gatheredData,
        string dataUnit)
    {
        var fakeDataSource = new Mock<IDataSource>();
        fakeDataSource.Setup(dataSource => dataSource.Name).Returns(name);
        fakeDataSource.Setup(dataSource => dataSource.Description).Returns(description);
        fakeDataSource.Setup(dataSource => dataSource.RefreshRate).Returns(refreshRate);
        fakeDataSource.Setup(dataSource => dataSource.LastRefreshTimestamp).Returns(lastRefreshTimestamp);
        fakeDataSource.Setup(dataSource => dataSource.WasDataGathered).Returns(wasDataGathered);
        fakeDataSource.Setup(dataSource => dataSource.GatheredData).Returns(gatheredData);
        fakeDataSource.Setup(dataSource => dataSource.DataUnit).Returns(dataUnit);
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
        return CreateFakeDataSource(
            DefaultName,
            DefaultDescription,
            DefaultRefreshRate,
            DefaultLastRefreshTimestamp,
            true,
            DefaultGatheredData,
            DefaultDataUnit);
    }
    #endregion
}
