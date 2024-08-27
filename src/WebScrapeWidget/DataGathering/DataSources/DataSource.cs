// Ignore Spelling: Timestamp

using WebScrapeWidget.DataGathering.Interfaces;


namespace WebScrapeWidget.DataGathering.DataSources;

/// <summary>
/// Base class for all more specific types of data sources.
/// It manages the data shared between all instances
/// of derivative classes and secures their integrity.
/// </summary>
public abstract class DataSource : IDataSource
{
    #region Static properties
    private static List<string> s_occupiedNames = new List<string>();
    #endregion

    #region Properties
    public string Name { get; init; }
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
    /// <param name="dataUnit">
    /// Unit, in which data gathered from source is presented.
    /// </param>
    /// <param name="refreshRate">
    /// Refresh rate of data gathered from source expressed in time period.
    /// Shall be not smaller than 1 second.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown, when at least one reference-type argument is a null reference.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown, when provided data source name is already occupied. 
    /// </exception>
    protected DataSource(string name, string dataUnit, TimeSpan refreshRate)
    {
        if (name is null)
        {
            string argumentName = nameof(name);
            const string ErrorMessage = "Provided data source name is a null reference:";
            throw new ArgumentNullException(argumentName, ErrorMessage);
        }

        if (name == string.Empty)
        {
            string argumentName = nameof(name);
            const string ErrorMessage = "Provided data source name is an empty string:";
            throw new ArgumentOutOfRangeException(argumentName, name, ErrorMessage);
        }

        if (s_occupiedNames.Contains(name))
        {
            string argumentName = nameof(name);
            string errorMessage = $"Provided data source name already in use: {name}";
            throw new ArgumentOutOfRangeException(argumentName, name, errorMessage);
        }

        if (dataUnit is null)
        {
            string argumentName = nameof(dataUnit);
            const string ErrorMessage = "Provided data unit is a null reference:";
            throw new ArgumentNullException(argumentName, ErrorMessage);
        }

        if (refreshRate < new TimeSpan(0, 0, 1))
        {
            string argumentName = nameof(refreshRate);
            string errorMessage = $"Provided value of refresh rate is invalid: {refreshRate}";
            throw new ArgumentOutOfRangeException(argumentName, refreshRate, errorMessage);
        }

        s_occupiedNames.Add(name);
        
        Name = name;
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
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown, when at least one reference-type argument is a null reference.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown, when at least one argument will be considered as invalid.
    /// </exception>
    public void AddSubscriber(IDataSourceSubscriber newSubscriber)
    {
        if (newSubscriber is null)
        {
            string argumentName = nameof(newSubscriber);
            const string ErrorMessage = "Provided subscriber object is a null reference:";
            throw new ArgumentNullException(argumentName, ErrorMessage);
        }

        if (_subscribers.Contains(newSubscriber))
        {
            string argumentName = nameof(newSubscriber);
            const string ErrorMessage = "Provided object is already a subscriber of the data source:";
            throw new ArgumentException(argumentName, ErrorMessage);
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
    /// Dummy implementation of data gathering mechanism of data source.
    /// Shall be overwritten by derivative classes - non abstract
    /// implementations of data sources.
    /// </summary>
    /// <returns>
    /// Failed task.
    /// </returns>
    /// <exception cref="NotImplementedException">
    /// Thrown, when derivative class does not implement data gathering mechanism.
    /// </exception>
    public async Task GatherData()
    {
        const string ErrorMessage = "Data gathering mechanism not implemented:";
        var exception = new NotImplementedException(ErrorMessage);

        await Task.FromException(exception);
    }
    #endregion
}
