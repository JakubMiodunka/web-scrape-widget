using System.Collections.Concurrent;

namespace WebScrapeWidget.DataGathering.Models;


/// <summary>
/// Base class for all more specific types of data sources.
/// It manages the data shared between all instances
/// of derivative classes and secures their integrity.
/// </summary>
public abstract class DataSource
{
    #region Static properties
    private static ConcurrentBag<string> s_occupiedINames = new ConcurrentBag<string>();
    #endregion

    #region Properties
    public string Name { get; init; }
    #endregion

    #region Class instantiation
    /// <summary>
    /// 
    /// </summary>
    /// <param name="name">
    /// Unique name of represented data source.
    /// </param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown, when provided data source name is already occupied. 
    /// </exception>
    protected DataSource(string name)
    {
        if (name is null)
        {
            string argumentName = nameof(name);
            const string ErrorMessage = "Provided data source name is a null reference:";
            throw new ArgumentNullException(argumentName, ErrorMessage);
        }

        if (name == string.Empty)
        {
            string argumentName = nameof(name);
            const string ErrorMessage = "Provided data source name is an empty string:";
            throw new ArgumentOutOfRangeException(argumentName, name, ErrorMessage);
        }

        if (s_occupiedINames.Contains(name))
        {
            string argumentName = nameof(name);
            string errorMessage = $"Provided data source name already in use: {name}";
            throw new ArgumentOutOfRangeException(argumentName, name, errorMessage);
        }

        s_occupiedINames.Add(name);
        Name = name;
    }
    #endregion
}
