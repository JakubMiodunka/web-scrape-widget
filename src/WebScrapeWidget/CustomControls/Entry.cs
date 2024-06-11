using System.Xml.Linq;
using System.Windows.Controls;

using WebScrapeWidget.DataGathering.Repositories;


namespace WebScrapeWidget.CustomControls;

/// <summary>
/// Container for key-value pair, held within System.Windows.Controls.TextBlock type objects.
/// </summary>
public sealed class Entry
{
    #region Properties
    public readonly TextBlock Label;
    public readonly TextBlock Value;
    #endregion

    #region Class instantiation
    /// <summary>
    /// Creates a new entry corresponding to definition
    /// contained by provided XML element.
    /// </summary>
    /// <param name="entryElement">
    /// XML element, containing entry definition.
    /// </param>
    /// <returns>
    /// New entry corresponding to definition contained by provided XML element.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown, when at least one reference-type argument is a null reference.
    /// </exception>
    public static Entry FromXml(XElement entryElement)
    {
        if (entryElement is null)
        {
            string argumentName = nameof(entryElement);
            const string ErrorMessage = "Provided XML element is a null reference:";
            throw new ArgumentNullException(argumentName, ErrorMessage);
        }

        string label = entryElement
            .Attributes("Label")
            .Select(attribute => attribute.Value)
            .First();

        string dataSourceName = entryElement
            .Attributes("DataSourceName")
            .Select(attribute => attribute.Value)
            .First();

        return new Entry(label, dataSourceName);
    }

    /// <summary>
    /// Creates a new entry instance.
    /// </summary>
    /// <param name="label">
    /// String, which shall be used as label for value shown by the entry.
    /// </param>
    /// <param name="dataSourceName">
    /// Name of data source, from which value shown by the entry shall be gathered.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown, when at least one reference-type argument is a null reference.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown ,when value of at least one argument will be considered as invalid.
    /// </exception>
    private Entry(string label, string dataSourceName)
    {
        if (label is null)
        {
            string argumentName = nameof(label);
            const string ErrorMessage = "Provided entry label is a null reference:";
            throw new ArgumentNullException(argumentName, ErrorMessage);
        }

        if (label == string.Empty)
        {
            string argumentName = nameof(label);
            const string ErrorMessage = "Provided entry label is an empty string:";
            throw new ArgumentOutOfRangeException(argumentName, label, ErrorMessage);
        }

        if (dataSourceName is null)
        {
            string argumentName = nameof(dataSourceName);
            const string ErrorMessage = "Provided data source name is a null reference:";
            throw new ArgumentNullException(argumentName, ErrorMessage);
        }

        if (dataSourceName == string.Empty)
        {
            string argumentName = nameof(dataSourceName);
            const string ErrorMessage = "Provided data source name is an empty string:";
            throw new ArgumentOutOfRangeException(argumentName, dataSourceName, ErrorMessage);
        }

        Label = new TextBlock();
        Label.Text = label;

        Value = new TextBlock();
        Value.Text = DataSourcesRepository.Instance.GetDataFromSource(dataSourceName);
    }
    #endregion
}
