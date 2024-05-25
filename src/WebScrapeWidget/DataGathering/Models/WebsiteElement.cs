using System.Text.RegularExpressions;
using WebScrapeWidget.Utilities;

using HtmlAgilityPack;
using WebScrapeWidget.DataGathering.Interfaces;

namespace WebScrapeWidget.DataGathering.Models;

/// <summary>
/// Representation of website element, which can be scraped to gather necessary data.
/// </summary>
public sealed class WebsiteElement : WebDataSource, IDataSource
{
    #region Static properties
    private static HtmlWeb? s_httpClient;
    #endregion

    #region Properties
    public readonly Uri WebsiteUrl;
    public readonly string HtmlNodeXPath;
    public readonly Regex NodeContentFilter;

    public bool IsScraped
    {
        get
        {
            return _scrapedData is null;
        }
    }
    public string ScrapedData
    {
        get
        {
            if (_scrapedData is null)
            {
                const string ErrorMessage = "Attempting to access scraped data before the scraping.";
                throw new InvalidOperationException(ErrorMessage);
            }

            return _scrapedData;
        }
    }

    private string? _scrapedData;
    #endregion

    #region Class instantiation
    /// <summary>
    /// Creates a new representation of a website element.
    /// </summary>
    /// <param name="identifier">
    /// Web data source unique identifier.
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
    public WebsiteElement(uint identifier, Uri websiteUrl, string htmlNodeXPath, Regex nodeContentFilter)
        : base(identifier)
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

        if (htmlNodeXPath is null)
        {
            string argumentName = nameof(htmlNodeXPath);
            const string ErrorMessage = "Provided HTML node XPath i a null reference:";
            throw new ArgumentNullException(argumentName, ErrorMessage);
        }

        if (htmlNodeXPath == string.Empty)
        {
            string argumentName = nameof(htmlNodeXPath);
            const string ErrorMessage = "Provided HTML node XPath i an empty string:";
            throw new ArgumentOutOfRangeException(argumentName, htmlNodeXPath, ErrorMessage);
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
    /// Performs scraping of website element.
    /// </summary>
    /// <returns>
    /// Scraped website content.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown, when HTTP client is not initialized.
    /// </exception>
    public async Task Scrape()
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

        _scrapedData = scrapedContent;
    }
    #endregion

    #region Implementation of IDataSource interface.
    public bool WasDataGathered
    {
        get
        {
            return IsScraped;
        }
    }
    public string GatheredData
    {
        get
        {
            return ScrapedData;
        }
    }

    /// <summary>
    /// Implementation of IDataSource interface.
    /// </summary>
    /// <returns>
    /// Returns data gathered from web source.
    /// </returns>
    public async Task GatherData()
    {
        await Scrape();
    }
    #endregion
}
