using System.Globalization;


namespace WebScrapeWidget.Utilities;

/// <summary>
/// Utility, which swaps the culture used by the thread,
/// executes a block of code and reverts the culture change.
/// Designed to use with 'using' statement.
/// </summary>
/// <example>
/// <code>
/// CultureInfo.CurrentCulture = new CultureInfo("en-GB")
/// 
/// // Here 'en-GB' culture is being used.
/// 
/// using(var cultureContext = new CultureContext("pl-PL"))
/// {
///     // Here 'pl-PL' culture is being used.
/// }
/// 
/// // Here 'en-GB' culture is being used.
/// </code>
/// </example>

public sealed class CultureContext : IDisposable
{
    #region Properties
    public readonly CultureInfo OriginalCulture;
    public readonly CultureInfo SwappedCulture;
    #endregion

    #region Class instantiation
    /// <summary>
    /// Creates a new culture context and swaps the culture used by the thread.
    /// </summary>
    /// <param name="swapCultureName">
    /// Name of culture, to which thread culture shall be swapped.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown, when at least one reference-type argument is a null reference.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown, when value of at least one argument will be considered as invalid.
    /// </exception>
    public static CultureContext FromCultureName(string swapCultureName)
    {
        if (swapCultureName is null)
        {
            string argumentName = nameof(swapCultureName);
            const string ErrorMessage = "Provided culture name is a null reference:";
            throw new ArgumentNullException(argumentName, ErrorMessage);
        }

        if (swapCultureName == string.Empty)
        {
            string argumentName = nameof(swapCultureName);
            const string ErrorMessage = "Provided culture name is an empty string:";
            throw new ArgumentOutOfRangeException(argumentName, swapCultureName, ErrorMessage);
        }

        CultureInfo swapCulture;

        try
        {
            swapCulture = new CultureInfo(swapCultureName);
        }
        catch (CultureNotFoundException)
        {
            string argumentName = nameof(swapCultureName);
            string errorMessage = $"Provided culture name is invalid: {swapCultureName}";
            throw new ArgumentOutOfRangeException(argumentName, swapCultureName, errorMessage);
        }

        return new CultureContext(swapCulture);
    }

    /// <summary>
    /// Creates a new culture context and swaps the culture used by the thread.
    /// </summary>
    /// <param name="swapCulture">
    /// Culture, to which thread culture shall be swapped.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown, when at least one reference-type argument is a null reference.
    /// </exception>
    public CultureContext(CultureInfo swapCulture)
    {
        if (swapCulture is null)
        {
            string argumentName = nameof(swapCulture);
            const string ErrorMessage = "Provided culture is a null reference:";
            throw new ArgumentNullException(argumentName, ErrorMessage);
        }

        OriginalCulture = CultureInfo.CurrentCulture;
        SwappedCulture = swapCulture;

        CultureInfo.CurrentCulture = SwappedCulture;
    }
    #endregion

    #region Interactions
    /// <summary>
    /// Restores thread culture back to original one.
    /// </summary>
    public void Dispose()
    {
        CultureInfo.CurrentCulture = OriginalCulture;
    }
    #endregion
}
