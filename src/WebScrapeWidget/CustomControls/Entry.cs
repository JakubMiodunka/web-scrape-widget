// Ignore Spelling: Timestamp

using System.Text;
using System.Windows.Controls;
using System.Xml.Linq;

using WebScrapeWidget.DataGathering.Interfaces;
using WebScrapeWidget.DataGathering.Repositories;


namespace WebScrapeWidget.CustomControls;

/// <summary>
/// Custom WPF control build around System.Windows.Controls.Grid.
/// Displays details about data contained by subscribed data source.
/// </summary>
internal sealed class Entry : Grid, IDataSourceSubscriber
{
    #region Constants
    private const string UnknownValueIndicator = "-";
    #endregion

    #region Properties
    private readonly TextBlock _label;
    private readonly TextBlock _value;
    #endregion

    #region Class instantiation
    /// <summary>
    /// Creates a new entry corresponding to definition
    /// contained by provided XML element.
    /// </summary>
    /// <param name="entryElement">
    /// XML element, containing entry definition.
    /// </param>
    /// <returns>
    /// New entry corresponding to definition contained by provided XML element.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown, when at least one reference-type argument is a null reference.
    /// </exception>
    internal static Entry FromXml(XElement entryElement)
    {
        if (entryElement is null)
        {
            string argumentName = nameof(entryElement);
            const string ErrorMessage = "Provided XML element is a null reference:";
            throw new ArgumentNullException(argumentName, ErrorMessage);
        }

        string label = entryElement
            .Attributes("Label")
            .Select(attribute => attribute.Value)
            .First();

        string dataSourceName = entryElement
            .Attributes("DataSourceName")
            .Select(attribute => attribute.Value)
            .First();

        return new Entry(label, dataSourceName);
    }

    /// <summary>
    /// Creates a new entry instance.
    /// </summary>
    /// <param name="label">
    /// String, which shall be used as label for value shown by the entry.
    /// </param>
    /// <param name="dataSourceName">
    /// Name of data source, to which entry shall be subscribed.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown, when at least one reference-type argument is a null reference.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown ,when value of at least one argument will be considered as invalid.
    /// </exception>
    private Entry(string label, string dataSourceName) : base()
    {
        if (label is null)
        {
            string argumentName = nameof(label);
            const string ErrorMessage = "Provided entry label is a null reference:";
            throw new ArgumentNullException(argumentName, ErrorMessage);
        }

        if (label == string.Empty)
        {
            string argumentName = nameof(label);
            const string ErrorMessage = "Provided entry label is an empty string:";
            throw new ArgumentOutOfRangeException(argumentName, label, ErrorMessage);
        }

        if (dataSourceName is null)
        {
            string argumentName = nameof(dataSourceName);
            const string ErrorMessage = "Provided data source name is a null reference:";
            throw new ArgumentNullException(argumentName, ErrorMessage);
        }

        if (dataSourceName == string.Empty)
        {
            string argumentName = nameof(dataSourceName);
            const string ErrorMessage = "Provided data source name is an empty string:";
            throw new ArgumentOutOfRangeException(argumentName, dataSourceName, ErrorMessage);
        }

        while (ColumnDefinitions.Count() < 2)
        {
            var newColumnDefinition = new ColumnDefinition();
            ColumnDefinitions.Add(newColumnDefinition);
        }
        
        _label = new TextBlock();
        _label.Text = label;
        SetColumn(_label, 0);
        Children.Add( _label);

        _value = new TextBlock();
        _value.TextAlignment = System.Windows.TextAlignment.Right;
        _value.Text = UnknownValueIndicator;
        SetColumn(_value, 1);
        Children.Add( _value);

        IDataSource dataSource = DataSourcesRepository.Instance
            .GetDataSource(dataSourceName);

        UpdateToolTip(dataSource);

        dataSource.AddSubscriber(this);
    }
    #endregion

    #region Notifications
    /// <summary>
    /// Invoked by subscribed data source, when new data will be gathered.
    /// </summary>
    /// <param name="sender">
    /// Instance of data source, which sends the notification.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown, when at least one reference-type argument is a null reference.
    /// </exception>
    public void Notify(IDataSource sender)
    {
        if (sender is null)
        {
            string argumentName = nameof(sender);
            const string ErrorMessage = "Provided sender instance is a null reference:";
            throw new ArgumentNullException(argumentName, ErrorMessage);
        }

        if (sender.DataUnit == string.Empty)
        {
            _value.Text = sender.GatheredData;
        }
        else
        {
            _value.Text = $"{sender.GatheredData} {sender.DataUnit}";
        }

        UpdateToolTip(sender);
    }

    /// <summary>
    /// Updates the content of control tool tip
    /// with details about provided data source.
    /// </summary>
    /// <param name="dataSource">
    /// Data source, which will be used to update control tool tip content.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown, when at least one reference-type argument is a null reference.
    /// </exception>
    private void UpdateToolTip(IDataSource dataSource)
    {
        if (dataSource is null)
        {
            string argumentName = nameof(dataSource);
            const string ErrorMessage = "Provided data source is a null reference:";
            throw new ArgumentNullException(argumentName, ErrorMessage);
        }

        var toolTipContent = new StringBuilder();

        toolTipContent.AppendLine("Data source:");
        toolTipContent.AppendLine($"\tName: {dataSource.Name}");
        toolTipContent.AppendLine($"\tRefresh rate: {dataSource.RefreshRate}");

        if (dataSource.LastRefreshTimestamp == DateTime.MinValue)
        {
            toolTipContent.AppendLine($"\tLast refresh: {UnknownValueIndicator}");
        }
        else
        {
            toolTipContent.AppendLine($"\tLast refresh: {dataSource.LastRefreshTimestamp.ToString("s")}");
        }

        ToolTip = toolTipContent.ToString();
    }
    #endregion
}
