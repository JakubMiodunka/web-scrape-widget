using System.Xml.Linq;
using System.Windows.Controls;


namespace WebScrapeWidget.CustomControls;

/// <summary>
/// Custom WPF control build around System.Windows.Controls.GroupBox.
/// Its content contains key-value entries.
/// </summary>
public sealed class Section : GroupBox
{
    #region Properties
    private readonly Grid _grid;
    #endregion

    #region Class instantiation
    /// <summary>
    /// Creates a new section corresponding to definition
    /// contained by provided XML element.
    /// </summary>
    /// <param name="sectionElement">
    /// XML element, containing section definition.
    /// </param>
    /// <returns>
    /// New section corresponding to definition contained by provided XML element.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown, when at least one reference-type argument is a null reference.
    /// </exception>
    public static Section FromXml(XElement sectionElement)
    {
        if (sectionElement is null)
        {
            string argumentName = nameof(sectionElement);
            const string ErrorMessage = "Provided XML element is a null reference:";
            throw new ArgumentNullException(argumentName, ErrorMessage);
        }

        string name = sectionElement
            .Attributes("Name")
            .Select(attribute => attribute.Value)
            .First();

        Entry[] entries = sectionElement
            .Elements("Entry")
            .Select(Entry.FromXml)
            .ToArray();

        return new Section(name, entries);
    }

    /// <summary>
    /// Generates grid divided into specified number of columns.
    /// </summary>
    /// <param name="columns">
    /// Number of columns, which generated grid shall contain.
    /// </param>
    /// <returns>
    /// New grid divided into specified number of columns.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown ,when value of at least one argument will be considered as invalid.
    /// </exception>
    private static Grid GenerateGrid(int columns)
    {
        if(columns <= 0)
        {
            string argumentName = nameof(columns);
            string errorMessage = $"Number of grid columns shall be greater than zero: {columns}";
            throw new ArgumentOutOfRangeException(argumentName, columns, errorMessage);
        }
        
        var grid = new Grid();

        while (grid.ColumnDefinitions.Count() < columns)
        {
            var newColumnDefinition = new ColumnDefinition();
            grid.ColumnDefinitions.Add(newColumnDefinition);
        }

        return grid;
    }

    /// <summary>
    /// Creates a new section instance..
    /// </summary>
    /// <param name="name">
    /// Name, which shall be assigned to created section.
    /// </param>
    /// <param name="entries">
    /// Collection of entries, which shall be contained by created section.
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
    private Section(string name, IEnumerable<Entry> entries)
    {
        if (name is null)
        {
            string argumentName = nameof(name);
            const string ErrorMessage = "Provided section name is a null reference:";
            throw new ArgumentNullException(argumentName, ErrorMessage);
        }

        if (name == string.Empty)
        {
            string argumentName = nameof(name);
            const string ErrorMessage = "Provided section name is an empty string:";
            throw new ArgumentOutOfRangeException(argumentName, name, ErrorMessage);
        }

        if (entries is null)
        {
            string argumentName = nameof(entries);
            const string ErrorMessage = "Provided entries collection is a null reference:";
            throw new ArgumentNullException(argumentName, ErrorMessage);
        }

        if (entries.Contains(null))
        {
            string argumentName = nameof(entries);
            const string ErrorMessage = "Provided entries collection contains a null reference:";
            throw new ArgumentException(argumentName, ErrorMessage);
        }

        Header = name;

        _grid = GenerateGrid(2);

        AddChild(_grid);
        entries.ToList().ForEach(AddEntry);
    }

    /// <summary>
    /// Adds a new row to section grid.
    /// </summary>
    /// <returns>
    /// Index of added row.
    /// </returns>
    private int AddRow()
    {
        var rowDefinition = new RowDefinition();
        _grid.RowDefinitions.Add(rowDefinition);

        return _grid.RowDefinitions.Count() - 1;
    }

    /// <summary>
    /// Adds provided entry to section grid.
    /// </summary>
    /// <param name="entry">
    /// Entry, which shall be added to section grid.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown, when at least one reference-type argument is a null reference.
    /// </exception>
    private void AddEntry(Entry entry)
    {
        if (entry is null)
        {
            string argumentName = nameof(entry);
            const string ErrorMessage = "Provided entry is a null reference:";
            throw new ArgumentNullException(argumentName, ErrorMessage);
        }

        int rowIndex = AddRow();

        Grid.SetRow(entry.Label, rowIndex);
        Grid.SetColumn(entry.Label, 0);
        _grid.Children.Add(entry.Label);

        Grid.SetRow(entry.Value, rowIndex);
        Grid.SetColumn(entry.Value, 1);
        _grid.Children.Add(entry.Value);
    }
    #endregion
}
