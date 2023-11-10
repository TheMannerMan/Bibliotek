
/// <summary>
/// Represents a book in a library.
/// </summary>
public class Book
{
    //Properties for the book
    public string Title { get; }
    public string Author { get; }
    public int bookID { get; } // Unique ID for the book
    public bool IsBorrowed { get; private set; } = false; // Property to check if the book is borrowed or not

    //Fields for the book
    private readonly int publishedYear;
    private string borrowedBy = null; // Name of the borrower (null if not borrowed)

    // Constructor to create a new book with specified attributes
    public Book(string titel, string author, int publishedYear, int bookID)
    {
        this.Title = titel;
        this.Author = author;
        this.publishedYear = publishedYear;
        this.bookID = bookID;
    }

    /// <summary>
    /// Method to lend the book to a borrower.
    /// </summary>
    /// <param name="nameOfBorrower"></param>
    public void BorrowBook(string nameOfBorrower)
    {
        // Check if the book is already borrowed
        if (!IsBorrowed) 
        {
            // Update the book's loan status and the book's record of the borrower's name
            IsBorrowed = true;
            borrowedBy = nameOfBorrower;
        }
    }
    /// <summary>
    /// Method to return the book.
    /// </summary>
    public void Return()
    {
        // Check if the book is borrowed before attempting to return it
        if (IsBorrowed != false)
        {
            // Reset the book's status and clear the borrower's name
            IsBorrowed = false;
            borrowedBy = null;
        }
        else
        {
            // Notify the user that the book is not currently borrowed
            Console.WriteLine("Error: This book is not currently borrowed.");
        }
    }
    /// <summary>
    /// Returns a string with the title, author, publishing year and availablity of the book
    /// </summary>
    public string PrintOut()
    {
        // Create a string indicating the book's loan status
        string bookStatus;
        if (this.IsBorrowed == true)
        {
            bookStatus = $"On loan by {borrowedBy}";
        }
        else
        {
            bookStatus = "Free";
        }
        // Format and return the book information as a string
        return ($"{this.Title}".PadRight(45) + $"{this.Author}".PadRight(30) + $"{this.publishedYear}".PadRight(15) + $"{this.bookID}".PadRight(15) + $"{bookStatus}");

    }
}
