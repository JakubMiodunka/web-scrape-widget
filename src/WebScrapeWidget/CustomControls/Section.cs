using System.Xml.Linq;
using System.Windows.Controls;


namespace WebScrapeWidget.CustomControls;

public sealed class Section : GroupBox
{
    #region Properties
    private readonly Grid _grid;
    #endregion

    #region Class instantiation
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

    private static Grid PrepareGrid()
    {
        const int columns = 2;

        var grid = new Grid();

        for (int i = 0; i < columns; i++)
        {
            var newColumnDefinition = new ColumnDefinition();
            grid.ColumnDefinitions.Add(newColumnDefinition);
        }

        return grid;
    }

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
        _grid = PrepareGrid();

        AddChild(_grid);
        entries.ToList().ForEach(AddEntry);
    }
  
    public void AddEntry(Entry entry)
    {
        if (entry is null)
        {
            string argumentName = nameof(entry);
            const string ErrorMessage = "Provided entry is a null reference:";
            throw new ArgumentNullException(argumentName, ErrorMessage);
        }

        var rowDefinition = new RowDefinition();
        _grid.RowDefinitions.Add(rowDefinition);

        int rowIndex = _grid.RowDefinitions.Count() - 1;

        Grid.SetRow(entry.Label, rowIndex);
        Grid.SetColumn(entry.Label, 0);
        _grid.Children.Add(entry.Label);

        Grid.SetRow(entry.Value, rowIndex);
        Grid.SetColumn(entry.Value, 1);
        _grid.Children.Add(entry.Value);
    }
    #endregion
}
