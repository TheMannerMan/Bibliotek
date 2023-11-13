using Newtonsoft.Json;
/// <summary>
/// The DataRepository class provides methods for saving and loading data related to borrowers and books using JSON serialization.
/// </summary>
public class DataRepository
{
    private const string BorrowersFileName = "borrowers.json";
    private const string BooksFileName = "books.json";

    /// <summary>
    /// Saves a list of borrowers to a JSON file.
    /// </summary>
    public void SaveBorrowersToFile(List<Borrower> borrowers)
    {
        // Serialize the list of borrowers to JSON and write it to a file.
        var json = JsonConvert.SerializeObject(borrowers);
        File.WriteAllText(BorrowersFileName, json);
    }

    /// <summary>
    /// Saves a list of books to a JSON file.
    /// </summary>
    public void SaveBooksToFile(List<Book> books)
    {
        // Serialize the list of books to JSON and write it to a file.
        var json = JsonConvert.SerializeObject(books);
        File.WriteAllText(BooksFileName, json);
    }

    /// <summary>
    /// Loads a list of borrowers from a JSON file.
    /// If the file doesn't exist, a default list of borrowers is provided.
    /// </summary>
    /// <returns>The list of borrowers loaded from the file or a default list if the file doesn't exist.</returns>
    public List<Borrower> LoadBorrowersFromFile()
    {
        if (File.Exists(BorrowersFileName))
        {
            // Read JSON from the file and deserialize it to a list of borrowers.
            var json = File.ReadAllText(BorrowersFileName);
            return JsonConvert.DeserializeObject<List<Borrower>>(json);
        }

        // Default list of borrowers if the file doesn't exist.
        return new List<Borrower>()
        {
            new Borrower("Test", "Testare", 111111111111),
            new Borrower("Henri", "Lehtonen", 198705291111)
        };
    }

    /// <summary>
    /// Loads a list of books from a JSON file.
    /// If the file doesn't exist, a default list of books is provided.
    /// </summary>
    /// <returns>The list of books loaded from the file or a default list if the file doesn't exist.</returns>
    public List<Book> LoadBooksFromFile()
    {
        if (File.Exists(BooksFileName))
        {
            // Read JSON from the file and deserialize it to a list of books.
            var json = File.ReadAllText(BooksFileName);
            return JsonConvert.DeserializeObject<List<Book>>(json);
        }

        // Default list of books if the file doesn't exist.
        return new List<Book>()
        {
        new Book("A Tale of Two Cities", "Charles Dickens", 1859, 1),
        new Book("The Little Prince", "Antoine de Saint-Exupéry", 1943, 2),
        new Book("The Lord of the Rings", "J.R.R. Tolkien", 1955, 3),
        new Book("Harry Potter and the Philosopher's Stone", "J. K. Rowling", 1997, 4)
        };
    }
}
