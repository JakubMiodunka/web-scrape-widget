﻿using HtmlAgilityPack;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using WebScrapeWidget.DataGathering.Interfaces;
using WebScrapeWidget.Utilities;


namespace WebScrapeWidget.DataGathering.DataSources;

/// <summary>
/// Representation of website element, which can be scraped to gather necessary data.
/// </summary>
public sealed class WebsiteElement : DataSource, IDataSource
{
    #region Constants
    const string WebsiteElementDefinitionSchema = @"..\..\..\..\..\res\schemas\website_element_schema.xsd";
    #endregion

    #region Static properties
    private static HtmlWeb? s_httpClient;
    #endregion

    #region Properties
    public readonly Uri WebsiteUrl;
    public readonly string HtmlNodeXPath;
    public readonly Regex NodeContentFilter;
    #endregion

    #region Identification
    /// <summary>
    /// Checks if provided XML document contains definition of a website element.
    /// </summary>
    /// <param name="xmlDocument">
    /// XML document, which shall be checked.
    /// </param>
    /// <returns>
    /// True or false, depending on check result.
    /// </returns>
    public static bool IsWebsiteElementDefinition(XDocument xmlDocument)
    {
        return XmlUtilities.IsMatchingToSchema(xmlDocument, WebsiteElementDefinitionSchema);
    }
    #endregion

    #region Class instantiation
    /// <summary>
    /// Creates a new representational of website element basing on provided XML document.
    /// </summary>
    /// <param name="xmlDocument">
    /// XML document, containing definition of a website element.
    /// </param>
    /// <returns>
    /// New website element representation created basing on provided XML document.
    /// </returns>
    public static WebsiteElement FromXmlDocument(XDocument xmlDocument)
    {
        XmlUtilities.ValidateXmlDocument(xmlDocument, WebsiteElementDefinitionSchema);

        XElement dataSourceElement = xmlDocument
            .Elements("DataSource")
            .First();

        return FromXmlElement(dataSourceElement);
    }

    /// <summary>
    /// Creates a new representational of website element basing on provided XML element.
    /// </summary>
    /// <param name="xmlElement">
    /// XML element, containing definition of a website element.
    /// </param>
    /// <returns>
    /// New website element representation created basing on provided XML element.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown, when at least one reference-type argument is a null reference.
    /// </exception>
    private static WebsiteElement FromXmlElement(XElement xmlElement)
    {
        if (xmlElement is null)
        {
            string argumentName = nameof(xmlElement);
            const string ErrorMessage = "Provided XML element is a null reference:";
            throw new ArgumentNullException(argumentName, ErrorMessage);
        }

        string name = xmlElement
            .Attributes("Name")
            .Select(attribute => attribute.Value)
            .First();

        string description = xmlElement
            .Elements("Description")
            .First()
            .Value;

        string dataUnit = xmlElement
            .Attributes("DataUnit")
            .Select(attribute => attribute.Value)
            .First();

        TimeSpan refreshRate = xmlElement
            .Attributes("RefreshRate")
            .Select(attribute => attribute.Value)
            .Select(XmlConvert.ToTimeSpan)
            .First();

        XElement websiteElement = xmlElement
            .Elements("WebsiteElement")
            .First();

        Uri websiteUrl = websiteElement
            .Elements("Website")
            .SelectMany(element => element.Attributes("Url"))
            .Select(attribute => attribute.Value)
            .Select(value => new Uri(value))
            .First();

        string htmlNodeXPath = websiteElement
            .Elements("HtmlNode")
            .SelectMany(element => element.Attributes("XPath"))
            .Select(attribute => attribute.Value)
            .First();

        Regex nodeContentFilter = websiteElement
            .Elements("NodeContentFilter")
            .SelectMany(element => element.Attributes("Regex"))
            .Select(attribute => attribute.Value)
            .Select(value => new Regex(value))
            .First();

        return new WebsiteElement(name, description, dataUnit, refreshRate, websiteUrl, htmlNodeXPath, nodeContentFilter);
    }

    /// <summary>
    /// Creates a new representation of a website element.
    /// </summary>
    /// <param name="name">
    /// Unique name of represented data source.
    /// </param>
    /// <param name="description">
    /// Description of created data source.
    /// </param>
    /// <param name="dataUnit">
    /// Unit, in which data gathered from source is presented.
    /// </param>
    /// <param name="refreshRate">
    /// Refresh rate of data gathered from source expressed in time period.
    /// </param>
    /// <param name="websiteUrl">
    /// HTTPS-based URL of a website, which element shall be represented.
    /// </param>
    /// <param name="htmlNodeXPath">
    /// XPath to website element, which shall be represented.
    /// </param>
    /// <param name="nodeContentFilter">
    /// Regular expression, which shall be used to filter content of represented
    /// element during scraping process.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown, when at least one reference-type argument is a null reference.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown, when value of at least one argument will be considered as invalid.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown, when at least one argument will be considered as invalid.
    /// </exception>
    private WebsiteElement(string name, string description, string dataUnit, TimeSpan refreshRate, Uri websiteUrl, string htmlNodeXPath, Regex nodeContentFilter)
        : base(name, description, dataUnit, refreshRate)
    {
        if (websiteUrl is null)
        {
            string argumentName = nameof(websiteUrl);
            const string ErrorMessage = "Provided website URL is a null reference:";
            throw new ArgumentNullException(argumentName, ErrorMessage);
        }

        if (websiteUrl.Scheme != Uri.UriSchemeHttps)
        {
            string argumentName = nameof(websiteUrl);
            string errorMessage = $"Provided website URL does not refer to HTTPS connection: {websiteUrl}";
            throw new ArgumentOutOfRangeException(argumentName, websiteUrl, errorMessage);
        }

        if (string.IsNullOrWhiteSpace(htmlNodeXPath))
        {
            string argumentName = nameof(htmlNodeXPath);
            string errorMessage = $"Provided HTML node XPath is invalid: {htmlNodeXPath}";
            throw new ArgumentException(argumentName, errorMessage);
        }

        if (nodeContentFilter is null)
        {
            string argumentName = nameof(nodeContentFilter);
            const string ErrorMessage = "Provided node content filter is a null reference:";
            throw new ArgumentNullException(argumentName, ErrorMessage);
        }

        string filterPattern = nodeContentFilter.ToString();

        if (string.IsNullOrWhiteSpace(filterPattern))
        {
            string argumentName = nameof(nodeContentFilter);
            string errorMessage = $"Provided node content filter does not contain any pattern: {filterPattern}";
            throw new ArgumentOutOfRangeException(argumentName, filterPattern, errorMessage);
        }

        if (s_httpClient is null)
        {
            s_httpClient = new HtmlWeb();
        }

        WebsiteUrl = websiteUrl;
        HtmlNodeXPath = htmlNodeXPath;
        NodeContentFilter = nodeContentFilter;
    }
    #endregion

    #region Scraping
    /// <summary>
    /// Performs scraping of website element and notifies
    /// subscribers with new value of gathered data.
    /// </summary>
    /// <returns>
    /// Task related to scraping.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown, when HTTP client is not initialized.
    /// </exception>
    public override async Task GatherData()
    {
        if (s_httpClient is null)
        {
            const string ErrorMessage = "HTTP client not initialized:";
            throw new InvalidOperationException(ErrorMessage);
        }

        string cononicalUrl = WebsiteUrl.ToString();
        HtmlDocument scrapedWebsite = await s_httpClient.LoadFromWebAsync(cononicalUrl);

        HtmlNode scrapedNode = HtmlUtilities.FilterNode(scrapedWebsite, HtmlNodeXPath);
        string scrapedContent = HtmlUtilities.FilterNodeContent(scrapedNode, NodeContentFilter);

        _gatheredData = scrapedContent;
        LastRefreshTimestamp = DateTime.Now;

        NotifySubscribers();
    }
    #endregion
}
