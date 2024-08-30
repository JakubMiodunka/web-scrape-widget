// Ignore Spelling: App

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
        /// TODO: Re-factor and improve.
        private void HandleException(object sender, DispatcherUnhandledExceptionEventArgs eventArguments)
        {
            var errorReport = new ErrorReport(eventArguments.Exception);
            errorReport.SaveAsXml();

            const string ErrorMessage = "Unexpected error occurred.\nError report was saved in default location.";

            MessageBox.Show(ErrorMessage);

            Shutdown();
        }
    }
}
