using System.Windows.Controls;
using System.Xml.Linq;
using WebScrapeWidget.DataGathering.Interfaces;
using WebScrapeWidget.Utilities;


namespace WebScrapeWidget.CustomControls;

/// <summary>
/// Custom WPF control build around System.Windows.Controls.TabControl.
/// Its content is divided into tabs.
/// </summary>
public class Interface : TabControl
{
    #region Constants
    const string InterfaceDefinitionSchema = @"..\..\..\..\..\res\schemas\interface_definition_schema.xsd";
    #endregion

    #region Class instantiation
    /// <summary>
    /// Creates a new application interface corresponding to provided XML document.
    /// </summary>
    /// <param name="xmlDocument">
    /// XML document, containing application interface definition.
    /// </param>
    /// <param name="dataSourcesRepository">
    /// Data sources repository, which shall be used to obtain displayed data.
    /// </param>
    /// <returns>
    /// New application interface instance corresponding to definition contained by provided XML file.
    /// </returns>
    public static Interface FromXmlDocument(XDocument xmlDocument, IDataSourcesRepository dataSourcesRepository)
    {
        XmlUtilities.ValidateXmlDocument(xmlDocument, InterfaceDefinitionSchema);

        XElement interfaceDefinitionElement = xmlDocument
            .Elements("InterfaceDefinition")
            .First();

        Tab[] tabs = interfaceDefinitionElement
            .Elements("Tab")
            .Select(tabDefinitionElement => Tab.FromXmlElement(tabDefinitionElement, dataSourcesRepository))
            .ToArray();

        return new Interface(tabs);
    }

    /// <summary>
    /// Creates a new instance of application interface.
    /// </summary>
    /// <param name="tabs">
    /// Collection of tabs, which shall be contained by created interface.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown, when at least one reference-type argument is a null reference.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown, when at least one argument will be considered as invalid.
    /// </exception>
    private Interface(IEnumerable<Tab> tabs) : base()
    {
        if (tabs is null)
        {
            string argumentName = nameof(tabs);
            const string ErrorMessage = "Provided tabs collection is a null reference:";
            throw new ArgumentNullException(argumentName, ErrorMessage);
        }

        if (tabs.Contains(null))
        {
            string argumentName = nameof(tabs);
            const string ErrorMessage = "Provided tabs collection contains a null reference:";
            throw new ArgumentException(ErrorMessage, argumentName);
        }

        tabs.ToList().ForEach(tab => Items.Add(tab));
    }
    #endregion
}
