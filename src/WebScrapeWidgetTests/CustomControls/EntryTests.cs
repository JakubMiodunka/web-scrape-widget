using Moq;
using System.Windows.Controls;
using System.Xml.Linq;
using System.Linq;
using WebScrapeWidget.CustomControls;
using WebScrapeWidget.DataGathering.Interfaces;
using WebScrapeWidgetTests.DataGathering.DataSources;
using WebScrapeWidgetTests.DataGathering.Repositories;
using System.Windows;


namespace WebScrapeWidgetTests.CustomControls;

/// <summary>
/// Test fixture for Entry custom control type.
/// </summary>
[TestOf(typeof(Entry))]
[Author("Jakub Miodunka")]
public sealed class EntryTests
{
    #region Default values
    public static readonly string DefaultEntryLabel = "Entry label";
    #endregion

    #region Test parameters
    private static string[] s_validLabels = [DefaultEntryLabel];
    private static string[] s_invalidLabels = [string.Empty, " ", "\t", "\n"];
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
        return CreateEntryDefinition(DefaultEntryLabel, DataSourceTests.DefaultDataSourceName);
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

        var entryDefinition = CreateEntryDefinition(validLabel, DataSourceTests.DefaultDataSourceName);
        
        TestDelegate entryInstantiation = () => Entry.FromXmlElement(entryDefinition, dataSourcesRepositoryStub.Object);

        Assert.DoesNotThrow(entryInstantiation);
    }

    /// <summary>
    /// Checks if entry instantiation is impossible using provided string as entry label.
    /// </summary>
    /// <param name="invalidLabel">
    /// Label, using which entry instantiation shall not be possible.
    /// </param>
    [Apartment(ApartmentState.STA)]
    [TestCaseSource(nameof(s_invalidLabels))]
    public void InstantiationImpossibleUsingInvalidLabel(string invalidLabel)
    {
        Mock<IDataSourcesRepository> dataSourcesRepositoryStub = DataSourcesRepositoryTests.CreateFakeDefaultDataSourcesRepository();

        var entryDefinition = CreateEntryDefinition(invalidLabel, DataSourceTests.DefaultDataSourceName);

        TestDelegate entryInstantiation = () => Entry.FromXmlElement(entryDefinition, dataSourcesRepositoryStub.Object);

        Assert.Throws<ArgumentException>(entryInstantiation);
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
    /// Checks if entry is derivate class from System.Windows.Controls.Grid.
    /// </summary>
    [Test]
    [Apartment(ApartmentState.STA)]
    public void IsGridInstance()
    {
        Entry entryUnderTest = CreateDefaultEntry();

        Assert.That(entryUnderTest is Grid);
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
    /// Checks if entry content consists of only text blocks.
    /// </summary>
    [Test]
    [Apartment(ApartmentState.STA)]
    public void ContainsOnlyTextBlocks()
    {
        Entry entryUnderTest = CreateDefaultEntry();

        int childrenCount = entryUnderTest.Children.Count;

        for (int childIndex = 0; childIndex < childrenCount; childIndex++)
        {
            UIElement child = entryUnderTest.Children[childIndex];

            Assert.That(child is TextBlock);
        }
    }
    #endregion
}
