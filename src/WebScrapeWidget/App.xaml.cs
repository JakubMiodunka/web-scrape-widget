// Ignore Spelling: App

using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Threading;
using System.Xml.Linq;
using WebScrapeWidget.DataGathering.Repositories;
using WebScrapeWidget.Utilities;


namespace WebScrapeWidget
{
    //TODO: Doc-string.
    public partial class App : Application
    {
        #region Constants
        private const string AppConfigFilePath = @"..\..\..\..\..\cfg\app_config.xml";
        private const string AppConfigSchema = @"..\..\..\..\..\res\schemas\app_config_schema.xsd";
        #endregion

        #region Config
        private string _errorReportsStorage;
        private string _dataSourcesStorage;
        private bool _dataSourcesStorageRecursiveSearch;
        private string _interfaceDefinitionPath;
        #endregion

        #region Class instantiation
        /// <summary>
        /// Creates a new application instance.
        /// </summary>
        public App() : base()
        {
            FileSystemUtilities.ValidateFile(AppConfigFilePath, ".xml");

            var appConfigDocument = XDocument.Load(AppConfigFilePath);

            XmlUtilities.ValidateXmlDocument(appConfigDocument, AppConfigSchema);

            XElement appConfigElement = appConfigDocument
                .Elements("AppConfig")
                .First();

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

            FileSystemUtilities.ValidateDirectory(errorReportsStorage);
            FileSystemUtilities.ValidateDirectory(dataSourcesStorage);
            FileSystemUtilities.ValidateFile(interfaceDefinitionPath);

            _errorReportsStorage = errorReportsStorage;
            _dataSourcesStorage = dataSourcesStorage;
            _dataSourcesStorageRecursiveSearch = dataSourcesStorageRecursiveSearch;
            _interfaceDefinitionPath = interfaceDefinitionPath;
        }
        #endregion

        #region Procedures
        /// <summary>
        /// Application startup procedure.
        /// </summary>
        /// <param name="sender">
        /// The object, that raised the event.
        /// </param>
        /// <param name="eventArguments">
        /// Arguments of raised event.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown, when at least one reference-type argument is a null reference.
        /// </exception>
        private async void StartupProcedure(object sender, StartupEventArgs eventArguments)
        {
            if (sender is null)
            {
                string argumentName = nameof(sender);
                const string ErrorMessage = "Provided sender is a null reference:";
                throw new ArgumentNullException(argumentName, ErrorMessage);
            }

            if (eventArguments is null)
            {
                string argumentName = nameof(eventArguments);
                const string ErrorMessage = "Provided event arguments are a null reference:";
                throw new ArgumentNullException(argumentName, ErrorMessage);
            }

            var dataSourcesRepoisitory = DataSourcesRepository.FromDirectory(_dataSourcesStorage, _dataSourcesStorageRecursiveSearch);

            var mainWindow = new MainWindow(_interfaceDefinitionPath, dataSourcesRepoisitory);
            mainWindow.Show();

            await dataSourcesRepoisitory.StartPeriodicDataGathering();
        }
        #endregion

        #region Exception handling
        /// <summary>
        /// Displays the message box, which reports occurrence of provided exception to user.
        /// </summary>
        /// <param name="reportedException">
        /// Exception, which shall be reported to user.
        /// </param>
        /// <returns>
        /// Result of displayed message box.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown, when at least one reference-type argument is a null reference.
        /// </exception>
        private MessageBoxResult ShowErrorMessageBox(Exception reportedException)
        {
            if (reportedException is null)
            {
                string argumentName = nameof(reportedException);
                const string ErrorMessage = "Provided exception is a null reference:";
                throw new ArgumentNullException(argumentName, ErrorMessage);
            }

            const string Heading = "Unhanded Exception";
            const MessageBoxButton Button = MessageBoxButton.OK;
            const MessageBoxImage Image = MessageBoxImage.Error;

            var message = new StringBuilder();

            message.AppendLine("During application runtime unhanded exception occurred.");
            message.AppendLine("Exception type:");
            message.AppendLine(reportedException.GetType().FullName);
            message.AppendLine("Error message:");
            message.AppendLine(reportedException.Message);

            MessageBoxResult messageBoxResult = MessageBox.Show(message.ToString(), Heading, Button, Image);

            return messageBoxResult;
        }

        /// <summary>
        /// Prepares error report file related to provided exception.
        /// </summary>
        /// <param name="reportedException">
        /// Exception, which shall be reported by created error report.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown, when at least one reference-type argument is a null reference.
        /// </exception>
        private void SaveAsErrorReport(Exception reportedException)
        {
            if (reportedException is null)
            {
                string argumentName = nameof(reportedException);
                const string ErrorMessage = "Provided exception is a  null reference:";
                throw new ArgumentNullException(argumentName, ErrorMessage);
            }

            const string RawFileName = "error_report";
            const string TimestampFormat = "yyyyMMddTHHmmff";

            var errorReport = new ErrorReport(reportedException);

            string fileName = $"{RawFileName}_{errorReport.Timestamp.ToString(TimestampFormat)}";
            fileName = Path.ChangeExtension(fileName, ".xml");

            string filePath = Path.Combine(_errorReportsStorage, fileName);

            try
            {
                errorReport.SaveAsXml(filePath);
            }
            catch (Exception exception)
            {
                MessageBoxResult messageBoxResult = ShowErrorMessageBox(exception);

                switch (messageBoxResult)
                {
                    default:
                        Shutdown();
                        break;
                }
            }
        }

        /// <summary>
        /// Handler for exceptions, which was thrown by an application but not handled anywhere else.
        /// </summary>
        /// <param name="sender">
        /// The object, that raised the event.
        /// </param>
        /// <param name="eventArguments">
        /// Arguments of raised event.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown, when at least one reference-type argument is a null reference.
        /// </exception>
        private void HandleException(object sender, DispatcherUnhandledExceptionEventArgs eventArguments)
        {
            if (sender is null)
            {
                string argumentName = nameof(sender);
                const string ErrorMessage = "Provided sender is a null reference:";
                throw new ArgumentNullException(argumentName, ErrorMessage);
            }

            if (eventArguments is null)
            {
                string argumentName = nameof(eventArguments);
                const string ErrorMessage = "Provided event arguments are a null reference:";
                throw new ArgumentNullException(argumentName, ErrorMessage);
            }

            Exception unhandledException = eventArguments.Exception;

            MessageBoxResult result = ShowErrorMessageBox(unhandledException);

            switch (result)
            {
                default:
                    SaveAsErrorReport(unhandledException);
                    Shutdown();
                    break;
            }
        }
       #endregion
    }
}
