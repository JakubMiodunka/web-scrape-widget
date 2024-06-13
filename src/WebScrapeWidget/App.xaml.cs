// Ignore Spelling: App

using System.Windows;

using WebScrapeWidget.DataGathering.Repositories;


namespace WebScrapeWidget
{
    //TODO: Doc-string.
    public partial class App : Application
    {
        /// <summary>
        /// Implementation of System.Windows.StartupEventHandler,
        /// which performs startup procedure of the application.
        /// </summary>
        /// <param name="sender">
        /// The object that raised startup event.
        /// </param>
        /// <param name="eventData">
        /// Event data.
        /// </param>
        public async void ApplicationStartup(object sender, StartupEventArgs eventData)
        {
            await DataSourcesRepository.Instance.GatherDataFromAllSources();

            var mainWindow = new MainWindow();
            mainWindow.Show();
            
            MainWindow = mainWindow;
        }
    }
}
