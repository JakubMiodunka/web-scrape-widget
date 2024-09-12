using System.Globalization;
using WebScrapeWidget.Utilities;

namespace WebScrapeWidgetTests.Utilities;


/// <summary>
/// Test fixture for WebScrapeWidgetTests.Utilities.CultureContext;
/// </summary>
/// TODO: Re-factor.
[Author("Jakub Miodunka")]
[TestOf(typeof(CultureContext))]
public sealed class CultureContextTests
{
    #region Properties
    private Random _randomNumberGenerator;
    #endregion

    #region Auxiliary methods
    /// <summary>
    /// Returns random culture drawn from the pool of all available cultures.
    /// </summary>
    /// <param name="ignore">
    /// Collection of cultures, which shall be excluded from pool of cultures
    /// from which the draw will be made.
    /// </param>
    /// <returns>
    /// Random culture drawn from the pool of specified cultures.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown, when at least one reference-type argument is a null reference.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown, when at least one argument will be considered as invalid.
    /// </exception>
    private CultureInfo GetRandomCulture(params CultureInfo[] ignore)
    {
        if (ignore is null)
        {
            string argumentName = nameof(ignore);
            const string ErrorMessage = "Provided ignore list is a null reference:";
            throw new ArgumentNullException(argumentName, ErrorMessage);
        }

        if (ignore.Contains(null))
        {
            string argumentName = nameof(ignore);
            const string ErrorMessage = "Provided ignore list contains a null reference:";
            throw new ArgumentException(argumentName, ErrorMessage);
        }

        string[] culturesToIgnore = ignore
            .Select(culture => culture.Name)
            .ToArray();
        
        CultureInfo[] poolOfCultures = CultureInfo
            .GetCultures(CultureTypes.AllCultures)
            .Where(culture => !culturesToIgnore.Contains(culture.Name))
            .ToArray();

        int randomPoolIndex = _randomNumberGenerator.Next(poolOfCultures.Length);

        return poolOfCultures.ElementAt(randomPoolIndex);
    }

    /// <summary>
    /// Returns random culture drawn from the pool of all available cultures.
    /// </summary>
    /// <param name="ignore">
    /// Collection of culture names, which shall be excluded from pool of cultures
    /// from which the draw will be made.
    /// </param>
    /// <returns>
    /// Random culture drawn from the pool of specified cultures.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown, when at least one reference-type argument is a null reference.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown, when at least one argument will be considered as invalid.
    /// </exception>
    private CultureInfo GetRandomCulture(params string[] ignore)
    {
        if (ignore is null)
        {
            string argumentName = nameof(ignore);
            const string ErrorMessage = "Provided ignore list is a null reference:";
            throw new ArgumentNullException(argumentName, ErrorMessage);
        }

        if (ignore.Contains(null))
        {
            string argumentName = nameof(ignore);
            const string ErrorMessage = "Provided ignore list contains a null reference:";
            throw new ArgumentException(argumentName, ErrorMessage);
        }

        CultureInfo[] culturesToIgnore = ignore
            .Select(CultureInfo.GetCultureInfo)
            .ToArray();

        return GetRandomCulture(culturesToIgnore);
    }
    #endregion

    #region Setup
    /// <summary>
    /// Fixture initialization method called once prior to executing any of the tests.
    /// </summary>
    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _randomNumberGenerator = new Random();
    }
    #endregion

    #region Properties tests
    /// <summary>
    /// Checks if CultureContext.OriginalCulture property is set to culture
    /// used before swap.
    /// </summary>
    [Test]
    public void OriginalCulturePropertyTest()
    {
        string originalCultureName = CultureInfo.CurrentCulture.Name;
        CultureInfo swappedCulture = GetRandomCulture(ignore: originalCultureName);

        using (var cultureContex = new CultureContext(swappedCulture))
        {
            Assert.IsTrue(cultureContex.OriginalCulture.Name == originalCultureName);
        }
    }

    /// <summary>
    /// Checks if CultureContext.SwappedCulture property is set to culture
    /// used for swap.
    /// </summary>
    [Test]
    public void SwappedCulturePropertyTest()
    {
        var originalCulture = CultureInfo.CurrentCulture;
        string swappedCultureName = GetRandomCulture(ignore: originalCulture).Name;

        using (var cultureContex = CultureContext.FromCultureName(swappedCultureName))
        {
            Assert.IsTrue(cultureContex.SwappedCulture.Name == swappedCultureName);
        }
    }
    #endregion

    #region Functionalities tests
    /// <summary>
    /// Tests if currently used culture will be temporary swapped for specified culture instance
    /// and restored when CultureContext instance will be disposed.
    /// </summary>
    [Test]
    public void CultureSwapUsingCultureInstanceTest()
    {
        var originalCulture = CultureInfo.CurrentCulture;
        CultureInfo swappedCulture = GetRandomCulture(ignore: originalCulture);

        using (var cultureContex = new CultureContext(swappedCulture))
        {
            Assert.IsTrue(CultureInfo.CurrentCulture.Name == swappedCulture.Name);
        }

        Assert.IsTrue(CultureInfo.CurrentCulture.Name == originalCulture.Name);
    }

    /// <summary>
    /// Tests if currently used culture will be temporary swapped for culture with specified name
    /// and restored when CultureContext instance will be disposed.
    /// </summary>
    [Test]
    public void CultureSwapUsingCultureNameTest()
    {
        string originalCultureName = CultureInfo.CurrentCulture.Name;
        string swappedCultureName = GetRandomCulture(ignore: originalCultureName).Name;

        using (var cultureContex = CultureContext.FromCultureName(swappedCultureName))
        {
            Assert.IsTrue(CultureInfo.CurrentCulture.Name == swappedCultureName);
        }

        Assert.IsTrue(CultureInfo.CurrentCulture.Name == originalCultureName);
    }
    #endregion
}
