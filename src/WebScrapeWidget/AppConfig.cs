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
    private const string AppConfigFilePath = @"..\..\..\..\..\config\app_config.xml";
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
    public readonly string DataSourcesStorage;
    public readonly bool DataSourcesStorageRecursiveSearch;
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

        return new AppConfig(dataSourcesStorage, dataSourcesStorageRecursiveSearch);
    }

    /// <summary>
    /// Creates a new container for program configuration values.
    /// </summary>
    /// <param name="dataSourcesStorage">
    /// Path to directory, where files containing data sources definitions are stored.
    /// </param>
    /// <param name="dataSourcesStorageRecursiveSearch">
    /// Flag, which specifies if data sources storage directory shall be searched recursively.
    /// </param>
    private AppConfig(string dataSourcesStorage, bool dataSourcesStorageRecursiveSearch)
    {
        FileSystemUtilities.ValidateDirectory(dataSourcesStorage);

        DataSourcesStorage = dataSourcesStorage;
        DataSourcesStorageRecursiveSearch = dataSourcesStorageRecursiveSearch;
    }
    #endregion
}
