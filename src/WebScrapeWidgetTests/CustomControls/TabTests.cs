using Moq;
using System.Xml.Linq;
using WebScrapeWidget.CustomControls;
using WebScrapeWidget.DataGathering.Interfaces;
using WebScrapeWidgetTests.DataGathering.Repositories;


namespace WebScrapeWidgetTests.CustomControls;

/// <summary>
/// Test fixture for Tab custom control type.
/// </summary>
[TestOf(typeof(Tab))]
[Author("Jakub Miodunka")]
public sealed class TabTests
{
    #region Default values
    public const string DefaultName = "Tab name";
    #endregion

    #region Auxiliary methods
    /// <summary>
    /// Creates XML element, which defines tab with specified name
    /// and containing set of provided section definitions. 
    /// </summary>
    /// <param name="name">
    /// Name of tab, to which created definition shall refer to.
    /// </param>
    /// <param name="sectionsDefinitions">
    /// Definitions of sections, which shall be contained by defined tab.
    /// </param>
    /// <returns>
    /// XML element, which defines tab with specified name
    /// and containing set of provided section definitions. 
    /// </returns>
    public static XElement CreateTabDefinition(string name, params XElement[] sectionsDefinitions)
    {
        var tabDefinition = new XElement("Tab",
                new XAttribute("Name", name));

        sectionsDefinitions.ToList().ForEach(tabDefinition.Add);

        return tabDefinition;
    }

    /// <summary>
    /// Creates XML element, which defines tab with default properties.
    /// </summary>
    /// <returns>
    /// XML element, which defines tab with default properties.
    /// </returns>
    public static XElement CreateDefaultTabDefinition()
    {
        XElement defaultSectionDefinition = SectionTests.CreateDefaultSectionDefinition();

        return CreateTabDefinition(DefaultName, defaultSectionDefinition);
    }

    /// <summary>
    /// Creates tab instance with default properties.
    /// </summary>
    /// <returns>
    /// New tab instance with default properties.
    /// </returns>
    public static Tab CreateDefaultTab()
    {
        XElement defaultTabDefinition = CreateDefaultTabDefinition();
        Mock<IDataSourcesRepository> defaultDataSourcesRepositoryStub = DataSourcesRepositoryTests.CreateFakeDefaultDataSourcesRepository();

        return Tab.FromXmlElement(defaultTabDefinition, defaultDataSourcesRepositoryStub.Object);
    }
    #endregion

    #region Test parameters
    private static string[] s_validNames = [DefaultName];
    #endregion

    #region Test cases
    /// <summary>
    /// Checks if tab instantiation is impossible using null reference as tab name.
    /// </summary>
    [Test]
    public void InstantiationImpossibleUsingNullReferenceAsName()
    {
        Mock<IDataSourcesRepository> dataSourcesRepositoryStub = DataSourcesRepositoryTests.CreateFakeDefaultDataSourcesRepository();

        TestDelegate tabInstantiation = () => Tab.FromXmlElement(null, dataSourcesRepositoryStub.Object);

        Assert.Throws<ArgumentNullException>(tabInstantiation);
    }

    /// <summary>
    /// Checks if tab instantiation is possible using provide string as tab name.
    /// </summary>
    /// <param name="validName">
    /// Name, using which tab instantiation shall be possible.
    /// </param>
    [Apartment(ApartmentState.STA)]
    [TestCaseSource(nameof(s_validNames))]
    public void InstantiationPossibleUsingValidName(string validName)
    {
        Mock<IDataSourcesRepository> dataSourcesRepositoryStub = DataSourcesRepositoryTests.CreateFakeDefaultDataSourcesRepository();
        XElement defaultSectionDefiniton = SectionTests.CreateDefaultSectionDefinition();

        var tabDefinition = CreateTabDefinition(validName, defaultSectionDefiniton);

        TestDelegate tabInstantiation = () => Tab.FromXmlElement(tabDefinition, dataSourcesRepositoryStub.Object);

        Assert.DoesNotThrow(tabInstantiation);
    }

    /// <summary>
    /// Checks if tab instantiation is impossible using null reference as data sources repository.
    /// </summary>
    [Test]
    public void InstantiationImpossibleUsingNullReferenceAsDataSourceRepository()
    {
        XElement defaultTabnDefinition = CreateDefaultTabDefinition();

        TestDelegate tabInstantiation = () => Tab.FromXmlElement(defaultTabnDefinition, null);

        Assert.Throws<ArgumentNullException>(tabInstantiation);
    }

    /// <summary>
    /// Checks if tab header is a string instance.
    /// </summary>
    [Test]
    [Apartment(ApartmentState.STA)]
    public void HeaderIsString()
    {
        Tab tabUnderTest = CreateDefaultTab();

        Assert.That(tabUnderTest.Header is string);
    }

    /// <summary>
    /// Checks if tab header is set to specified tab name.
    /// </summary>
    [Test]
    [Apartment(ApartmentState.STA)]
    public void IsHeaderEqualToSpecifiedName()
    {
        Tab tabUnderTest = CreateDefaultTab();

        string expectedHeader = DefaultName;
        string? actualHeader = tabUnderTest.Header as string;

        Assert.That(actualHeader == expectedHeader);
    }
    #endregion
}
