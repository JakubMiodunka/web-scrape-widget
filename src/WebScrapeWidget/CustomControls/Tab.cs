using System.Xml.Linq;

using System.Windows.Controls;


namespace WebScrapeWidget.CustomControls;

/// <summary>
/// Custom WPF control build around System.Windows.Controls.TabItem.
/// Its content is divided into sections.
/// </summary>
internal sealed class Tab : TabItem
{
    #region Class instantiation
    /// <summary>
    /// Creates a new tab corresponding to definition
    /// contained by provided XML element.
    /// </summary>
    /// <param name="tabElement">
    /// XML element, containing tab definition.
    /// </param>
    /// <returns>
    /// New tab corresponding to definition contained by provided XML element.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown, when at least one reference-type argument is a null reference.
    /// </exception>
    internal static Tab FromXml(XElement tabElement)
    {
        if (tabElement is null)
        {
            string argumentName = nameof(tabElement);
            const string ErrorMessage = "Provided XML element is a null reference:";
            throw new ArgumentNullException(argumentName, ErrorMessage);
        }

        string name = tabElement
            .Attributes("Name")
            .Select(attribute => attribute.Value)
            .First();

        Section[] sections = tabElement
            .Elements("Section")
            .Select(Section.FromXml)
            .ToArray();

        return new Tab(name, sections);
    }

    /// <summary>
    /// Creates a new instance of the tab.
    /// </summary>
    /// <param name="name">
    /// Tab name.
    /// </param>
    /// <param name="sections">
    /// Collection of sections, which shall be contained by created tab.
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
    private Tab(string name, IEnumerable<Section> sections)
    {
        if (name is null)
        {
            string argumentName = nameof(name);
            const string ErrorMessage = "Provided tab name is a null reference:";
            throw new ArgumentNullException(argumentName, ErrorMessage);
        }

        if (name == string.Empty)
        {
            string argumentName = nameof(name);
            const string ErrorMessage = "Provided tab name is an empty string:";
            throw new ArgumentOutOfRangeException(argumentName, name, ErrorMessage);
        }

        if (sections is null)
        {
            string argumentName = nameof(sections);
            const string ErrorMessage = "Provided sections collection is a null reference:";
            throw new ArgumentNullException(argumentName, ErrorMessage);
        }

        if (sections.Contains(null))
        {
            string argumentName = nameof(sections);
            const string ErrorMessage = "Provided sections collection contains a null reference:";
            throw new ArgumentException(argumentName, ErrorMessage);
        }

        Header = name;

        var content = new StackPanel();
        sections.ToList().ForEach(section => content.Children.Add(section));

        AddChild(content);
    }
    #endregion
}
