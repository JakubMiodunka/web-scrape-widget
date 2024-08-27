using System.Windows.Controls;
using System.Xml.Linq;


namespace WebScrapeWidget.CustomControls;

/// <summary>
/// Custom WPF control build around System.Windows.Controls.GroupBox.
/// Its content is divided into entries.
/// </summary>
internal sealed class Section : GroupBox
{
    #region Class instantiation
    /// <summary>
    /// Creates a new section corresponding to definition
    /// contained by provided XML element.
    /// </summary>
    /// <param name="sectionElement">
    /// XML element, containing section definition.
    /// </param>
    /// <returns>
    /// New section corresponding to definition contained by provided XML element.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown, when at least one reference-type argument is a null reference.
    /// </exception>
    internal static Section FromXml(XElement sectionElement)
    {
        if (sectionElement is null)
        {
            string argumentName = nameof(sectionElement);
            const string ErrorMessage = "Provided XML element is a null reference:";
            throw new ArgumentNullException(argumentName, ErrorMessage);
        }

        string name = sectionElement
            .Attributes("Name")
            .Select(attribute => attribute.Value)
            .First();

        Entry[] entries = sectionElement
            .Elements("Entry")
            .Select(Entry.FromXml)
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
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown ,when value of at least one argument will be considered as invalid.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown, when at least one argument will be considered as invalid.
    /// </exception>
    private Section(string name, IEnumerable<Entry> entries)
    {
        if (name is null)
        {
            string argumentName = nameof(name);
            const string ErrorMessage = "Provided section name is a null reference:";
            throw new ArgumentNullException(argumentName, ErrorMessage);
        }

        if (name == string.Empty)
        {
            string argumentName = nameof(name);
            const string ErrorMessage = "Provided section name is an empty string:";
            throw new ArgumentOutOfRangeException(argumentName, name, ErrorMessage);
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
