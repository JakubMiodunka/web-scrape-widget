// Ignore Spelling: App

using System.Windows;

using WebScrapeWidget.DataGathering.Repositories;


namespace WebScrapeWidget
{
    //TODO: Doc-string.
    public partial class App : Application
    {
        //TODO: Doc-string.
        private async void ApplicationStartup(object sender, StartupEventArgs eventData)
        {
            MainWindow = new MainWindow();
            MainWindow.Show();

            await DataSourcesRepository.Instance.StartPeriodicDataGathering();
        }
    }
}
