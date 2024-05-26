using System.Xml.Linq;
using System.Xml.Schema;

namespace WebScrapeWidget.Utilities;

public static class FileSystemUtilities
{
    /// <summary>
    /// Checks if given file system entry exists, is a file and its extension
    /// matches the expected one.
    /// If either of above cases would not be fulfilled, according exception will be thrown.
    /// </summary>
    /// <param name="filePath">
    /// Path to file system entry, which shall be validated.
    /// </param>
    /// <param name="expectedExtension">
    /// File extension, which shall be conceited as valid for validated file.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown, when at least one reference-type argument is a null reference.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown, when value of at least one argument will be considered as invalid.
    /// </exception>
    /// <exception cref="FileNotFoundException">
    /// Thrown, when given file system entry is not a file or does not exists.
    /// </exception>
    /// <exception cref="IOException">
    /// Thrown, when provided file system entry has invalid extension.
    /// </exception>
    public static void ValidateFile(string filePath, string? expectedExtension = null)
    {
        if (filePath is null)
        {
            string argumentName = nameof(filePath);
            const string ErrorMessage = "File path is a null reference:";
            throw new ArgumentNullException(argumentName, ErrorMessage);
        }

        if (filePath == string.Empty)
        {
            string argumentName = nameof(filePath);
            string errorMessage = $"File path is an empty string: {filePath}";
            throw new ArgumentOutOfRangeException(argumentName, filePath, errorMessage);
        }

        if (expectedExtension == string.Empty)
        {
            string argumentName = nameof(expectedExtension);
            string errorMessage = $"Expected file extension is an empty string: {filePath}";
            throw new ArgumentOutOfRangeException(argumentName, filePath, errorMessage);
        }

        if (!File.Exists(filePath))
        {
            if (Directory.Exists(filePath))
            {
                string errorMessage = $"Given entry is a directory: {filePath}";
                throw new FileNotFoundException(errorMessage, filePath);
            }
            else
            {
                string errorMessage = $"File does not exist: {filePath}";
                throw new FileNotFoundException(errorMessage, filePath);
            }
        }

        if (expectedExtension is not null)
        {
            string fileExtension = Path.GetExtension(filePath);

            if (fileExtension != expectedExtension)
            {
                string errorMessage = $"Invalid file extension: is '{fileExtension}', but shall be '{expectedExtension}'";
                throw new IOException(errorMessage);
            }
        }
    }

    /// <summary>
    /// Checks if given file system entry exists, is a XML file and matches specified schema.
    /// </summary>
    /// <remarks>
    /// If provided file system entry does not exist or is not *.xml file according exception will be thrown.
    /// If each mentioned criteria would be fulfilled, XML file will be validated against provided schema
    /// and validation result will be returned as bool value.
    /// </remarks>
    /// <param name="filePath">
    /// Path to file system entry, which shall be validated.
    /// </param>
    /// <param name="schemaPath">
    /// Path to *.xsd schema, against which checked file shall be validated against.
    /// </param>
    /// <returns>
    /// True, or false if provided XML file is valid, and matches provided *.xsd schema.
    /// </returns>
    public static bool ValidateXmlFile(string filePath, string schemaPath)
    {
        ValidateFile(filePath, ".xml");

        var schemaSet = new XmlSchemaSet();
        schemaSet.Add(null, schemaPath);

        var xmlDocument = XDocument.Load(filePath);

        bool isDocumentValid = true;
        xmlDocument.Validate(schemaSet, (sender, eventData) => isDocumentValid = false);

        return isDocumentValid;
    }
}
