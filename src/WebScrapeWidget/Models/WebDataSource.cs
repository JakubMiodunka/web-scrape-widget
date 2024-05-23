using System.Collections.Concurrent;

namespace WebScrapeWidget.Models;

public abstract class WebDataSource
{
    #region Static properties
    private static ConcurrentBag<uint> s_occupiedIdentifiers = new ConcurrentBag<uint>();
    #endregion

    #region Properties
    public readonly uint Identifier;
    #endregion

    #region Class instantiation
    protected WebDataSource(uint identifier)
    {
        if (s_occupiedIdentifiers.Contains(identifier))
        {
            string argumentName = nameof(identifier);
            string errorMessage = $"Provided identifier already in use: {identifier}";
            throw new ArgumentOutOfRangeException(argumentName, identifier, errorMessage);
        }

        s_occupiedIdentifiers.Add(identifier);
        Identifier = identifier;
    }
    #endregion
}
