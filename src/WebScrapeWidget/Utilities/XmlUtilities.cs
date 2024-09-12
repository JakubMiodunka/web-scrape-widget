using System.Xml.Linq;
using System.Xml.Schema;


namespace WebScrapeWidget.Utilities;

public sealed class XmlUtilities
{
    /// <summary>
    /// Checks if given XML document matches specified schema.
    /// </summary>
    /// <param name="xmlDocument">
    /// XML document, which shall be validated.
    /// </param>
    /// <param name="schemaPath">
    /// Path to *.xsd schema, against which provided XML document shall be validated against.
    /// </param>
    /// <returns>
    /// True if provided XML document matches provided *.xsd schema, false otherwise.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown, when at least one reference-type argument is a null reference.
    /// </exception>
    public static bool ValidateXmlDocument(XDocument xmlDocument, string schemaPath)
    {
        if (xmlDocument is null)
        {
            string argumentName = nameof(xmlDocument);
            const string ErrorMessage = "Provided XML document is a null reference:";
            throw new ArgumentNullException(argumentName, ErrorMessage);
        }

        FileSystemUtilities.ValidateFile(schemaPath, ".xsd");

        var schemaSet = new XmlSchemaSet();
        schemaSet.Add(null, schemaPath);

        bool isDocumentValid = true;
        xmlDocument.Validate(schemaSet, (sender, eventData) => isDocumentValid = false);

        return isDocumentValid;
    }
}
