using HtmlAgilityPack;
using System.Text.RegularExpressions;


namespace WebScrapeWidget.Utilities;

/// <summary>
/// Set of static methods helpful during with interactions with HTML documents.
/// </summary>
public static class HtmlUtilities
{
    /// <summary>
    /// Extracts from provided HTML document a node, placed under provided XPath.
    /// </summary>
    /// <param name="htmlDocument">
    /// HTML document, from which node shall be extracted.
    /// </param>
    /// <param name="xPath">
    /// XPath corresponding to node, which shall be extracted.
    /// </param>
    /// <returns>
    /// HTML node extracted from HTML document.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown, when at least one reference-type argument is a null reference.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown, when at least one argument will be considered as invalid.
    /// </exception>
    public static HtmlNode FilterNode(HtmlDocument htmlDocument, string xPath)
    {
        if (htmlDocument is null)
        {
            string argumentName = nameof(htmlDocument);
            const string ErrorMesage = "Provided HTML document is a null reference.";
            throw new ArgumentNullException(argumentName, ErrorMesage);
        }

        if (string.IsNullOrWhiteSpace(xPath))
        {
            string argumentName = nameof(xPath);
            string errorMesage = $"Provided XPath is invalid: {xPath}";
            throw new ArgumentException(errorMesage, argumentName);
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

    /// <summary>
    /// Filters text content of provided HTML node using specified regular expression. 
    /// </summary>
    /// <param name="node">
    /// HTML node, which text content shall be filtered,
    /// </param>
    /// <param name="filter">
    /// Regular expression, which shall be used to filter
    /// text content of provided HTML node.
    /// </param>
    /// <returns>
    /// First phrase from provided HTML node text content,
    /// which matches provided regular expression.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown, when at least one reference-type argument is a null reference.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown, when value of at least one argument will be considered as invalid.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown, when at least one argument will be considered as invalid.
    /// </exception>
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
