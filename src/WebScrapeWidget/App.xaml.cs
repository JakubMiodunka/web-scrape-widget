// Ignore Spelling: App

using System.Windows;

using WebScrapeWidget.DataGathering.Repositories;


namespace WebScrapeWidget
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public void Program()
        {
            DataSourcesRepository PrepareDataSourcesRepository()
            {
                string directoryPath = AppConfig.Instance.DataSourcesStorage;
                bool recursiveSearch = AppConfig.Instance.DataSourcesStorageRecursiveSearch;

                return new DataSourcesRepository(directoryPath, recursiveSearch);
            }

            DataSourcesRepository dataSourcesRepository = PrepareDataSourcesRepository();
        }
    }
}
