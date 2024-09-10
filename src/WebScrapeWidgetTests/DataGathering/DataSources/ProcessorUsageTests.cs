// Ignore Spelling: Timestamp

using Moq;
using System.Text.RegularExpressions;
using WebScrapeWidget.DataGathering.DataSources;
using WebScrapeWidget.DataGathering.Interfaces;


namespace WebScrapeWidgetTests.DataGathering.DataSources;

/// <summary>
/// Test fixture for ProcessorUsage data source type.
/// </summary>
[TestOf(typeof(ProcessorUsage))]
[Author("Jakub Miodunka")]
public sealed class ProcessorUsageTests
{
    #region Default data source instance.
    private static string _defaultName = nameof(ProcessorUsage);
    private static TimeSpan _defaultRefreshRate = TimeSpan.FromHours(1);

    private static ProcessorUsage CreateDefaultDataSource()
    {
        return new ProcessorUsage(_defaultName, _defaultRefreshRate);
    }
    #endregion

    #region Test parameters
    private static string?[] s_validNames = [_defaultName];
    private static string?[] s_invalidNames = [null, string.Empty];
    private static TimeSpan[] s_validRefreshRates = [TimeSpan.FromSeconds(2), TimeSpan.MaxValue];
    private static TimeSpan[] s_invalidRefreshRates = [TimeSpan.MinValue, TimeSpan.Zero, TimeSpan.FromMilliseconds(1999)];
    #endregion

    #region Test cases
    /// <summary>
    /// Checks if instantiation of data source using specified name is possible.
    /// </summary>
    /// <param name="validName">
    /// Name of data source, using which instantiation of data source shall be possible.
    /// </param>
    [TestCaseSource(nameof(s_validNames))]
    public void InstantiationPossibleUsingValidName(string validName)
    {
        string expectedName = validName;

        var dataSourceUnderTest = new ProcessorUsage(expectedName, _defaultRefreshRate);
        
        string actualName = dataSourceUnderTest.Name;

        Assert.That(actualName == expectedName);
    }

    /// <summary>
    /// Checks if instantiation of data source using specified name is possible not possible.
    /// </summary>
    /// <param name="invalidName">
    /// Name of data source, using which instantiation of data source shall not be possible.
    /// </param>
    [TestCaseSource(nameof(s_invalidNames))]
    public void InstantiationImpossibleUsingInvalidName(string? invalidName)
    {
        TestDelegate dataSourceCreation = () => new ProcessorUsage(invalidName, _defaultRefreshRate);

        if (invalidName is null)
        {
            Assert.Throws<ArgumentNullException>(dataSourceCreation);
        }
        else
        {
            Assert.Throws<ArgumentOutOfRangeException>(dataSourceCreation);
        }
    }

    /// <summary>
    /// Checks if data source description is set to fixed, predefined value.
    /// </summary>
    [Test]
    public void FixedDescriptionValue()
    {
        const string ExpectedDescription = "Current CPU usage.";

        ProcessorUsage dataSourceUnderTest = CreateDefaultDataSource();
        var actualDescription = dataSourceUnderTest.Description;

        Assert.That(actualDescription == ExpectedDescription);
    }

    /// <summary>
    /// Checks if data unit used by data source is set to fixed, predefined value.
    /// </summary>
    [Test]
    public void FixedDataUnitValue()
    {
        const string ExpectedDataUnit = "%";

        ProcessorUsage dataSourceUnderTest = CreateDefaultDataSource();
        
        var actualDataUnit = dataSourceUnderTest.DataUnit;

        Assert.That(actualDataUnit == ExpectedDataUnit);
    }

    /// <summary>
    /// Checks if instantiation of data source using specified refresh rate is possible.
    /// </summary>
    /// <param name="validRefreshRate">
    /// Refresh rate, using which instantiation of data source shall be possible.
    /// </param>
    [TestCaseSource(nameof(s_validRefreshRates))]
    public void InstantiationPossibleUsingValidRefreshRate(TimeSpan validRefreshRate)
    {
        TimeSpan expectedRefreshRate = validRefreshRate;

        var dataSourceUnderTest = new ProcessorUsage(_defaultName, expectedRefreshRate);
        
        TimeSpan actualRefreshRate = dataSourceUnderTest.RefreshRate;

        Assert.That(actualRefreshRate == expectedRefreshRate);
    }

    /// <summary>
    /// Checks if instantiation of data source using specified refresh rate is not possible.
    /// </summary>
    /// <param name="invalidRefreshRate">
    /// Refresh rate, using which instantiation of data source shall not be possible.
    /// </param>
    [TestCaseSource(nameof(s_invalidRefreshRates))]
    public void InstantiationImpossibleUsingInvalidRefreshRate(TimeSpan invalidRefreshRate)
    {
        TestDelegate dataSourceCreation = () => new ProcessorUsage(_defaultName, invalidRefreshRate);

        Assert.Throws<ArgumentOutOfRangeException>(dataSourceCreation);
    }

    /// <summary>
    /// Checks if data unavailability within data source is correctly indicated by its corresponding property.
    /// </summary>
    [Test]
    public void DataUnavailabilityIndicatedViaPropertyValue()
    {
        ProcessorUsage dataSourceUnderTest = CreateDefaultDataSource();

        Assert.False(dataSourceUnderTest.WasDataGathered);
    }

    /// <summary>
    /// Checks if data availability within data source is correctly indicated by its corresponding property.
    /// </summary>
    [Test]
    public void DataAvailabilityIndicatedViaPropertyValue()
    {
        ProcessorUsage dataSourceUnderTest = CreateDefaultDataSource();

        Task dataGatheringTask = dataSourceUnderTest.GatherData();
        Task.WaitAll(dataGatheringTask);

        Assert.True(dataSourceUnderTest.WasDataGathered);
    }

    /// <summary>
    /// Checks if access to data gathered from data source is not possible
    /// before data gathering process.
    /// </summary>
    [Test]
    public void DataAccessNotPossibleBeforeDataGathering()
    {
        var dataSourceUnderTest = CreateDefaultDataSource();

        TestDelegate accessGatheredData = () => { string gatheredData = dataSourceUnderTest.GatheredData; };

        Assert.Throws<InvalidOperationException>(accessGatheredData);
    }

    /// <summary>
    /// Check if data gathered from data source is percentage value.
    /// </summary>
    [Test]
    public void GatheredDataIsPercentageValue()
    {
        var expecedValuePattern = new Regex(@"^\d$|^[1-9]\d$|^100$");
        ProcessorUsage dataSourceUnderTest = CreateDefaultDataSource();

        Task dataGatheringTask = dataSourceUnderTest.GatherData();
        Task.WaitAll(dataGatheringTask);

        string actualValue = dataSourceUnderTest.GatheredData;

        Assert.True(expecedValuePattern.IsMatch(actualValue));
    }

    /// <summary>
    /// Checks if last refresh timestamp is set initially (before first data gathering)
    /// to fixed, predefined value.
    /// </summary>
    [Test]
    public void FixedInitialValueOfLastRefreshTimestamp()
    {
        var expectedInitialLastRefreshTimestamp = DateTime.MinValue;

        ProcessorUsage dataSourceUnderTest = CreateDefaultDataSource();

        var actualInitialLastRefreshTimestamp = dataSourceUnderTest.LastRefreshTimestamp;

        Assert.That(actualInitialLastRefreshTimestamp == expectedInitialLastRefreshTimestamp);
    }

    /// <summary>
    /// Checks if last refresh timestamp is set accordingly to finish time of data gathering process.
    /// </summary>
    [Test]
    public void AccurateValueOfLastRefreshTimestamp()
    {
        ProcessorUsage dataSourceUnderTest = CreateDefaultDataSource();

        Task dataGatheringTask = dataSourceUnderTest.GatherData();
        Task.WaitAll(dataGatheringTask);

        var expectedLastRefreshTimesatamp = DateTime.Now;
        DateTime actualLastRefreshTimesatmp = dataSourceUnderTest.LastRefreshTimestamp;
        var accetableTimestampsDiviation = TimeSpan.FromMilliseconds(500);

        Assert.That(expectedLastRefreshTimesatamp - actualLastRefreshTimesatmp < accetableTimestampsDiviation);
    }

    /// <summary>
    /// Checks if absence of any data source subscribers is indicated
    /// by data source via its corresponding property.
    /// </summary>
    [Test]
    public void AbsenceOfSubscribersIndicatedViaPropertyValue()
    {
        ProcessorUsage dataSourceUnderTest = CreateDefaultDataSource();

        Assert.False(dataSourceUnderTest.IsSubscribed);
    }

    /// <summary>
    /// Checks if presence of data source subscribers is indicated
    /// by data source via its corresponding property.
    /// </summary>
    [Test]
    public void PresenceOfSubscribersIndicatedViaPropertyValue()
    {
        var dataSourceSubscriberMock = new Mock<IDataSourceSubscriber>();
        
        ProcessorUsage dataSourceUnderTest = CreateDefaultDataSource();
        dataSourceUnderTest.AddSubscriber(dataSourceSubscriberMock.Object);

        Assert.True(dataSourceUnderTest.IsSubscribed);
    }

    /// <summary>
    /// Checks if data source subscribers are notified by data source about newly gathered data.
    /// </summary>
    [Test]
    public void SubscribersAreNotifiedAboutNewlyGatheredData()
    {
        ProcessorUsage dataSourceUnderTest = CreateDefaultDataSource();
        
        var dataSourceSubscriberMock = new Mock<IDataSourceSubscriber>();
        dataSourceSubscriberMock.Setup(subscriber => subscriber.Notify(dataSourceUnderTest));
        
        dataSourceUnderTest.AddSubscriber(dataSourceSubscriberMock.Object);

        Task dataGatheringTask = dataSourceUnderTest.GatherData();
        Task.WaitAll(dataGatheringTask);

        dataSourceSubscriberMock.Verify(subscriber => subscriber.Notify(dataSourceUnderTest), Times.Once());
    }
    #endregion
}
