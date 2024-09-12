using System.Windows.Controls;
using System.Xml.Linq;
using WebScrapeWidget.DataGathering.Interfaces;


namespace WebScrapeWidget.CustomControls;

/// <summary>
/// Custom WPF control build around System.Windows.Controls.GroupBox.
/// Its content is divided into entries.
/// </summary>
public sealed class Section : GroupBox
{
    #region Class instantiation
    /// <summary>
    /// Creates a new section corresponding to provided XML element.
    /// </summary>
    /// <param name="xmlElement">
    /// XML element, containing section definition.
    /// </param>
    /// <param name="dataSourcesRepository">
    /// Data sources repository, which shall be used to obtain displayed data.
    /// </param>
    /// <returns>
    /// New section corresponding to definition contained by provided XML element.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown, when at least one reference-type argument is a null reference.
    /// </exception>
    public static Section FromXmlElement(XElement xmlElement, IDataSourcesRepository dataSourcesRepository)
    {
        if (xmlElement is null)
        {
            string argumentName = nameof(xmlElement);
            const string ErrorMessage = "Provided XML element is a null reference:";
            throw new ArgumentNullException(argumentName, ErrorMessage);
        }

        string name = xmlElement
            .Attributes("Name")
            .Select(attribute => attribute.Value)
            .First();

        Entry[] entries = xmlElement
            .Elements("Entry")
            .Select(entryDefinitionElement => Entry.FromXmlElement(entryDefinitionElement, dataSourcesRepository))
            .ToArray();

        return new Section(name, entries);
    }

    /// <summary>
    /// Creates a new section instance.
    /// </summary>
    /// <param name="name">
    /// Name, which shall be assigned to created section.
    /// </param>
    /// <param name="entries">
    /// Collection of entries, which shall be contained by created section.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown, when at least one reference-type argument is a null reference.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown, when at least one argument will be considered as invalid.
    /// </exception>
    private Section(string name, IEnumerable<Entry> entries)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            string argumentName = nameof(name);
            string errorMessage = $"Provided name is invalid: {name}";
            throw new ArgumentException(argumentName, errorMessage);
        }

        if (entries is null)
        {
            string argumentName = nameof(entries);
            const string ErrorMessage = "Provided entries collection is a null reference:";
            throw new ArgumentNullException(argumentName, ErrorMessage);
        }

        if (entries.Contains(null))
        {
            string argumentName = nameof(entries);
            const string ErrorMessage = "Provided entries collection contains a null reference:";
            throw new ArgumentException(argumentName, ErrorMessage);
        }

        Header = name;

        var content = new StackPanel();
        entries.ToList().ForEach(entry => content.Children.Add(entry));

        AddChild(content);
    }
    #endregion
}
