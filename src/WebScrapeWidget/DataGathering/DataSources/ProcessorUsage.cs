using System.Diagnostics;

using WebScrapeWidget.DataGathering.Interfaces;
using WebScrapeWidget.Utilities;


namespace WebScrapeWidget.DataGathering.DataSources;

/// <summary>
/// Special data source, which monitors the usage of machine CPU.
/// </summary>
/// <remarks>
/// Designed to be accurate to Windows Task Manager.
/// </remarks>
public sealed class ProcessorUsage : DataSource, IDataSource
{
    #region Constants
    private const string DataSourceDescription = "Current CPU usage.";
    #endregion

    #region Properties
    private readonly PerformanceCounter _performanceCounter;
    #endregion

    #region Class instantiation
    /// <summary>
    /// Creates an new data source related to machine CPU usage.
    /// </summary>
    /// <param name="name">
    /// Unique name for created data source.
    /// </param>
    /// <param name="refreshRate">
    /// Refresh rate of data gathered from source expressed in time period.
    /// Shall be not smaller than 1 second.
    /// </param>
    public ProcessorUsage(string name, TimeSpan refreshRate) : base(name, DataSourceDescription, "%", refreshRate)
    {
        
        // Validity of values passed to constructor of PerformanceCounter depends on currently used culture.
        using (var cultureContext = CultureContext.FromCultureName("en-US"))
        {
            _performanceCounter = new PerformanceCounter("Processor Information", "% Processor Utility", "_Total");
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
    public new async Task GatherData()
    {
        await Task.Yield();

        double processorUsage = Convert.ToDouble(_performanceCounter.NextValue());
        processorUsage = Math.Round(processorUsage);
        
        _gatheredData = processorUsage.ToString();
        LastRefreshTimestamp = DateTime.Now;

        NotifySubscribers();
    }
    #endregion
}
