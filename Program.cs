/* Implementera följande funktioner:
 * [X] Lägg till nya böcker i bibliotek
 * [X] Låna ut böcker till låntagare
 * [X] Återlämna böcker
 * [X] Visa tillgängliga böcker
 * [] Visa låntagare och deras lånade böcker

Använd listor och/eller andra lämpliga datastrukturer/filer för att hantera samlingar av böcker och låntagare.

Skapa ett användargränssnitt (konsol-baserat eller GUI) där bibliotekspersonal kan interagera med programmet och utföra ovanstående åtgärder. Skapa en användarvänlig och intuitiv design för användargränssnittet.

Se till att programmet följer god praxis för objektorienterad programmering inklusive inkapsling och  arv.

Testa programmet genom att låna ut och återlämna böcker samt visa korrekt information om tillgängliga böcker och låntagare.

*/
//TODO: dela upp library i minst två klasser
//TODO: hur implementerar vi arv?
//TODO: spara data
//TODO: att man kan komma ur val genom att t.ex. trycka på esc.
//TODO:Hantera exceptions och liknande
//TODO: innan man lägger till bok eller låntagare, bekfräfta rätt uppgifter
//TODO: lägg till en funktion som gör det möjligt att redigera uppgifter
//TODO: gå igenom att public, private, static osv är korrekt
//TODO: lägg till kommentarer

using System;
using System.Globalization;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;

Library myLibrary = new Library();
UI ui = new UI(myLibrary);
ui.MainMenu();

public class Book
{
    public readonly string title;
    public readonly string author;
    public readonly int publishedYear;
    public Borrower BorrowedTo { get; private set; } = null;
    public bool IsBorrowed { get; set; } = false;

    public Book(string titel, string author, int publishedYear)
    {
        this.title = titel;
        this.author = author;
        this.publishedYear = publishedYear;
    }

    public void BorrowBook(Borrower borrower)
    {
        if (!IsBorrowed)
        {
            BorrowedTo = borrower;
            IsBorrowed = true;
        }
    }
    public void Return()
    {
        BorrowedTo = null;
        IsBorrowed = false;
    }
    /// <summary>
    /// Returns a string with the title, author, publishing year and availablity of the book
    /// </summary>
    public string PrintOut()
    {
        string bookStatus;
        if (this.IsBorrowed == true)
        {
            bookStatus = "Unavailable";
        }
        else
        {
            bookStatus = "Available";
        }
        //TODO: Snygga till formatet.
        return ($"\tTitle:{this.title}\tAuthor: {this.author}\tPublished: {this.publishedYear}\tStatus: {bookStatus}");
    }
}

public class BookHandling
{

}

public class Borrower
{
    public string FirstName { get; init; }
    public string LastName { get; init; }
    public long socialSecurityNumber { get; init; }
    List<Book> borrowedBooks = new List<Book>();

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
}

public class BorrowerHandling
{

}
public class Library
{
    // Lista med samtliga böcker i biblioteket. Jag la till några färdiga böcker i biblioteket.
    List<Book> allLibraryBooks = new List<Book>()
    {
        new Book("A Tale of Two Cities", "Charles Dickens", 1859),
        new Book("The Little Prince", "Antoine de Saint-Exupéry", 1943),
        new Book("The Lord of the Rings", "J.R.R. Tolkien", 1955),
        new Book("Harry Potter and the Philosopher's Stone", "J. K. Rowling", 1997)
    };

    List<Borrower> allLibraryBorrowers = new List<Borrower>()
    {
        new Borrower("Test", "Testare", 198705291410)
    };

    private static void PressAKeyToContinue()
    {
        Console.WriteLine();
        Console.WriteLine("Please press a key to continue.");
        Console.ReadKey();
        Console.Clear();
    }
    /// <summary>
    /// Asks the user for number. Validates if correct number and returns a book from a list depending on the number.
    /// </summary>
    /// <param name="listOfBooks"></param>
    /// <param name="input"></param>
    /// <returns></returns>
    private Book SelectABookFromAList(List<Book> listOfBooks, string input)
    {
        while (true)
        {
            Console.WriteLine($"Enter the number of the book you want to {input}:");
            if (int.TryParse(Console.ReadLine(), out int bookNumber) && bookNumber >= 1 && bookNumber <= listOfBooks.Count)
            {
                return listOfBooks[bookNumber - 1];
            }
            else
            {
                return null;
            }
        }
    }

    #region Methods that handle borrowers in the library
    public void AddNewBorrower()
    {
        Console.WriteLine("You are about to add a new user.");
        Console.Write("Please enter borrowers first name:");
        string firstName = Console.ReadLine();
        Console.Write("Please enter borrowers last name:");
        string lastName = Console.ReadLine();
        long socialSecurityNumber = GetSocialSecurityNumber();

        if (BorrowerExist(socialSecurityNumber, out Borrower CurrentBorrower))
        {
            Console.WriteLine("A person with that social security number already exist.");
            Console.WriteLine("Please press a key to continue.");
            Console.ReadKey();
            Console.Clear();
        }
        else
        {
            allLibraryBorrowers.Add(new Borrower(firstName, lastName, socialSecurityNumber)); // Creates a new user and adds it to the list of Borrowers.
            Console.WriteLine($"{firstName} {lastName} added as a borrower to the library.");
            Console.WriteLine("Please press a key to continue.");
            Console.ReadKey();
            Console.Clear();

        }

    }

    private static long GetSocialSecurityNumber()
    {
        bool isValidInput = false;
        long socialSecurityNumber = 0;
        while (!isValidInput)
        {
            Console.Write("Please enter borrowers social security number (12 digits):");
            if (long.TryParse(Console.ReadLine(), out long userInput))
            {
                if (Borrower.IsValidSocialSecurityNumber(userInput))
                {
                    socialSecurityNumber = userInput;
                    isValidInput = true;
                }
                else
                {
                    Console.WriteLine("Not a valid number.");
                }
            }
            else
            {
                Console.WriteLine("Please enter digits only.");
            }
        }
        return socialSecurityNumber;
    }

    public bool BorrowerExist(long socialSecurityNumberToControll, out Borrower currentBorrower)
    {
        currentBorrower = null;
        foreach (Borrower borrower in allLibraryBorrowers)
        {
            if (borrower.socialSecurityNumber == socialSecurityNumberToControll)
            {
                currentBorrower = borrower;
                return true;
            }
        }
        return false;
    }
    #endregion

    #region Methods to handle adding book to the library
    internal void AddNewBook()
    {
        //TODO: Hantera null och tomma inputs
        Console.WriteLine("You are about to add a new book to the library.");
        Console.WriteLine("Please enter the title of the book:");
        string userInputTitle = Console.ReadLine();
        Book existingBook = null;
        bool bookTitleExist = false;

        if (DoesBookExist(userInputTitle, out existingBook))
        {
            while (true)
            {
                Console.WriteLine($"There is a book with the title '{existingBook.title}' by {existingBook.author} already existing in the library. Is this the book you have in mind? [y/n]");
                string userChoice = Console.ReadLine().ToLower();

                switch (userChoice)
                {
                    case "y":
                        PressAKeyToContinue();
                        return; // Exit the method
                    case "n":
                        bookTitleExist = true;
                        goto labelContinueHere;
                    default:
                        Console.WriteLine("Invalid choice!");
                        break;
                }
            }

        }

    labelContinueHere:
        Console.WriteLine("Please enter the author of the book");
        string userInputAuthor = Console.ReadLine();
        if (existingBook != null && userInputAuthor.ToLower() == existingBook.author.ToLower())
        {
            Console.WriteLine($"{existingBook.title} by {existingBook.author} already exists.");
            PressAKeyToContinue();
            return; // Exit the method
        }
        while (true)
        {
            Console.WriteLine("Please enter the publishing year of the book");
            if (Int32.TryParse(Console.ReadLine(), out int year))
            {
                if (validDate(year))
                {
                    allLibraryBooks.Add(new Book(userInputTitle, userInputAuthor, year));
                    Console.WriteLine($"{userInputTitle} by {userInputAuthor} has been added to the library.");
                    PressAKeyToContinue();
                    return;
                }
                else
                {
                    Console.WriteLine("Invalid year.");
                }
            }
            else { Console.WriteLine("Invalid input. Please enter digits only."); }
        }
    }

    /// <summary>
    /// Controlls that a year given in int-format is a a valid date and not set in the future.
    /// </summary>
    /// <param name="year"></param>
    /// <returns></returns>
    private bool validDate(int year)
    {

        if (year < DateTime.Now.Year)
        {
            return false;
        }
        return true;
    }
    private bool DoesBookExist(string userInputTitle, out Book foundBook)
    {
        foundBook = null;
        foreach (Book book in allLibraryBooks)
        {
            if (book.title.ToLower() == userInputTitle.ToLower())
            {
                foundBook = book;
                return true;
            }
        }
        return false;
    }
    #endregion

    #region Methods for loaning a book

    internal void LoanABookFromAList()
    {
        HandleLoaningABook(ListAllFreeBooks());
    }
    internal void SearchABookToLoan()
    {
        List<Book> listOfBooks = BookSearch();
        List<Book> listOfBooksFilteredByNotLoaned = new List<Book>();
        foreach (Book book in listOfBooks)
        {
            if (!book.IsBorrowed)
            {
                listOfBooksFilteredByNotLoaned.Add(book);
            }
        }
        HandleLoaningABook(listOfBooksFilteredByNotLoaned);
    }
    private void HandleLoaningABook(List<Book> books)
    {
        if (books.Count == 0)
        {
            Console.WriteLine("No available books at the moment.");
            PressAKeyToContinue();
            return;
        }

        DisplayBooks(books);

        // Ask the user to select a book to borrow
        Console.WriteLine("Enter the number of the book you want to borrow:");
        Book selectedBook = null;
        //TODO: LOOP?
        if (int.TryParse(Console.ReadLine(), out int bookNumber) && bookNumber >= 1 && bookNumber <= books.Count)
        {
            selectedBook = books[bookNumber - 1];
            //TODO: TA BORT
            //selectedBook.BorrowBook(borrower);
            //Console.WriteLine($"{selectedBook.title} has been borrowed by {borrower.FirstName} {borrower.LastName}.");
        }
        else
        {
            Console.WriteLine("Invalid book selection.");
        }

        long socialSecurityNumber = GetSocialSecurityNumber();
        if (BorrowerExist(socialSecurityNumber, out Borrower currentBorrower))
        {
            selectedBook.BorrowBook(currentBorrower);
            Console.WriteLine($"{selectedBook.title} has been borrowed by {currentBorrower.FirstName} {currentBorrower.LastName}.");
            PressAKeyToContinue();
        }
        else
        {
            Console.WriteLine("Borrower not found or not valid. Please create a valid borrower first.");
            PressAKeyToContinue();
        }
    }
    #endregion

    #region Methods for returning a book
    internal void ReturnABookFromAList()
    {
        List<Book> listOfBorrowedBooks = ListAllBorrowedBooks();
        HandleReturningABook(listOfBorrowedBooks);
    }

    internal void SearchABookToReturn()
    {
        List<Book> listOfBooks = BookSearch();
        List<Book> listOfBooksFilteredByLoaned = new List<Book>();
        foreach (Book book in listOfBooks)
        {
            if (book.IsBorrowed)
            {
                listOfBooksFilteredByLoaned.Add(book);
            }
        }
        HandleReturningABook(listOfBooksFilteredByLoaned);
    }
    private void HandleReturningABook(List<Book> bookList)
    {
        if (bookList.Count == 0)
        {
            Console.Clear();
            Console.WriteLine("No books found");
            return;
        }

        DisplayBooks(bookList);

        Book userChoiceOfBook = SelectABookFromAList(bookList, "return");
        userChoiceOfBook.Return();
        Console.WriteLine($"{userChoiceOfBook.title} by {userChoiceOfBook.author} has been returned.");
    }
    #endregion

    #region Methods used to display books
    /// <summary>
    /// Ask the user for a search word. If a matching book exist, the books are printed out.
    /// </summary>
    internal void FindABook()
    {

        List<Book> foundBooks = BookSearch();

        if (foundBooks == null)
        {
            Console.WriteLine("No matches found");
        }
        else
        {
            Console.WriteLine("Matches found:\n");

            DisplayBooks(foundBooks);
            PressAKeyToContinue();
        }


    }
    internal void PrintAllBooks()
    {
        DisplayBooks(allLibraryBooks);
        PressAKeyToContinue();
    }
    /// <summary>
    /// Asks the user of a search word and returns a list of books containing that word.
    /// </summary>
    /// <returns></returns>
    internal List<Book> BookSearch()
    {
        Console.Clear();
        Console.WriteLine("Please enter a search word:");
        string searchWord = Console.ReadLine();

        List<Book> results = allLibraryBooks.
            Where(book => book.title.Contains(searchWord, StringComparison.OrdinalIgnoreCase) ||
            book.author.Contains(searchWord, StringComparison.OrdinalIgnoreCase))
            .ToList();
        return results;
    }

    /// <summary>
    /// Creates a list with only books that are borrowed
    /// </summary>
    /// <returns></returns>
    internal List<Book> ListAllBorrowedBooks()
    {
        List<Book> borrowedBooks = new List<Book>();

        foreach (Book book in allLibraryBooks)
        {
            if (book.IsBorrowed)
            {
                borrowedBooks.Add(book);
            }
        }

        return borrowedBooks;
    }

    /// <summary>
    /// Creates a list with only books that are free for loan.
    /// </summary>
    /// <returns></returns>
    internal List<Book> ListAllFreeBooks()
    {
        List<Book> freeBooks = new List<Book>();
        foreach (Book book in allLibraryBooks)
        {
            if (book.IsBorrowed == false)
            {
                freeBooks.Add(book);
            }
        }
        return freeBooks;
    }
    private void DisplayBooks(List<Book> books)
    {
        for (int i = 0; i < books.Count; i++)
        {
            Console.WriteLine($"{i + 1}: {books[i].PrintOut()}");
        }
    }
    #endregion
}
public class UI
{
    private Library library;
    public UI(Library library)
    {
        this.library = library;
    }
    public void MainMenu()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("This is the main menu.");
            Console.WriteLine("1 - Add new borrower or book to the library");
            Console.WriteLine("2 - Find a book");
            Console.WriteLine("3 - Loan out a book");
            Console.WriteLine("4 - Return a book");
            Console.WriteLine("5 - Close program");

            string userChoice = Console.ReadLine();
            switch (userChoice)
            {
                case "1":
                    AddItemMenu();
                    break;
                case "2":
                    SearchABookMenu();
                    break;
                case "3":
                    LoanBookMenu();
                    break;
                case "4":
                    ReturnBookMenu();
                    break;
                case "5":
                    Environment.Exit(0);
                    break;
                default:
                    Console.WriteLine("Invalid choice");
                    break;
            }
        }
    }

    private void AddItemMenu()
    {
        Console.Clear();
        while (true)
        {
            Console.WriteLine("Please choose an item to add:");
            Console.WriteLine();
            Console.WriteLine("1 - New borrower");
            Console.WriteLine();
            Console.WriteLine("2 - New book");
            Console.WriteLine();
            Console.WriteLine("3 - Return to main menu");

            string userchoice = Console.ReadLine();

            switch (userchoice)
            {
                case "1":
                    library.AddNewBorrower();
                    break;
                case "2":
                    library.AddNewBook();
                    break;
                case "3": return;
                default: Console.WriteLine("Invalid choice"); break;
            }
        }
    }
    private void SearchABookMenu()
    {
        Console.Clear();
        while (true)
        {
            Console.WriteLine("How do you want to find a book:");
            Console.WriteLine();
            Console.WriteLine("1 - Search for the book");
            Console.WriteLine();
            Console.WriteLine("2 - List all books");
            Console.WriteLine();
            Console.WriteLine("3 - Return to main menu");

            string userchoice = Console.ReadLine();

            switch (userchoice)
            {
                case "1":
                    library.FindABook();
                    break;
                case "2":
                    library.PrintAllBooks();
                    break;
                case "3": return;
                default: Console.WriteLine("Invalid choice"); break;
            }
        }
    }
    private void LoanBookMenu()
    {
        Console.Clear();
        while (true)
        {
            Console.WriteLine("You are about loan a book.");
            Console.WriteLine("Do you want to:");
            Console.WriteLine();
            Console.WriteLine("1. List all free books to loan");
            Console.WriteLine();
            Console.WriteLine("2. Search for a free book to loan");
            Console.WriteLine();
            Console.WriteLine("3. Return to main menu");

            string userchoice = Console.ReadLine();

            switch (userchoice)
            {
                case "1":
                    library.LoanABookFromAList();
                    break;
                case "2":
                    library.SearchABookToLoan();
                    break;
                case "3": return; ;
                default: Console.WriteLine("Invalid choice"); break;
            }
        }


    }
    private void ReturnBookMenu()
    {
        Console.Clear();
        while (true)
        {
            Console.WriteLine("You are about to return a loaned book.");
            Console.WriteLine("Do you want to:");
            Console.WriteLine("1. List all loaned out books");
            Console.WriteLine();
            Console.WriteLine("2. Search of for the book to return");
            Console.WriteLine();
            Console.WriteLine("3. Return to main menu");

            string userchoice = Console.ReadLine();

            switch (userchoice)
            {
                case "1":
                    library.ReturnABookFromAList();
                    break;
                case "2":
                    library.SearchABookToReturn();
                    break;
                case "3": return; ;
                default: Console.WriteLine("Invalid choice"); break;
            }
        }


    }
}

