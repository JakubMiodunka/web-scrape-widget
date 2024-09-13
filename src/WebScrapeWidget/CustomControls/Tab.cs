using System.Xml.Linq;

using System.Windows.Controls;
using WebScrapeWidget.DataGathering.Interfaces;


namespace WebScrapeWidget.CustomControls;

/// <summary>
/// Custom WPF control build around System.Windows.Controls.TabItem.
/// Its content is divided into sections.
/// </summary>
public sealed class Tab : TabItem
{
    #region Class instantiation
    /// <summary>
    /// Creates a new tab corresponding to provided XML element.
    /// </summary>
    /// <param name="xmlElement">
    /// XML element, containing tab definition.
    /// </param>
    /// <param name="dataSourcesRepository">
    /// Data sources repository, which shall be used to obtain displayed data.
    /// </param>
    /// <returns>
    /// New tab corresponding to definition contained by provided XML element.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown, when at least one reference-type argument is a null reference.
    /// </exception>
    public static Tab FromXmlElement(XElement xmlElement, IDataSourcesRepository dataSourcesRepository)
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

        Section[] sections = xmlElement
            .Elements("Section")
            .Select(sectionDefinitionElement => Section.FromXmlElement(sectionDefinitionElement, dataSourcesRepository))
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
    /// <exception cref="ArgumentException">
    /// Thrown, when at least one argument will be considered as invalid.
    /// </exception>
    private Tab(string name, IEnumerable<Section> sections)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            string argumentName = nameof(name);
            string errorMessage = $"Provided name is invalid: {name}";
            throw new ArgumentException(argumentName, errorMessage);
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
