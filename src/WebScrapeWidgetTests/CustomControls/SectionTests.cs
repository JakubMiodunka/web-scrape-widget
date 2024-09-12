﻿using Moq;
using System.DirectoryServices;
using System.Xml.Linq;
using WebScrapeWidget.CustomControls;
using WebScrapeWidget.DataGathering.Interfaces;
using WebScrapeWidgetTests.DataGathering.Repositories;


namespace WebScrapeWidgetTests.CustomControls;

/// <summary>
/// Test fixture for Section custom control type.
/// </summary>
[TestOf(typeof(Section))]
[Author("Jakub Miodunka")]
public sealed class SectionTests
{
    #region Default values
    public const string DefaultSectionName = "Section name";
    #endregion

    #region Auxiliary methods
    public static XElement CreateSectionDefinition(string name, params XElement[] entriesDefinitions)
    {
        var sectionDefinition = new XElement("Section",
                new XAttribute("Name", name));

        entriesDefinitions.ToList().ForEach(sectionDefinition.Add);
        
        return sectionDefinition;
    }

    public static XElement CreateDefaultSectionDefinition()
    {
        XElement defaultEntryDefinition = EntryTests.CreateDefaultEntryDefinition();
        
        return CreateSectionDefinition(DefaultSectionName, defaultEntryDefinition);
    }
    
    public static Section CreateDefaultSection()
    {
        XElement defaultSectionDefinition = CreateDefaultSectionDefinition();
        Mock<IDataSourcesRepository> defaultDataSourcesRepositoryStub = DataSourcesRepositoryTests.CreateFakeDefaultDataSourcesRepository();

        return Section.FromXmlElement(defaultSectionDefinition, defaultDataSourcesRepositoryStub.Object);
    }
    #endregion

    #region Test parameters
    private static string[] s_validNames = [DefaultSectionName];
    private static string[] s_invalidNames = [string.Empty, " ", "\t", "\n"];
    #endregion

    #region Test cases
    /// <summary>
    /// Checks if section instantiation is impossible using null reference as section name.
    /// </summary>
    [Test]
    public void InstantiationImpossibleUsingNullReferenceAsName()
    {
        Mock<IDataSourcesRepository> dataSourcesRepositoryStub = DataSourcesRepositoryTests.CreateFakeDefaultDataSourcesRepository();

        TestDelegate sectionInstantiation = () => Section.FromXmlElement(null, dataSourcesRepositoryStub.Object);

        Assert.Throws<ArgumentNullException>(sectionInstantiation);
    }

    /// <summary>
    /// Checks if section instantiation is possible using provide string as section name.
    /// </summary>
    /// <param name="validName">
    /// Name, using which section instantiation shall be possible.
    /// </param>
    [Apartment(ApartmentState.STA)]
    [TestCaseSource(nameof(s_validNames))]
    public void InstantiationPossibleUsingValidName(string validName)
    {
        Mock<IDataSourcesRepository> dataSourcesRepositoryStub = DataSourcesRepositoryTests.CreateFakeDefaultDataSourcesRepository();
        XElement defaultEntryDefiniton = EntryTests.CreateDefaultEntryDefinition();
        
        var sectionDefinition = CreateSectionDefinition(validName, defaultEntryDefiniton);

        TestDelegate entryInstantiation = () => Section.FromXmlElement(sectionDefinition, dataSourcesRepositoryStub.Object);

        Assert.DoesNotThrow(entryInstantiation);
    }

    /// <summary>
    /// Checks if section instantiation is impossible using provided string as section name.
    /// </summary>
    /// <param name="invalidName">
    /// Name, using which entry instantiation shall not be possible.
    /// </param>
    [Apartment(ApartmentState.STA)]
    [TestCaseSource(nameof(s_invalidNames))]
    public void InstantiationImpossibleUsingInvalidLabel(string invalidName)
    {
        Mock<IDataSourcesRepository> dataSourcesRepositoryStub = DataSourcesRepositoryTests.CreateFakeDefaultDataSourcesRepository();
        XElement defaultEntryDefiniton = EntryTests.CreateDefaultEntryDefinition();
        
        var sectionDefinition = CreateSectionDefinition(invalidName, defaultEntryDefiniton);

        TestDelegate entryInstantiation = () => Section.FromXmlElement(sectionDefinition, dataSourcesRepositoryStub.Object);

        Assert.Throws<ArgumentException>(entryInstantiation);
    }

    /// <summary>
    /// Checks if section instantiation is impossible using null reference as data sources repository.
    /// </summary>
    [Test]
    public void InstantiationImpossibleUsingNullReferenceAsDataSourceRepository()
    {
        XElement defaultSectionDefinition = CreateDefaultSectionDefinition();

        TestDelegate entryInstantiation = () => Section.FromXmlElement(defaultSectionDefinition, null);

        Assert.Throws<ArgumentNullException>(entryInstantiation);
    }

    /// <summary>
    /// Checks if section header is a string instance.
    /// </summary>
    [Test]
    [Apartment(ApartmentState.STA)]
    public void HeaderIsString()
    {
        Section sectionUnderTest = CreateDefaultSection();

        Assert.IsInstanceOf<string>(sectionUnderTest.Header);
    }

    /// <summary>
    /// Checks if section header is set to specified section name.
    /// </summary>
    [Test]
    [Apartment(ApartmentState.STA)]
    public void IsHeaderEqualToSpecifiedName()
    {
        Section sectionUnderTest = CreateDefaultSection();

        Assert.That(sectionUnderTest.Header.ToString() == DefaultSectionName);
    }
    #endregion
}
