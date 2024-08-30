using System.IO;
using System.Xml.Linq;
using System.Xml.Schema;


namespace WebScrapeWidget.Utilities;

public static class FileSystemUtilities
{
    /// <summary>
    /// Checks if given file system entry exists and is a directory.
    /// If either of above cases would not be fulfilled, according exception will be thrown.
    /// </summary>
    /// <param name="path">
    /// Path to file system entry, which shall be validated.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown, when at least one reference-type argument is a null reference.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown, when value of at least one argument will be considered as invalid.
    /// </exception>
    /// <exception cref="DirectoryNotFoundException">
    /// Thrown, when provided file system entry does not exist in file system as directory.
    /// </exception>
    public static void ValidateDirectory(string path)
    {
        if (path is null)
        {
            string argumentName = nameof(path);
            const string ErrorMessage = "Entry path is a null reference:";
            throw new ArgumentNullException(argumentName, ErrorMessage);
        }

        if (path == string.Empty)
        {
            string argumentName = nameof(path);
            string errorMessage = $"Entry path is an empty string: {path}";
            throw new ArgumentOutOfRangeException(argumentName, path, errorMessage);
        }

        if (!Directory.Exists(path))
        {
            if (File.Exists(path))
            {
                string errorMessage = $"Given entry is a file: {path}";
                throw new DirectoryNotFoundException(errorMessage);
            }
            else
            {
                string errorMessage = $"Directory does not exist: {path}";
                throw new DirectoryNotFoundException(errorMessage);
            }
        }
    }

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
    /// <param name="shallExist">
    /// Specifies if provided file system entry shall exists within file system as file.
    /// Useful, when there is a need to check if specified path is not already
    /// in use by other file in file system.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown, when at least one reference-type argument is a null reference.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown, when value of at least one argument will be considered as invalid.
    /// </exception>
    /// <exception cref="FileNotFoundException">
    /// Thrown, when provided file system entry does not exist in file system as file.
    /// </exception>
    /// <exception cref="IOException">
    /// Thrown, either when provided file system entry has invalid extension
    /// or it exists within file system but shall not.
    /// </exception>
    public static void ValidateFile(string filePath, string? expectedExtension = null, bool shallExist = true)
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

        if (shallExist)
        {
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
        }
        else
        {
            if (File.Exists(filePath))
            {
                string errorMessage = $"File already exist: {filePath}";
                throw new IOException(errorMessage);
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
