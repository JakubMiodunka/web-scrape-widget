// Ignore Spelling: App

using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Threading;
using WebScrapeWidget.DataGathering.Repositories;
using WebScrapeWidget.Utilities;


namespace WebScrapeWidget
{
    //TODO: Doc-string.
    public partial class App : Application
    {
        /// <summary>
        /// Application startup procedure.
        /// </summary>
        /// <param name="sender">
        /// The object, that raised the event.
        /// </param>
        /// <param name="eventArguments">
        /// Arguments of raised event.
        /// </param>
        private async void ApplicationStartup(object sender, StartupEventArgs eventArguments)
        {
            await DataSourcesRepository.Instance.StartPeriodicDataGathering();
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
        private void HandleException(object sender, DispatcherUnhandledExceptionEventArgs eventArguments)
        {
            Exception unhandledException = eventArguments.Exception;

            var errorReport = new ErrorReport(unhandledException);
            string errorReportFilePath = "x";// errorReport.SaveAsXml();

            const string Heading = "Unhanded Exception";
            const MessageBoxButton Button = MessageBoxButton.OK;
            const MessageBoxImage Image = MessageBoxImage.Error;

            var errorMessage = new StringBuilder();

            errorMessage.AppendLine($"During application runtime unhanded exception occurred.");
            errorMessage.AppendLine("Incident will be saved as error report file and application will be closed.");
            errorMessage.AppendLine();
            errorMessage.AppendLine($"Error message:");
            errorMessage.AppendLine(unhandledException.Message);
            errorMessage.AppendLine();
            errorMessage.AppendLine($"Saved error report:");
            errorMessage.AppendLine(Path.GetFullPath(errorReportFilePath));

            MessageBoxResult result = MessageBox.Show(errorMessage.ToString(), Heading, Button, Image);

            switch (result)
            {
                default:
                    Shutdown();
                    break;
            }
        }
    }
}
