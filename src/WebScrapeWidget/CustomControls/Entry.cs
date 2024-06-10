using System.Xml.Linq;
using System.Windows.Controls;


namespace WebScrapeWidget.CustomControls;

public sealed class Entry
{
    #region Properties
    public readonly TextBlock Label;
    public readonly TextBlock Value;
    #endregion

    #region Class instantiation
    public static Entry FromXml(XElement entryElement)
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

        return new Entry(label);
    }

    private Entry(string label)
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

        Label = new TextBlock();
        Label.Text = label;

        Value = new TextBlock();
        Value.Text = "value";   //TODO: Gather value from data source
    }
    #endregion
}
