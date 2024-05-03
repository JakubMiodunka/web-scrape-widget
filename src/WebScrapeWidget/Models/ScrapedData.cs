namespace WebScrapeWidget.Models;

//TODO: Doc-strings.
public sealed class ScrapedData
{
    public readonly WebDataSource Source;
    public readonly Exception[] ScrapingExceptions;

    public string Value
    {
        get
        {
            if (ScrapingExceptions.Any())
            {
                string errorMesage = $"Scraped value is not available due to exceptions thrown during scraping: {ScrapingExceptions.Count()}";
                throw new InvalidOperationException(errorMesage);
            }

            if (_value is null)
            {
                string errorMesage = $"Despite no scraping exceptions were specified, scraped value is a null reference:";
                throw new InvalidOperationException(errorMesage);
            }

            return _value;
        }
    }

    private readonly string? _value;

    public ScrapedData(WebDataSource source, string? value, IEnumerable<Exception> scrapingExceptions)
    {
        if (source is null)
        {
            const string ErrorMessage = "Provided data source is a null reference:";
            throw new ArgumentNullException(nameof(source), ErrorMessage);
        }

        if (scrapingExceptions is null)
        {
            const string ErrorMessage = "Provided scraping exceptions collection is a null reference:";
            throw new ArgumentNullException(nameof(scrapingExceptions), ErrorMessage);
        }

        if (scrapingExceptions.Contains(null))
        {
            const string ErrorMessage = $"Provided scraping exceptions collection contains a null reference:";
            throw new ArgumentNullException(nameof(scrapingExceptions), ErrorMessage);
        }

        Source = source;
        ScrapingExceptions = scrapingExceptions.ToArray();
        _value = value;
    }
}
