﻿// Ignore Spelling: Timestamp

using Moq;
using System.Text.RegularExpressions;
using WebScrapeWidget.DataGathering.DataSources;
using WebScrapeWidget.DataGathering.Interfaces;


namespace WebScrapeWidgetTests.DataGathering.DataSources;

/// <summary>
/// Test fixture for RamUsage data source type.
/// </summary>
[TestOf(typeof(RamUsage))]
[Author("Jakub Miodunka")]
public sealed class RamUsageTests
{
    #region Default values
    public const string DefaultName = "Ram usage name";
    public static readonly TimeSpan _defaultRefreshRate = TimeSpan.FromHours(1);
    #endregion

    #region Auxiliary methods
    /// <summary>
    /// Creates data source instance with default properties.
    /// </summary>
    /// <returns>
    /// New data source instance with default properties.
    /// </returns>
    public static RamUsage CreateDefaultDataSource()
    {
        return new RamUsage(DefaultName, _defaultRefreshRate);
    }
    #endregion

    #region Test parameters
    private static string[] s_validNames = [DefaultName];
    private static string[] s_invalidNames = [string.Empty];
    private static TimeSpan[] s_validRefreshRates = [TimeSpan.FromSeconds(2), TimeSpan.MaxValue];
    private static TimeSpan[] s_invalidRefreshRates = [TimeSpan.MinValue, TimeSpan.Zero, TimeSpan.FromMilliseconds(1999)];
    #endregion

    #region Test cases
    /// <summary>
    /// Checks if instantiation of data source using null reference as its name is not possible.
    /// </summary>
    [Test]
    public void InstantiationImpossibleUsingNullReferenceAsName()
    {
        TestDelegate dataSourceCreation = () => new RamUsage(null, _defaultRefreshRate);

        Assert.Throws<ArgumentException>(dataSourceCreation);
    }

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

        var dataSourceUnderTest = new RamUsage(expectedName, _defaultRefreshRate);

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
    public void InstantiationImpossibleUsingInvalidName(string invalidName)
    {
        TestDelegate dataSourceCreation = () => new RamUsage(invalidName, _defaultRefreshRate);

        Assert.Throws<ArgumentException>(dataSourceCreation);
    }

    /// <summary>
    /// Checks if data source description is set to fixed, predefined value.
    /// </summary>
    [Test]
    public void FixedDescriptionValue()
    {
        const string ExpectedDescription = "Current RAM usage.";

        RamUsage dataSourceUnderTest = CreateDefaultDataSource();
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

        RamUsage dataSourceUnderTest = CreateDefaultDataSource();

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

        var dataSourceUnderTest = new RamUsage(DefaultName, expectedRefreshRate);

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
        TestDelegate dataSourceCreation = () => new RamUsage(DefaultName, invalidRefreshRate);

        Assert.Throws<ArgumentOutOfRangeException>(dataSourceCreation);
    }

    /// <summary>
    /// Checks if data unavailability within data source is correctly indicated by its corresponding property.
    /// </summary>
    [Test]
    public void DataUnavailabilityIndicatedViaPropertyValue()
    {
        RamUsage dataSourceUnderTest = CreateDefaultDataSource();

        Assert.False(dataSourceUnderTest.WasDataGathered);
    }

    /// <summary>
    /// Checks if data availability within data source is correctly indicated by its corresponding property.
    /// </summary>
    [Test]
    public void DataAvailabilityIndicatedViaPropertyValue()
    {
        RamUsage dataSourceUnderTest = CreateDefaultDataSource();

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
        RamUsage dataSourceUnderTest = CreateDefaultDataSource();

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

        RamUsage dataSourceUnderTest = CreateDefaultDataSource();

        var actualInitialLastRefreshTimestamp = dataSourceUnderTest.LastRefreshTimestamp;

        Assert.That(actualInitialLastRefreshTimestamp == expectedInitialLastRefreshTimestamp);
    }

    /// <summary>
    /// Checks if last refresh timestamp is set accordingly to finish time of data gathering process.
    /// </summary>
    [Test]
    public void AccurateValueOfLastRefreshTimestamp()
    {
        RamUsage dataSourceUnderTest = CreateDefaultDataSource();

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
        RamUsage dataSourceUnderTest = CreateDefaultDataSource();

        Assert.False(dataSourceUnderTest.IsSubscribed);
    }

    /// <summary>
    /// Checks if presence of data source subscribers is indicated
    /// by data source via its corresponding property.
    /// </summary>
    [Test]
    public void PresenceOfSubscribersIndicatedViaPropertyValue()
    {
        var dataSourceSubscriberStub = new Mock<IDataSourceSubscriber>();

        RamUsage dataSourceUnderTest = CreateDefaultDataSource();
        dataSourceUnderTest.AddSubscriber(dataSourceSubscriberStub.Object);

        Assert.True(dataSourceUnderTest.IsSubscribed);
    }

    /// <summary>
    /// Checks if data source subscribers are notified by data source about newly gathered data.
    /// </summary>
    [Test]
    public void SubscribersAreNotifiedAboutNewlyGatheredData()
    {
        RamUsage dataSourceUnderTest = CreateDefaultDataSource();

        var dataSourceSubscriberMock = new Mock<IDataSourceSubscriber>();
        dataSourceSubscriberMock.Setup(subscriber => subscriber.Notify(dataSourceUnderTest));

        dataSourceUnderTest.AddSubscriber(dataSourceSubscriberMock.Object);

        Task dataGatheringTask = dataSourceUnderTest.GatherData();
        Task.WaitAll(dataGatheringTask);

        dataSourceSubscriberMock.Verify(subscriber => subscriber.Notify(dataSourceUnderTest), Times.Once());
    }
    #endregion
}
