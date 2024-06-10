using System.Xml.Linq;
using System.Windows.Controls;


namespace WebScrapeWidget.CustomControls;

/// <summary>
/// Custom WPF control build around System.Windows.Controls.TabItem.
/// Its content is divided into sections.
/// </summary>
public sealed class Tab : TabItem
{
    #region Properties
    private readonly Grid _grid;
    #endregion

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
    public static Tab FromXml(XElement tabElement)
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
    /// Collection of sections, which shall be placed on tab grid.
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

        _grid = new Grid();
        AddChild(_grid);

        sections.ToList().ForEach(AddSection);
    }

    /// <summary>
    /// Adds provided section to the tab grid.
    /// </summary>
    /// <param name="section">
    /// Section, which shall be added to the grid of the tab.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown, when at least one reference-type argument is a null reference.
    /// </exception>
    public void AddSection(GroupBox section)
    {
        if (section is null)
        {
            string argumentName = nameof(section);
            const string ErrorMessage = "Provided section is a null reference:";
            throw new ArgumentNullException(argumentName, ErrorMessage);
        }

        var newRowDefinition = new RowDefinition();
        _grid.RowDefinitions.Add(newRowDefinition);

        int newRowIndex = _grid.RowDefinitions.Count() - 1;
        Grid.SetRow(section, newRowIndex);

        _grid.Children.Add(section);
    }
    #endregion
}
