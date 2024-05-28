using System.Collections.Concurrent;

namespace WebScrapeWidget.DataGathering.Models;


/// <summary>
/// Base class for all more specific types of web data sources.
/// It manages the data shared between all instances
/// of derivative classes and secures their integrity.
/// </summary>
public abstract class WebDataSource
{
    #region Static properties
    private static ConcurrentBag<uint> s_occupiedIdentifiers = new ConcurrentBag<uint>();
    #endregion

    #region Properties
    public uint Identifier { get; init; }
    #endregion

    #region Class instantiation
    /// <summary>
    /// 
    /// </summary>
    /// <param name="identifier">
    /// Unique identifier of represented web data source.
    /// </param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown, when provided web data source identifier is already occupied. 
    /// </exception>
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
