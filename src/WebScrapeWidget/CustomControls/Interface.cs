using System.Xml.Linq;
using System.Windows.Controls;

using WebScrapeWidget.Utilities;


namespace WebScrapeWidget.CustomControls;

/// <summary>
/// Custom WPF control build around System.Windows.Controls.TabControl.
/// Contains the main part of application interface.
/// </summary>
public class Interface : TabControl
{
    #region Class instantiation
    /// <summary>
    /// Checks if provided file can be used to obtain
    /// the definition of application user interface.
    /// </summary>
    /// <param name="filePath">
    /// Path to *.xml file, which shall be checked.
    /// </param>
    /// <returns>
    /// True or false, depending on check result.
    /// </returns>
    private static bool IsInterfaceDefinition(string filePath)
    {
        const string InterfaceDefinitionSchema = @"..\..\..\..\..\res\schemas\interface_definition_schema.xsd";

        return FileSystemUtilities.ValidateXmlFile(filePath, InterfaceDefinitionSchema);
    }

    /// <summary>
    /// Creates a new custom WPF control corresponding to definition of user interface
    /// provided within specified file.
    /// </summary>
    /// <param name="filePath">
    /// Path to *.xml file, containing user interface definition.
    /// </param>
    /// <returns>
    /// New custom WPF control corresponding to user interface defined within provided file.
    /// </returns>
    /// <exception cref="FormatException">
    /// Thrown, when format of provided file will be considered as invalid.
    /// </exception>
    public static Interface FromFile(string filePath)
    {
        if (!IsInterfaceDefinition(filePath))
        {
            string errorMessage = $"Invalid format of provided config file: {filePath}";
            throw new FormatException(errorMessage);
        }

        XElement interfaceElement = XDocument.Load(filePath)
            .Elements("InterfaceDefinition")
            .First();

        return FromXml(interfaceElement);
    }

    /// <summary>
    /// Creates a new custom WPF control corresponding to definition of user interface
    /// provided within specified XML element.
    /// </summary>
    /// <param name="interfaceDefinitionElement">
    /// XML element, containing user interface definition.
    /// </param>
    /// <returns>
    /// New custom WPF control corresponding to user interface defined within provided XML element.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown, when at least one reference-type argument is a null reference.
    /// </exception>
    private static Interface FromXml(XElement interfaceDefinitionElement)
    {
        if (interfaceDefinitionElement is null)
        {
            string argumentName = nameof(interfaceDefinitionElement);
            const string ErrorMessage = "Provided XML element is a null reference:";
            throw new ArgumentNullException(argumentName, ErrorMessage);
        }

        Tab[] tabs = interfaceDefinitionElement
            .Elements("Tab")
            .Select(Tab.FromXml)
            .ToArray();

        return new Interface(tabs);
    }

    /// <summary>
    /// Creates a new instance of custom WPF control.
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
