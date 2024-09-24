namespace WebScrapeWidget.Utilities;

/// <summary>
/// Set of utilities related to string formatting.
/// </summary>
public static class StringUtilities
{
    /// <summary>
    /// Normalizes provided multi line string.
    /// </summary>
    /// <remarks>
    /// Normalization process consists of removal empty lines,
    /// lines which consists of whitespace characters only
    /// and trimming the lines both at their begging and end.
    /// </remarks>
    /// <param name="input">
    /// String, which shall be normalized.
    /// </param>
    /// <returns>
    /// Normalized version of provided input string.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown, when at least one reference-type argument is a null reference.
    /// </exception>
    public static string NormalizeMultiLineString(string input)
    {
        if (input is null)
        {
            string argumentName = nameof(input);
            const string ErrorMessage = "Provided input string is a null reference:";
            throw new ArgumentNullException(argumentName, ErrorMessage);
        }

        string[] normalizedInputLines = input.Split('\n')
            .Where(line => !string.IsNullOrWhiteSpace(line))
            .Select(line => line.Trim())
            .ToArray();

        return string.Join('\n', normalizedInputLines);
    }
}
