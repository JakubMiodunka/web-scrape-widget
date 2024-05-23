using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Xml.Schema;
using HtmlAgilityPack;


namespace WebScrapeWidget.Models;

public sealed class WebsiteElement : WebDataSource
{
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

    #region HTML utilities
    private static HtmlNode FilterNode(HtmlDocument htmlDocument, string xPath)
    {
        if (htmlDocument is null)
        {
            string argumentName = nameof(htmlDocument);
            const string ErrorMesage = "Provided HTML document is a null reference.";
            throw new ArgumentNullException(argumentName, ErrorMesage);
        }

        if (xPath is null)
        {
            string argumentName = nameof(xPath);
            const string ErrorMesage = "Provided XPath is a null reference.";
            throw new ArgumentNullException(argumentName, ErrorMesage);
        }

        if (xPath == string.Empty)
        {
            string argumentName = nameof(xPath);
            const string ErrorMesage = "Provided XPath is an empty string.";
            throw new ArgumentOutOfRangeException(argumentName, xPath, ErrorMesage);
        }

        HtmlNode[] matchingNodes = htmlDocument
            .DocumentNode
            .SelectNodes(xPath)
            .ToArray();

        if (matchingNodes.Any())
        {
            return matchingNodes.First();
        }
        else
        {
            string argumentName = nameof(htmlDocument);
            string errorMessage = $"Provided HTML document does not contain any nodes matching specified XPath: {xPath}";
            throw new ArgumentException(errorMessage, argumentName);
        }
    }

    private static string FilterNodeContent(HtmlNode node, Regex filter)
    {
        if (node is null)
        {
            string argumentName = nameof(node);
            const string ErrorMesage = "Provided HTML node is a null reference.";
            throw new ArgumentNullException(argumentName, ErrorMesage);
        }

        if (filter is null)
        {
            string argumentName = nameof(filter);
            const string ErrorMesage = "Provided filter is a null reference.";
            throw new ArgumentNullException(argumentName, ErrorMesage);
        }

        string filterPattern = filter.ToString();

        if (string.IsNullOrWhiteSpace(filterPattern))
        {
            string argumentName = nameof(filter);
            string errorMessage = $"Provided filter does not contain any pattern: {filterPattern}";
            throw new ArgumentOutOfRangeException(argumentName, filter, errorMessage);
        }

        string nodeContent = node.InnerText;

        string[] matches = filter
            .Matches(nodeContent)
            .Select(match => match.Value)
            .ToArray();

        if (matches.Any())
        {
            return matches.First();
        }
        else
        {
            string argumentName = nameof(filter);
            string errorMessage = $"Provided HTML node does not contain any text matching specified filter: {filterPattern}";
            throw new ArgumentException(errorMessage, argumentName);
        }
    }
    #endregion

    #region Class instantiation
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

        WebsiteUrl = websiteUrl;
        HtmlNodeXPath = htmlNodeXPath;
        NodeContentFilter = nodeContentFilter;
    }
    #endregion

    #region Scraping
    public async Task Scrape(HtmlWeb httpClient)
    {
        HtmlDocument scrapedWebsite = await httpClient.LoadFromWebAsync(WebsiteUrl.ToString());

        HtmlNode filteredNode = FilterNode(scrapedWebsite, HtmlNodeXPath);
        string filteredNodeContent = FilterNodeContent(filteredNode, NodeContentFilter);

        _scrapedData = filteredNodeContent;
    }
    #endregion
}
