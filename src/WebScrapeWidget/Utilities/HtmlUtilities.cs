using System.Text.RegularExpressions;

using HtmlAgilityPack;

namespace WebScrapeWidget.Utilities;


public static class HtmlUtilities
{
    public static HtmlNode FilterNode(HtmlDocument htmlDocument, string xPath)
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
    public static string FilterNodeContent(HtmlNode node, Regex filter)
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
}
