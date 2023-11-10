//TODO: hur implementerar vi arv?
//TODO:Hantera exceptions och liknande
//TODO: innan man lägger till bok eller låntagare, bekfräfta rätt uppgifter NEJ
//TODO: lägg till en funktion som gör det möjligt att redigera uppgifter NEJ
//TODO: gå igenom att public, private, static osv är korrekt
//TODO: lägg till kommentarer

// Kommentar till arv. Klassen borrower och book har flertal likheter, t.ex. metoden PrintOut() och sättet programmet spar ner data i Json format. Här ser jag en möjlighet att implementera ett interface
// för att möjliggöra att båda klasserna delar på gemensamma metoder.

using System.Globalization;

public class Borrower
{
    public string FirstName { get; init; }
    public string LastName { get; init; }
    public long socialSecurityNumber { get; init; }
    public List<int> borrowedBooksByID = new List<int>();

    public Borrower(string firstName, string lastName, long socialSecurityNumber)
    {
        FirstName = firstName;
        LastName = lastName;
        this.socialSecurityNumber = socialSecurityNumber;
    }

    /// <summary>
    /// Takes a number and controlls if is valid. Returns true if valid social security number.
    /// </summary>
    /// <param name="socialSecurityNumber"></param>
    /// <returns></returns>
    public static bool IsValidSocialSecurityNumber(long socialSecurityNumber)
    {

        string ssnString = socialSecurityNumber.ToString(); //Converts the int number to a string.
        if (!(ssnString.Length == 12)) // Validates that the number is 12 digits
        {
            return false;
        }
        ssnString = ssnString.Substring(0, 8); // Removes the 4 last digits.

        DateTime parsedDate; //Var that saves a successfully converted DateTime
        if (!(DateTime.TryParseExact(ssnString, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDate))) // Validates if the string can be converted to a string.
        {
            return false;
        }

        if ((parsedDate < DateTime.Now) && (parsedDate > new DateTime(1900, 01, 01))) // Validates that the given birthday is not in the future and within reasoneble time.
        {
            return true;
        }

        return false;
    }

    public void ReturnBook()
    {

    }

    public string PrintOut()
    {

        return ($"{this.FirstName} {this.LastName}, SSN: {this.socialSecurityNumber.ToString()}");

    }
}
