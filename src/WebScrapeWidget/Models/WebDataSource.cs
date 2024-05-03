using System.Text.RegularExpressions;
using System.Xml.Linq;

using Utilities;


namespace WebScrapeWidget.Models;

//TODO: Doc-strings.
public sealed class WebDataSource
{
    private const string XmlSchema = @"..\..\..\..\..\res\schemas\web_data_source_schema.xsd";
    
    private static List<uint> s_occupiedIdentifiers = new List<uint>();
    
    public readonly uint Identifier;
    public readonly Uri WebsiteUrl;
    public readonly string HtmlNodeXPath;
    public readonly Regex NodeContentFilter;

    public WebDataSource FromXmlFile(string filePath)
    {
        FileSystemUtilities.ValidateXmlDocument(filePath, XmlSchema);
        
        XElement root = XDocument.Load(filePath)
            .Elements("WebDataSource")
            .First();

        uint identifier = root
            .Attributes("Identifier")
            .Select(attribute => attribute.Value)
            .Select(uint.Parse)
            .First();

        Uri websiteUrl = root
            .Elements("Website")
            .SelectMany(element => element.Attributes("Url"))
            .Select(attribute => attribute.Value)
            .Select(url => new Uri(url))
            .First();

        string htmlNodeXPath = root
            .Elements("HtmlNode")
            .SelectMany(element => element.Attributes("XPath"))
            .Select(attribute => attribute.Value)
            .First();

        Regex nodeContentFilter = root
            .Elements("NodeContentFilter")
            .SelectMany(element => element.Attributes("Regex"))
            .Select(attribute => attribute.Value)
            .Select(pattern => new Regex(pattern))
            .First();

        return new WebDataSource(identifier, websiteUrl, htmlNodeXPath, nodeContentFilter);
    }

    public WebDataSource(uint identifier, Uri websiteUrl, string htmlNodeXPath, Regex nodeContentFilter)
    {
        if (s_occupiedIdentifiers.Contains(identifier))
        {
            string errorMessage = $"Provided identifier already in use: {identifier}";
            throw new ArgumentOutOfRangeException(nameof(identifier), identifier, errorMessage);
        }
        
        if (websiteUrl is null)
        {
            const string ErrorMessage = "Provided website URL is a null reference:";
            throw new ArgumentNullException(nameof(websiteUrl), ErrorMessage);
        }

        if (websiteUrl.Scheme != Uri.UriSchemeHttps)
        {
            string canonicalUrl = websiteUrl.ToString();
            
            string errorMessage = $"Provided website URL does not refer to HTTPS connection: {canonicalUrl}";
            throw new ArgumentOutOfRangeException(nameof(websiteUrl), canonicalUrl, errorMessage);
        }

        if (htmlNodeXPath is null)
        {
            const string ErrorMessage = "Provided HTML node XPath i a null reference:";
            throw new ArgumentNullException(nameof(htmlNodeXPath), ErrorMessage);
        }

        if (htmlNodeXPath == string.Empty)
        {
            const string ErrorMessage = "Provided HTML node XPath i an empty string:";
            throw new ArgumentOutOfRangeException(nameof(htmlNodeXPath), htmlNodeXPath, ErrorMessage);
        }

        if (nodeContentFilter is null)
        {
            const string ErrorMessage = "Provided node content filter is a null reference:";
            throw new ArgumentNullException(nameof(nodeContentFilter), ErrorMessage);
        }

        string filterPattern = nodeContentFilter.ToString();

        if (string.IsNullOrWhiteSpace(filterPattern))
        {
            string errorMessage = $"Provided node content filter does not contain any pattern: {filterPattern}";
            throw new ArgumentOutOfRangeException(nameof(htmlNodeXPath), filterPattern, errorMessage);
        }

        Identifier = identifier;
        WebsiteUrl = websiteUrl;
        HtmlNodeXPath = htmlNodeXPath;
        NodeContentFilter = nodeContentFilter;

        s_occupiedIdentifiers.Add(Identifier);
    }
}
