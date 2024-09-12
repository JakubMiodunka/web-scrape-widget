using Moq;
using WebScrapeWidget.DataGathering.Interfaces;
using WebScrapeWidget.DataGathering.Repositories;
using WebScrapeWidgetTests.DataGathering.DataSources;


namespace WebScrapeWidgetTests.DataGathering.Repositories;

/// <summary>
/// Test fixture for DataSourcesRepository type.
/// </summary>
[TestOf(typeof(DataSourcesRepository))]
[Author("Jakub Miodunka")]
public sealed class DataSourcesRepositoryTests
{
    #region Fakes creation
    /// <summary>
    /// Creates fake implementation of data sources repository according to specified arguments.
    /// </summary>
    /// <param name="dataSources">
    /// Collection of data sources, which shall be contained by created data sources repository.
    /// </param>
    /// <returns>
    /// Fake implementation of data sources repository according to specified arguments.
    /// </returns>
    public static Mock<IDataSourcesRepository> CreateFakeDataSourcesRepository(params IDataSource[] dataSources)
    {
        var fakeDataSourcesRepository = new Mock<IDataSourcesRepository>();

        foreach (IDataSource dataSource in dataSources)
        {
            fakeDataSourcesRepository
                .Setup(dataSourcesRepository => dataSourcesRepository.GetDataSource(dataSource.Name))
                .Returns(dataSource);
        }

        return fakeDataSourcesRepository;
    }

    /// <summary>
    /// Creates fake implementation of default data sources repository.
    /// </summary>
    /// <remarks>
    /// Fake default data sources repository contains fake implementation of default data source. 
    /// </remarks>
    /// <returns>
    /// Fake implementation of default data sources repository.
    /// </returns>
    public static Mock<IDataSourcesRepository> CreateFakeDefaultDataSourcesRepository()
    {
        Mock<IDataSource> defaultDataSourceStub = DataSourceTests.CreateFakeDefaultDataSource();

        return CreateFakeDataSourcesRepository(defaultDataSourceStub.Object);
    }
    #endregion
}
