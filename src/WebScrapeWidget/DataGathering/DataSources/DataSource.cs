// Ignore Spelling: Timestamp

using WebScrapeWidget.DataGathering.Interfaces;
using WebScrapeWidget.Utilities;


namespace WebScrapeWidget.DataGathering.DataSources;

/// <summary>
/// Base class for all more specific types of data sources.
/// </summary>
public abstract class DataSource : IDataSource
{
    #region Properties
    public string Name { get; init; }
    public string Description { get; init; }
    public bool WasDataGathered
    {
        get
        {
            return _gatheredData is not null;
        }
    }
    public string GatheredData
    {
        get
        {
            if (_gatheredData is null)
            {
                const string ErrorMessage = "Attempting to access gathered data before the gathering.";
                throw new InvalidOperationException(ErrorMessage);
            }

            return _gatheredData;
        }
    }
    public string DataUnit { get; init; }
    public TimeSpan RefreshRate { get; init; }
    public DateTime LastRefreshTimestamp { get; protected set; }
    public bool IsSubscribed
    {
        get
        {
            return _subscribers.Any();
        }
    }

    protected string? _gatheredData;
    protected List<IDataSourceSubscriber> _subscribers;
    #endregion

    #region Class instantiation
    /// <summary>
    /// Base constructor of every data source type.
    /// </summary>
    /// <param name="name">
    /// Unique name of represented data source.
    /// </param>
    /// <param name="description">
    /// Description of created data source.
    /// </param>
    /// <param name="dataUnit">
    /// Unit, in which data gathered from source is presented.
    /// </param>
    /// <param name="refreshRate">
    /// Refresh rate of data gathered from source expressed in time period.
    /// Shall be not shorter than 2 second.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown, when at least one reference-type argument is a null reference.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown, when provided data source name is already occupied. 
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown, when at least one argument will be considered as invalid.
    /// </exception>
    protected DataSource(string name, string description, string dataUnit, TimeSpan refreshRate)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            string argumentName = nameof(name);
            string errorMessage = $"Provided name is invalid: {name}";
            throw new ArgumentException(argumentName, errorMessage);
        }

        if (description is null)
        {
            string argumentName = nameof(description);
            const string ErrorMessage = "Provided data source description is a null reference:";
            throw new ArgumentNullException(argumentName, ErrorMessage);
        }

        if (dataUnit is null)
        {
            string argumentName = nameof(dataUnit);
            const string ErrorMessage = "Provided data unit is a null reference:";
            throw new ArgumentNullException(argumentName, ErrorMessage);
        }

        if (refreshRate < TimeSpan.FromSeconds(2))
        {
            string argumentName = nameof(refreshRate);
            string errorMessage = $"Provided value of refresh rate is invalid: {refreshRate}";
            throw new ArgumentOutOfRangeException(argumentName, refreshRate, errorMessage);
        }
       
        Name = name;
        Description = StringUtilities.NormalizeMultiLineString(description);
        DataUnit = dataUnit;
        RefreshRate = refreshRate;
        LastRefreshTimestamp = DateTime.MinValue;

        _subscribers = new List<IDataSourceSubscriber>();
    }
    #endregion

    #region Subscription mechanism
    /// <summary>
    /// Adds a new object to subscribers list of data source.
    /// </summary>
    /// <param name="newSubscriber">
    /// Object, which shall be added to data source subscriber list.
    /// If provided subscriber is already on the list,
    /// method will exit without performing any action. 
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown, when at least one reference-type argument is a null reference.
    /// </exception>
    public void AddSubscriber(IDataSourceSubscriber newSubscriber)
    {
        if (newSubscriber is null)
        {
            string argumentName = nameof(newSubscriber);
            const string ErrorMessage = "Provided subscriber object is a null reference:";
            throw new ArgumentNullException(argumentName, ErrorMessage);
        }

        if (_subscribers.Any(subscriber => ReferenceEquals(subscriber, newSubscriber)))
        {
            return;
        }

        _subscribers.Add(newSubscriber);
    }

    /// <summary>
    /// Notifies all subscribers about new value of data gathered from data source.
    /// </summary>
    protected void NotifySubscribers()
    {
        _subscribers.ForEach(subscriber => subscriber.Notify(this));
    }
    #endregion

    #region Data gathering
    /// <summary>
    /// Dummy data gathering mechanism of data source.
    /// Shall be overwritten by derivative classes - non abstract
    /// implementations of data sources.
    /// </summary>
    public abstract Task GatherData();
    #endregion
}
