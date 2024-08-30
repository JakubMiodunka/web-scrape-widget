// Ignore Spelling: App
using System.Xml.Linq;

using WebScrapeWidget.Utilities;


namespace WebScrapeWidget;

/// <summary>
/// Container for configuration values of the program.
/// </summary>
public sealed class AppConfig
{
    #region Singleton
    private const string AppConfigFilePath = @"..\..\..\..\..\cfg\app_config.xml";
    public static AppConfig Instance
    {
        get
        {
            if(s_instance is null)
            {
                s_instance = FromFile(AppConfigFilePath);
            }

            return s_instance;
        }
    }

    private static AppConfig? s_instance;
    #endregion

    #region Properties
    public readonly string ErrorReportsStorage;
    public readonly string DataSourcesStorage;
    public readonly bool DataSourcesStorageRecursiveSearch;
    public readonly string InterfaceDefinitionPath;
    #endregion

    #region Class instantiation
    /// <summary>
    /// Checks if provided file can be used to obtain program configuration.
    /// </summary>
    /// <param name="filePath">
    /// Path to *.xml file, which shall be checked.
    /// </param>
    /// <returns>
    /// True or false, depending on check result.
    /// </returns>
    private static bool IsAppConfig(string filePath)
    {
        const string AppConfigSchema = @"..\..\..\..\..\res\schemas\app_config_schema.xsd";

        return FileSystemUtilities.ValidateXmlFile(filePath, AppConfigSchema);
    }

    /// <summary>
    /// Creates a new container for program configuration values using provided *.xml file.
    /// </summary>
    /// <param name="filePath">
    /// Path to *.xml file, containing program configuration values.
    /// </param>
    /// <returns>
    /// New class instance containing configuration values obtained from provided file.
    /// </returns>
    /// <exception cref="FormatException">
    /// Thrown, when format of provided file will be considered as invalid.
    /// </exception>
    private static AppConfig FromFile(string filePath)
    {
        if (!IsAppConfig(filePath))
        {
            string errorMessage = $"Invalid format of provided config file: {filePath}";
            throw new FormatException(errorMessage);
        }

        XElement appConfigElement = XDocument.Load(filePath)
            .Elements("AppConfig")
            .First();

        return FromXml(appConfigElement);
    }

    /// <summary>
    /// Creates a new container for program configuration values using provided XML element.
    /// </summary>
    /// <param name="appConfigElement">
    /// XML element, containing program configuration values.
    /// </param>
    /// <returns>
    /// New class instance containing configuration values obtained from provided XML element.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown, when at least one reference-type argument is a null reference.
    /// </exception>
    private static AppConfig FromXml(XElement appConfigElement)
    {
        if (appConfigElement is null)
        {
            string argumentName = nameof(appConfigElement);
            const string ErrorMessage = "Provided XML element is a null reference:";
            throw new ArgumentNullException(argumentName, ErrorMessage);
        }

        string errorReportsStorage = appConfigElement
            .Elements("ErrorReportsStorage")
            .Attributes("Path")
            .Select(attribute => attribute.Value)
            .First();

        XElement dataSourcesStorageElement = appConfigElement
            .Elements("DataSourcesStorage")
            .First();

        string dataSourcesStorage = dataSourcesStorageElement
            .Attributes("Path")
            .Select(attribute => attribute.Value)
            .First();

        bool dataSourcesStorageRecursiveSearch = dataSourcesStorageElement
            .Attributes("RecursiveSearch")
            .Select(attribute => attribute.Value)
            .Select(value => value == "enabled")
            .First();

        string interfaceDefinitionPath = appConfigElement
            .Elements("InterfaceDefinition")
            .Attributes("Path")
            .Select(attribute => attribute.Value)
            .First();

        return new AppConfig(errorReportsStorage, dataSourcesStorage, dataSourcesStorageRecursiveSearch, interfaceDefinitionPath);
    }

    /// <summary>
    /// Creates a new container for program configuration values.
    /// </summary>
    /// <param name="errorReportsStorage">
    /// Path to directory, where error report files shall be saved.
    /// </param>
    /// <param name="dataSourcesStorage">
    /// Path to directory, where files containing data sources definitions are stored.
    /// </param>
    /// <param name="dataSourcesStorageRecursiveSearch">
    /// Flag, which specifies if data sources storage directory shall be searched recursively.
    /// </param>
    /// <param name="interfaceDefinitionPath">
    /// Path to *.xml file, which content defines the appearance of application interface.
    /// </param>
    private AppConfig(string errorReportsStorage, string dataSourcesStorage, bool dataSourcesStorageRecursiveSearch, string interfaceDefinitionPath)
    {
        FileSystemUtilities.ValidateDirectory(errorReportsStorage);
        FileSystemUtilities.ValidateDirectory(dataSourcesStorage);
        FileSystemUtilities.ValidateFile(interfaceDefinitionPath, ".xml");

        ErrorReportsStorage = errorReportsStorage;
        DataSourcesStorage = dataSourcesStorage;
        DataSourcesStorageRecursiveSearch = dataSourcesStorageRecursiveSearch;
        InterfaceDefinitionPath = interfaceDefinitionPath;
    }
    #endregion
}
