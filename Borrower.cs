using System.Globalization;

/// <summary>
/// Represents a borrower in the library system with basic information and a list of borrowed books.
/// </summary>
public class Borrower
{
    /// <summary>
    /// Represents a borrower in the library system with basic information and a list of borrowed books.
    /// </summary>
    public string FirstName { get; init; }

    /// <summary>
    /// Gets or initializes the last name of the borrower.
    /// </summary>
    public string LastName { get; init; }

    /// <summary>
    /// Gets or initializes the social security number of the borrower.
    /// </summary>
    public long socialSecurityNumber { get; init; }

    //VARFÖR INGEN REFERENS NÄR DEN BÖR HA DET?? Den finns t.ex. i DisplayBorrower()-metoden i BorrowerHandling-klassen.
    /// <summary>
    /// Gets a list of book IDs that the borrower has borrowed.
    /// </summary>
    public List<int> borrowedBooksByID = new List<int>();

    /// <summary>
    /// Initializes a new instance of the Borrower class with provided information.
    /// </summary>
    /// <param name="firstName">The first name of the borrower.</param>
    /// <param name="lastName">The last name of the borrower.</param>
    /// <param name="socialSecurityNumber">The social security number of the borrower.</param>
    public Borrower(string firstName, string lastName, long socialSecurityNumber)
    {
        FirstName = firstName;
        LastName = lastName;
        this.socialSecurityNumber = socialSecurityNumber;
    }

    /// <summary>
    /// Validates if the given social security number is in a valid format and represents a reasonable birthdate.
    /// </summary>
    /// <param name="socialSecurityNumber">The social security number to validate.</param>
    /// <returns>True if the social security number is valid; otherwise, false.</returns>
    public static bool IsValidSocialSecurityNumber(long socialSecurityNumber)
    {
        //Converts the int number to a string.
        string ssnString = socialSecurityNumber.ToString();

        // Validates that the number is 12 digits
        if (!(ssnString.Length == 12))
        {
            return false;
        }

        // Removes the 4 last digits.
        ssnString = ssnString.Substring(0, 8);

        // Validates if the string can be converted to a DateTime.
        DateTime parsedDate;
        if (!(DateTime.TryParseExact(ssnString, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDate))) // Validates if the string can be converted to a string.
        {
            return false;
        }

        // Validates that the given birthday is not in the future and within reasonable time.
        if ((parsedDate < DateTime.Now) && (parsedDate > new DateTime(1900, 01, 01)))
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Generates a formatted string representing the borrower's information.
    /// </summary>
    /// <returns>A formatted string with the borrower's name and social security number.</returns>
    public string PrintOut()
    {

        return ($"{this.FirstName} {this.LastName}, SSN: {this.socialSecurityNumber.ToString()}");

    }
}
