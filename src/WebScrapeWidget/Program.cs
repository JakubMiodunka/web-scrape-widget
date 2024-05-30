using WebScrapeWidget;
using WebScrapeWidget.DataGathering.Repositories;

DataSourcesRepository PrepareDataSourcesRepository()
{
    string directoryPath = AppConfig.Instance.DataSourcesStorage;
    bool recursiveSearch = AppConfig.Instance.DataSourcesStorageRecursiveSearch;

    return new DataSourcesRepository(directoryPath, recursiveSearch);
}

DataSourcesRepository dataSourcesRepository =  PrepareDataSourcesRepository();
Console.ReadLine();