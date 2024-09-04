// Ignore Spelling: Timestamp

using System.IO;
using System.Security.Principal;
using System.Xml.Linq;


namespace WebScrapeWidget.Utilities
{
    /// <summary>
    /// Error report, which can be saved for diagnostics poorhouses.
    /// </summary>
    /// TODO: Fix the loop where error report cannot obtain storage director from
    /// app config so throws exception which shall be handled once again.
    public sealed class ErrorReport
    {
        #region Constants
        const string ErrorReportSchema = @"..\..\..\..\..\res\schemas\error_report_schema.xsd";
        #endregion

        #region Properties
        public readonly Exception Exception; 
        public readonly DateTime Timestamp;
        #endregion

        #region Class instantiation
        /// <summary>
        /// Creates a new error report representation.
        /// </summary>
        /// <param name="exception">
        /// Exception, which shall be detailed in created error report.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown, when at least one reference-type argument is a null reference.
        /// </exception>
        public ErrorReport (Exception exception)
        {
            if (exception is null)
            {
                string argumentName = nameof(exception);
                const string ErrorMessage = "Provided exception instance is a null reference:";
                throw new ArgumentNullException(argumentName, ErrorMessage);
            }

            Exception = exception;
            Timestamp = DateTime.Now;
        }
        #endregion

        #region XML utilities
        /// <summary>
        /// Gen rates XML-based representation of provided exception.
        /// </summary>
        /// <param name="exception">
        /// Exception, which shall be detailed by generated XML element.
        /// </param>
        /// <returns>
        /// XML element, which details provided exception.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown, when at least one reference-type argument is a null reference.
        /// </exception>
        private static XElement AsXElement(Exception exception)
        {
            if (exception is null)
            {
                string argumentName = nameof(exception);
                const string ErrorMessage = "Provided exception instance is a null reference:";
                throw new ArgumentNullException(argumentName, ErrorMessage);
            }

            string exceptionType = exception.GetType().FullName ?? "Unknown";

            var exceptionElement = new XElement("Exception",
                new XAttribute("Type", exceptionType));

            string className = exception.Source ?? "Unknown";
            string methodName = (exception.TargetSite is null) ? "Unknown" : exception.TargetSite.Name;

            var sourceElement = new XElement("Source",
                    new XAttribute("Class", className),
                    new XAttribute("Method", methodName));

            exceptionElement.Add(sourceElement);

            var messageElement = new XElement("Message");
            messageElement.Value = exception.Message;

            exceptionElement.Add(messageElement);

            var stackTraceElement = new XElement("StackTrace");
            stackTraceElement.Value = exception.StackTrace ?? string.Empty;

            exceptionElement.Add(stackTraceElement);

            var innerExceptionElement = new XElement("InnerException");
            if (exception.InnerException is not null)
            {
                innerExceptionElement.Add(AsXElement(exception.InnerException));
            }

            exceptionElement.Add(innerExceptionElement);

            return exceptionElement;
        }

        /// <summary>
        /// Generates XML-based representation of error report.
        /// </summary>
        /// <returns>
        /// XML element, which details an error report.
        /// </returns>
        public XElement AsXElement()
        {
            string timestamp = Timestamp.ToString("yyyy-MM-ddTHH:mm:sszzz");

            var errorReportElement = new XElement("ErrorReport",
                new XAttribute("Timestamp", timestamp),
                AsXElement(Exception));

            return errorReportElement;
        }
        #endregion

        #region Saving as XML file
        /// <summary>
        /// Generates file path, under which error report
        /// shall be saved by default.
        /// </summary>
        /// <returns>
        /// File path, under which error report shall be saved by default.
        /// </returns>
        private string GenerateFilePath()
        {
            const string FileNamePrefix = "web_scrape_widget_error_report_";
            const string FileExtension = ".xml";
            
            /*
             * Timestamp cannot be formated according to ISO standard
             * as it would contain characters not allowed in file names
             * in Windows operating system.
             */
            const string TimestampFormat = "yyyyMMdd_HHmmssfff";
            string timestamp = Timestamp.ToString(TimestampFormat);

            string fileName = $"{FileNamePrefix}{timestamp}";
            fileName = Path.ChangeExtension(fileName, FileExtension);

            string errorReportsStorage = AppConfig.Instance.ErrorReportsStorage;

            string filePath = Path.Combine(errorReportsStorage, fileName);

            return filePath;
        }

        /// <summary>
        /// Saves error report as XML file under specified file path.
        /// </summary>
        /// <param name="filePath">
        /// File path, under which generated XML file shall be saved.
        /// </param>
        /// TODO: Maybe add some kind of callback if XSD validation will fail.
        public void SaveAsXml(string filePath)
        {
            FileSystemUtilities.ValidateFile(filePath, ".xml", false);

            var xmlDocument = new XDocument(AsXElement());

            xmlDocument.Save(filePath, SaveOptions.None);

            bool isErrorReportValid = FileSystemUtilities.ValidateXmlFile(filePath, ErrorReportSchema);

            // TODO: Re-factor and improve.
            if (!isErrorReportValid)
            {
                throw new FormatException();
            }
        }

        /// <summary>
        /// Saves error report as XML file in default location.
        /// </summary>
        /// <returns>
        /// File path, under which error report was saved.
        /// </returns>
        public string SaveAsXml()
        {
            string filePath = GenerateFilePath();

            SaveAsXml(filePath);

            return filePath;
        }
        #endregion
    }
}
