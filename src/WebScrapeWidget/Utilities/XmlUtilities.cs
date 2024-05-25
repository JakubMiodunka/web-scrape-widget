using System.Xml.Linq;
using System.Xml.Schema;

namespace WebScrapeWidget.Utilities;

public static class XmlUtilities
{
    public static bool ValidateAgainstSchema(string xmlFile, string xmlSchema)
    {
        // TODO: Add files existence and extensions checks.
        
        var schemaSet = new XmlSchemaSet();
        schemaSet.Add(null, xmlSchema);
        
        var xmlDocument = XDocument.Load(xmlFile);

        bool isDocumentValid = true;
        xmlDocument.Validate(schemaSet, (sender, eventData) => isDocumentValid = false);

        return isDocumentValid;
    }
}
