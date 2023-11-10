//TODO: hur implementerar vi arv?
//TODO:Hantera exceptions och liknande
//TODO: innan man lägger till bok eller låntagare, bekfräfta rätt uppgifter NEJ
//TODO: lägg till en funktion som gör det möjligt att redigera uppgifter NEJ
//TODO: gå igenom att public, private, static osv är korrekt
//TODO: lägg till kommentarer

// Kommentar till arv. Klassen borrower och book har flertal likheter, t.ex. metoden PrintOut() och sättet programmet spar ner data i Json format. Här ser jag en möjlighet att implementera ett interface
// för att möjliggöra att båda klasserna delar på gemensamma metoder.

using Newtonsoft.Json;

public class DataRepository
{
    private const string BorrowersFileName = "borrowers.json";
    private const string BooksFileName = "books.json";

    public void SaveBorrowersToFile(List<Borrower> borrowers)
    {
        var json = JsonConvert.SerializeObject(borrowers);
        File.WriteAllText(BorrowersFileName, json);
    }
    public void SaveBooksToFile(List<Book> books)
    {
        var json = JsonConvert.SerializeObject(books);
        File.WriteAllText(BooksFileName, json);
    }

    public List<Borrower> LoadBorrowersFromFile()
    {
        if (File.Exists(BorrowersFileName))
        {
            var json = File.ReadAllText(BorrowersFileName);
            return JsonConvert.DeserializeObject<List<Borrower>>(json);
        }
        return new List<Borrower>()
        {
            new Borrower("Test", "Testare", 111111111111),
            new Borrower("Test2", "Testare2", 222222222222),
            new Borrower("Henri", "Lehtonen", 198705291111)
        };
    }

    public List<Book> LoadBooksFromFile()
    {
        if (File.Exists(BooksFileName))
        {
            var json = File.ReadAllText(BooksFileName);
            return JsonConvert.DeserializeObject<List<Book>>(json);
        }
        return new List<Book>()
        {
        new Book("A Tale of Two Cities", "Charles Dickens", 1859, 1),
        new Book("The Little Prince", "Antoine de Saint-Exupéry", 1943, 2),
        new Book("The Lord of the Rings", "J.R.R. Tolkien", 1955, 3),
        new Book("Harry Potter and the Philosopher's Stone", "J. K. Rowling", 1997, 4)
        };
    }
}
