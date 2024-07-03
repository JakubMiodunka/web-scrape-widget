// Ignore Spelling: App

using System.Windows;

using WebScrapeWidget.DataGathering.Repositories;


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
    }
}
