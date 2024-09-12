using System.Xml.Linq;
using System.Windows.Controls;

using WebScrapeWidget.Utilities;
using WebScrapeWidget.DataGathering.Interfaces;


namespace WebScrapeWidget.CustomControls;

/// <summary>
/// Custom WPF control build around System.Windows.Controls.TabControl.
/// Contains the main part of application interface.
/// </summary>
public class Interface : TabControl
{
    #region Constants
    const string InterfaceDefinitionSchema = @"..\..\..\..\..\res\schemas\interface_definition_schema.xsd";
    #endregion

    #region Class instantiation
    /// <summary>
    /// Checks if provided XML document contains application interface definition.
    /// </summary>
    /// <param name="xmlDocument">
    /// XML document, which shall be checked.
    /// </param>
    /// <returns>
    /// True or false, depending on check result.
    /// </returns>
    public static bool IsInterfaceDefinition(XDocument xmlDocument)
    {
        return XmlUtilities.ValidateXmlDocument(xmlDocument, InterfaceDefinitionSchema);
    }

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
    /// <exception cref="FormatException">
    /// Thrown, when format of provided file will be considered as invalid.
    /// </exception>
    public static Interface FromXmlDocument(XDocument xmlDocument, IDataSourcesRepository dataSourcesRepository)
    {
        if (!IsInterfaceDefinition(xmlDocument))
        {
            const string ErrorMessage = $"Invalid format of provided XML document:";
            throw new FormatException(ErrorMessage);
        }

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
    private Interface(IEnumerable<Tab> tabs)
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
            throw new ArgumentException(argumentName, ErrorMessage);
        }

        tabs.ToList().ForEach(tab => Items.Add(tab));
    }
    #endregion
}
