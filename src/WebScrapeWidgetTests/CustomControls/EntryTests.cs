// Ignore Spelling: Timestamp

using Moq;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;
using WebScrapeWidget.CustomControls;
using WebScrapeWidget.DataGathering.Interfaces;
using WebScrapeWidgetTests.DataGathering.DataSources;
using WebScrapeWidgetTests.DataGathering.Repositories;


namespace WebScrapeWidgetTests.CustomControls;

/// <summary>
/// Test fixture for Entry custom control type.
/// </summary>
[TestOf(typeof(Entry))]
[Author("Jakub Miodunka")]
public sealed class EntryTests
{
    #region Default values
    public static readonly string DefaultLabel = "Entry label";
    #endregion

    #region Test parameters
    private static string[] s_validLabels = [DefaultLabel];
    #endregion

    #region Auxiliary methods
    /// <summary>
    /// Creates entry definition containing specified properties.
    /// </summary>
    /// <param name="label">
    /// Entry label specified in generated definition. 
    /// </param>
    /// <param name="dataSourceName">
    /// Name of data source sued by entry specified in generated definition. 
    /// </param>
    /// <returns>
    /// Definition of entry containing specified properties.
    /// </returns>
    public static XElement CreateEntryDefinition(string label, string dataSourceName)
    {
        var entryDefinition = new XElement("Entry",
                new XAttribute("Label", label),
                new XAttribute("DataSourceName", dataSourceName));

        return entryDefinition;
    }

    /// <summary>
    /// Creates definition of default entry.
    /// </summary>
    /// <remarks>
    /// Default entry is referring to default implementation of fake data source.
    /// </remarks>
    /// <returns>
    /// Definition of default entry.
    /// </returns>
    public static XElement CreateDefaultEntryDefinition()
    {
        return CreateEntryDefinition(DefaultLabel, DataSourceTests.DefaultName);
    }

    /// <summary>
    /// Creates default instance of entry.
    /// </summary>
    /// <returns>
    /// Default instance of entry.
    /// </returns>
    public static Entry CreateDefaultEntry()
    {
        XElement defaultEntryDefinition = CreateDefaultEntryDefinition();
        Mock<IDataSourcesRepository> defaultDataSourcesRepositoryStub = DataSourcesRepositoryTests.CreateFakeDefaultDataSourcesRepository();

        return Entry.FromXmlElement(defaultEntryDefinition, defaultDataSourcesRepositoryStub.Object);
    }

    /// <summary>
    /// Converts provided UIElementCollection instance to List<UIElement>.
    /// </summary>
    /// <param name="uiElementCollection">
    /// UIElementCollection instance, which shall be converted.
    /// </param>
    /// <returns>
    /// Provide UIElementCollection instance converted to List<UIElement>.
    /// </returns>
    private static List<UIElement> ConvertToList(UIElementCollection uiElementCollection)
    {
        var list = new List<UIElement>();

        foreach (UIElement element in uiElementCollection)
        {
            list.Add(element);
        }

        return list;
    }
    #endregion

    #region Test cases
    /// <summary>
    /// Checks if entry instantiation is impossible using null reference as entry label.
    /// </summary>
    [Test]
    public void InstantiationImpossibleUsingNullReferenceAsLabel()
    {
        Mock<IDataSourcesRepository> dataSourcesRepositoryStub = DataSourcesRepositoryTests.CreateFakeDefaultDataSourcesRepository();

        TestDelegate entryInstantiation = () => Entry.FromXmlElement(null, dataSourcesRepositoryStub.Object);

        Assert.Throws<ArgumentNullException>(entryInstantiation);
    }

    /// <summary>
    /// Checks if entry instantiation is possible using provided string as entry label.
    /// </summary>
    /// <param name="validLabel">
    /// Label, using which entry instantiation shall be possible.
    /// </param>
    [Apartment(ApartmentState.STA)]
    [TestCaseSource(nameof(s_validLabels))]
    public void InstantiationPossibleUsingValidLabel(string validLabel)
    {
        Mock<IDataSourcesRepository> dataSourcesRepositoryStub = DataSourcesRepositoryTests.CreateFakeDefaultDataSourcesRepository();

        var entryDefinition = CreateEntryDefinition(validLabel, DataSourceTests.DefaultName);
        
        TestDelegate entryInstantiation = () => Entry.FromXmlElement(entryDefinition, dataSourcesRepositoryStub.Object);

        Assert.DoesNotThrow(entryInstantiation);
    }

    /// <summary>
    /// Checks if entry instantiation is impossible using null reference as data sources repository,
    /// from which data source aimed by entry shall be obtained.
    /// </summary>
    [Test]
    public void InstantiationImpossibleUsingNullReferenceAsDataSourceRepository()
    {
        XElement defaultEntryDefinition = CreateDefaultEntryDefinition();
        
        TestDelegate entryInstantiation = () => Entry.FromXmlElement(defaultEntryDefinition, null);

        Assert.Throws<ArgumentNullException>(entryInstantiation);
    }

    /// <summary>
    /// Checks if text displayed by label text block is the same as entry label specified in its definition.
    /// </summary>
    [Test]
    [Apartment(ApartmentState.STA)]
    public void LabelTextBlockTextSetAccordingToEntryDefinition()
    {
        Entry entryUnderTest = CreateDefaultEntry();

        string expectedLabelTextBlockText = DefaultLabel;
        string actualLabelTextBlockText = entryUnderTest.Label.Text;

        Assert.That(actualLabelTextBlockText == expectedLabelTextBlockText);
    }

    /// <summary>
    /// Checks if value text block uses specified string as unknown value indicator,
    /// when data source, to which entry is referring to does not notify it about gathered data.
    /// </summary>
    /// <param name="expectedUnknownValueIndicator">
    /// String, which shall be used as unknown value indicator.
    /// </param>
    [TestCase("-")]
    [Apartment(ApartmentState.STA)]
    public void ValueTextBlockUsesUnknownValueIndicator(string expectedUnknownValueIndicator)
    {
        Entry entryUnderTest = CreateDefaultEntry();

        string actualUnknownValueIndicator = entryUnderTest.Value.Text;

        Assert.That(actualUnknownValueIndicator == expectedUnknownValueIndicator);
    }

    /// <summary>
    /// Checks if notification changes text displayed by value text block
    /// to value of data gathered by referenced data source.
    /// </summary>
    [Test]
    [Apartment(ApartmentState.STA)]
    public void NotificationChangesTextDisplayedByValueTextBlock()
    {
        Mock<IDataSource> dataSourceStub = DataSourceTests.CreateFakeDefaultDataSource();
        Entry entryUnderTest = CreateDefaultEntry();

        entryUnderTest.Notify(dataSourceStub.Object);

        string expectedText = $"{DataSourceTests.DefaultGatheredData} {DataSourceTests.DefaultDataUnit}";
        string actualText = entryUnderTest.Value.Text;

        Assert.That(actualText == expectedText);
    }

    /// <summary>
    /// Checks if entry tool tip is a string instance.
    /// </summary>
    [Test]
    [Apartment(ApartmentState.STA)]
    public void ToolTipIsStringInstance()
    {
        Entry entryUnderTest = CreateDefaultEntry();

        Assert.That(entryUnderTest.ToolTip is string);
    }

    /// <summary>
    /// Checks if tool tip content includes name of data source, to which entry refers.
    /// </summary>
    [Test]
    [Apartment(ApartmentState.STA)]
    public void ToolTipContainsDataSourceName()
    {
        Entry entryUnderTest = CreateDefaultEntry();

        string toolTipContent = (string)entryUnderTest.ToolTip;

        Assert.That(toolTipContent.Contains(DataSourceTests.DefaultName));
    }

    /// <summary>
    /// Checks if tool tip content includes description of data source, to which entry refers.
    /// </summary>
    [Test]
    [Apartment(ApartmentState.STA)]
    public void ToolTipContainsDataSourceDescription()
    {
        Entry entryUnderTest = CreateDefaultEntry();

        string toolTipContent = (string)entryUnderTest.ToolTip;

        Assert.That(toolTipContent.Contains(DataSourceTests.DefaultDescription));
    }

    /// <summary>
    /// Checks if tool tip content includes refresh rate of data source, to which entry refers.
    /// </summary>
    /// <param name="expectedRefreshRateFormat">
    /// Refresh rate format expected to be used in tool tip content.
    /// </param>
    [TestCase("c")]
    [Apartment(ApartmentState.STA)]
    public void ToolTipContainsDataSourceRefreshRate(string expectedRefreshRateFormat)
    {
        Entry entryUnderTest = CreateDefaultEntry();

        string toolTipContent = (string)entryUnderTest.ToolTip;

        string expectedFormatedRefreshRate = DataSourceTests.DefaultRefreshRate.ToString(expectedRefreshRateFormat);

        Assert.That(toolTipContent.Contains(expectedFormatedRefreshRate));
    }

    /// <summary>
    /// Checks if entry content is divided into specified number of columns.
    /// </summary>
    /// <param name="expectedColumnsCount">
    /// Number of columns, on which entry content shall be divided.
    /// </param>
    [TestCase(2)]
    [Apartment(ApartmentState.STA)]
    public void DividedIntoColumns(int expectedColumnsCount)
    {
        Entry entryUnderTest = CreateDefaultEntry();

        int actualColumnsCount = entryUnderTest.ColumnDefinitions.Count();

        Assert.That(actualColumnsCount == expectedColumnsCount);
    }

    /// <summary>
    /// Checks if entry content is divided into specified number of rows.
    /// </summary>
    /// <param name="expectedRowsCount">
    /// Number of rows, on which entry content shall be divided.
    /// </param>
    [TestCase(0)]
    [Apartment(ApartmentState.STA)]
    public void DividedIntoRows(int expectedRowsCount)
    {
        Entry entryUnderTest = CreateDefaultEntry();

        int actualRowsCount = entryUnderTest.RowDefinitions.Count();

        Assert.That(actualRowsCount == expectedRowsCount);
    }

    /// <summary>
    /// Checks if entry content contains specified amount of children.
    /// </summary>
    /// <param name="expectedChildrenCount">
    /// Number of children, which shall be contained by entry content.
    /// </param>
    [TestCase(2)]
    [Apartment(ApartmentState.STA)]
    public void ContainsChildren(int expectedChildrenCount)
    {
        Entry entryUnderTest = CreateDefaultEntry();

        int actualChildrenCount = entryUnderTest.Children.Count;

        Assert.That(actualChildrenCount == expectedChildrenCount);
    }

    /// <summary>
    /// Checks if entry content contains label text block.
    /// </summary>
    [Test]
    [Apartment(ApartmentState.STA)]
    public void ContainsLabelTextBlock()
    {
        Entry entryUnderTest = CreateDefaultEntry();

        List<UIElement> children = ConvertToList(entryUnderTest.Children);

        bool isLabelTextBlockInChildren = children.Any(child => object.ReferenceEquals(child, entryUnderTest.Label));

        Assert.True(isLabelTextBlockInChildren);
    }

    /// <summary>
    /// Checks if entry content contains value text block.
    /// </summary>
    [Test]
    [Apartment(ApartmentState.STA)]
    public void ContainsValueTextBlock()
    {
        Entry entryUnderTest = CreateDefaultEntry();

        List<UIElement> children = ConvertToList(entryUnderTest.Children);

        bool isLabelTextBlockInChildren = children.Any(child => ReferenceEquals(child, entryUnderTest.Value));

        Assert.True(isLabelTextBlockInChildren);
    }

    /// <summary>
    /// Checks if label text block is placed in specified column.
    /// </summary>
    /// <param name="expectedLabelTextBlockColumn">
    /// Column, in which label text block shall be placed.
    /// </param>
    [TestCase(0)]
    [Apartment(ApartmentState.STA)]
    public void LabelTextBlockPlacedInSpecifiedColumn(int expectedLabelTextBlockColumn)
    {
        Entry entryUnderTest = CreateDefaultEntry();

        int actualLabelTextBlockColumn = Grid.GetColumn(entryUnderTest.Label);

        Assert.That(actualLabelTextBlockColumn == expectedLabelTextBlockColumn);
    }

    /// <summary>
    /// Checks if value text block is placed in specified column.
    /// </summary>
    /// <param name="expectedValueTextBlockColumn">
    /// Column, in which value text block shall be placed.
    /// </param>
    [TestCase(1)]
    [Apartment(ApartmentState.STA)]
    public void ValueTextBlockPlacedInSpecifiedColumn(int expectedValueTextBlockColumn)
    {
        Entry entryUnderTest = CreateDefaultEntry();

        int actualValueTextBlockColumn = Grid.GetColumn(entryUnderTest.Value);

        Assert.That(actualValueTextBlockColumn == expectedValueTextBlockColumn);
    }
    #endregion
}
