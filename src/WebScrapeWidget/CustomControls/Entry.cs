// Ignore Spelling: Timestamp

using System.Text;
using System.Windows.Controls;
using System.Xml.Linq;
using WebScrapeWidget.DataGathering.Interfaces;


namespace WebScrapeWidget.CustomControls;

/// <summary>
/// Custom WPF control build around System.Windows.Controls.Grid.
/// Displays details about data contained by subscribed data source.
/// </summary>
public sealed class Entry : Grid, IDataSourceSubscriber
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
    /// Creates a new entry corresponding provided XML element.
    /// </summary>
    /// <param name="xmlElement">
    /// XML element, containing entry definition.
    /// </param>
    /// <param name="dataSourcesRepository">
    /// Data sources repository, which shall be used to obtain displayed data.
    /// </param>
    /// <returns>
    /// New entry corresponding to definition contained by provided XML element.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown, when at least one reference-type argument is a null reference.
    /// </exception>
    public static Entry FromXmlElement(XElement xmlElement, IDataSourcesRepository dataSourcesRepository)
    {
        if (xmlElement is null)
        {
            string argumentName = nameof(xmlElement);
            const string ErrorMessage = "Provided XML element is a null reference:";
            throw new ArgumentNullException(argumentName, ErrorMessage);
        }

        if (dataSourcesRepository is null)
        {
            string argumentName = nameof(dataSourcesRepository);
            const string ErrorMessage = "Provided data sources repository is a null reference:";
            throw new ArgumentNullException(argumentName, ErrorMessage);
        }

        string label = xmlElement
            .Attributes("Label")
            .Select(attribute => attribute.Value)
            .First();

        string dataSourceName = xmlElement
            .Attributes("DataSourceName")
            .Select(attribute => attribute.Value)
            .First();

        IDataSource dataSource = dataSourcesRepository.GetDataSource(dataSourceName);

        return new Entry(label, dataSource);
    }

    /// <summary>
    /// Creates a new entry instance.
    /// </summary>
    /// <param name="label">
    /// String, which shall be used as label for value shown by the entry.
    /// </param>
    /// <param name="dataSource">
    /// Data source, to which entry shall be subscribed.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown, when at least one reference-type argument is a null reference.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown ,when value of at least one argument will be considered as invalid.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown, when at least one argument will be considered as invalid.
    /// </exception>
    private Entry(string label, IDataSource dataSource) : base()
    {
        if (string.IsNullOrWhiteSpace(label))
        {
            string argumentName = nameof(label);
            string errorMessage = $"Provided label is invalid: {label}";
            throw new ArgumentException(argumentName, errorMessage);
        }

        if (dataSource is null)
        {
            string argumentName = nameof(dataSource);
            const string ErrorMessage = "Provided data source is a null reference:";
            throw new ArgumentNullException(argumentName, ErrorMessage);
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

        dataSource.AddSubscriber(this);

        UpdateToolTip(dataSource);
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
    private void UpdateToolTip(IDataSource dataSource)
    {
        const string Indentation = "  ";

        string[] descriptionLines = dataSource.Description
            .Split('\n')
            .Select(line => $"{Indentation}{line}")
            .ToArray();

        var toolTipContent = new StringBuilder();

        toolTipContent.AppendLine($"Name:");
        toolTipContent.AppendLine($"{Indentation}{dataSource.Name}");
        toolTipContent.AppendLine($"Description:");
        toolTipContent.AppendLine($"{string.Join('\n', descriptionLines)}");
        toolTipContent.AppendLine($"Refresh rate:");
        toolTipContent.AppendLine($"{Indentation}{dataSource.RefreshRate}");
        toolTipContent.AppendLine($"Last refresh:");

        if (dataSource.LastRefreshTimestamp == DateTime.MinValue)
        {
            toolTipContent.Append($"{Indentation}{UnknownValueIndicator}");
        }
        else
        {
            toolTipContent.Append($"{Indentation}{dataSource.LastRefreshTimestamp.ToString("s")}");
        }
      
        ToolTip = toolTipContent.ToString();
    }
    #endregion
}
