using System.Diagnostics;

using WebScrapeWidget.DataGathering.Interfaces;
using WebScrapeWidget.Utilities;


namespace WebScrapeWidget.DataGathering.DataSources;

/// <summary>
/// Special data source, which monitors the usage of machine RAM.
/// </summary>
/// <remarks>
/// Currently not fully accurate to WIndows Tass manager (divination: from +1% to +4%).
/// </remarks>
public sealed class RamUsage : DataSource, IDataSource
{
    #region Properties
    private readonly PerformanceCounter _performanceCounter;
    #endregion

    #region Class instantiation
    /// <summary>
    /// Creates an new data source related to machine RAM usage.
    /// </summary>
    /// <param name="name">
    /// Unique name for created data source.
    /// </param>
    /// <param name="refreshRate">
    /// Refresh rate of data gathered from source expressed in time period.
    /// Shall be not smaller than 1 second.
    /// </param>
    public RamUsage(string name, TimeSpan refreshRate) : base(name, "%", refreshRate)
    {

        // Validity of values passed to constructor of PerformanceCounter depends on currently used culture.
        using (var cultureContext = CultureContext.FromCultureName("en-US"))
        {
            _performanceCounter = new PerformanceCounter("Memory", "% Committed Bytes In Use");
        }

        // First value obtained from PerformanceCounter instance is always zero.
        _performanceCounter.NextValue();
    }
    #endregion

    #region Data gathering
    /// <summary>
    /// Triggers the process of gathering data from data source.
    /// </summary>
    /// <returns>
    /// Task related to data gathering process.
    /// </returns>
    public async Task GatherData()
    {
        await Task.Yield();

        double ramUdage = Convert.ToDouble(_performanceCounter.NextValue());
        ramUdage = Math.Round(ramUdage);

        _gatheredData = ramUdage.ToString();
        LastRefreshTimestamp = DateTime.Now;

        NotifySubscribers();
    }
    #endregion
}
