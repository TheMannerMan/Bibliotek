﻿/* Implementera följande funktioner:
 * [X] Lägg till nya böcker i bibliotek
 * [X] Låna ut böcker till låntagare
 * [X] Återlämna böcker
 * [X] Visa tillgängliga böcker
 * [X] Visa låntagare och deras lånade böcker

Använd listor och/eller andra lämpliga datastrukturer/filer för att hantera samlingar av böcker och låntagare.

Skapa ett användargränssnitt (konsol-baserat eller GUI) där bibliotekspersonal kan interagera med programmet och utföra ovanstående åtgärder. Skapa en användarvänlig och intuitiv design för användargränssnittet.

Se till att programmet följer god praxis för objektorienterad programmering inklusive inkapsling och  arv.

Testa programmet genom att låna ut och återlämna böcker samt visa korrekt information om tillgängliga böcker och låntagare.

*/

//TODO: hur implementerar vi arv?
//TODO: att man kan komma ur val genom att t.ex. trycka på esc.
//TODO:Hantera exceptions och liknande
//TODO: innan man lägger till bok eller låntagare, bekfräfta rätt uppgifter
//TODO: lägg till en funktion som gör det möjligt att redigera uppgifter
//TODO: gå igenom att public, private, static osv är korrekt
//TODO: lägg till kommentarer
//TODO: lägg till console title

// Kommentar till arv. Klassen borrower och book har flertal likheter, t.ex. metoden PrintOut() och sättet programmet spar ner data i Json format. Här ser jag en möjlighet att implementera ett interface
// för att möjliggöra att båda klasserna delar på gemensamma metoder.

using Newtonsoft.Json;
using System;
using System.ComponentModel.Design;
using System.Globalization;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;
using static System.Reflection.Metadata.BlobBuilder;


List<Book> allLibraryBooks = new List<Book>();
List<Borrower> allLibraryBorrowers = new List<Borrower>();


DataRepository repository = new DataRepository();
BorrowerHandling myBorrowerHandling = new BorrowerHandling(repository);
BookHandling myBookHandling = new BookHandling(myBorrowerHandling, repository);
UI ui = new UI(myBookHandling, myBorrowerHandling);

ui.MainMenu();

public class Book
{
    public string title;
    public string author;
    public int publishedYear;
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
            bookStatus = "On loan";
        }
        else
        {
            bookStatus = "Free";
        }
        //TODO: Snygga till formatet.
        return ($"\tTitle:{this.title}\tAuthor: {this.author}\tPublished: {this.publishedYear}\tLoan status: {bookStatus}");
    }
}

public class BookHandling
{
    private BorrowerHandling borrowerHandling;
    private DataRepository _dataRepository;
    public List<Book> allLibraryBooks;

    public BookHandling(BorrowerHandling borrowerHandling, DataRepository repository)
    {
        this.borrowerHandling = borrowerHandling;
        _dataRepository = repository;
        allLibraryBooks = repository.LoadBooksFromFile();
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

    public void SaveCurrentStatusOfBooks()
    {
        _dataRepository.SaveBooksToFile(this.allLibraryBooks);
    }

    #region Methods to handle adding book to the library
    internal void AddNewBook()
    {
        //TODO: Hantera null och tomma inputs
        Console.WriteLine("You are about to add a new book to the library.");
        Console.WriteLine("Please enter the title of the book:");
        string userInputTitle = UI.GetInputWithCancel();
        Console.WriteLine(); // To make a new line in console for better design.
        if (userInputTitle == null)
        {
            return;
        }
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
                        UI.PressAKeyToContinue();
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
        string userInputAuthor = UI.GetInputWithCancel();
        Console.WriteLine(); // To make a new line in console for better design.
        if (userInputAuthor == null)
        {
            return;
        }
        if (existingBook != null && userInputAuthor.ToLower() == existingBook.author.ToLower())
        {
            Console.WriteLine($"{existingBook.title} by {existingBook.author} already exists.");
            UI.PressAKeyToContinue();
            return; // Exit the method
        }
        while (true)
        {
            Console.WriteLine("Please enter the publishing year of the book");
            string year = UI.GetInputWithCancel();
            Console.WriteLine(); // To make a new line in console for better design.
            if (year == null)
            {
                return;
            }
            if (Int32.TryParse(year, out int parsedYear))
            {
                if (ValidDate(parsedYear))
                {
                    allLibraryBooks.Add(new Book(userInputTitle, userInputAuthor, parsedYear));
                    Console.WriteLine($"{userInputTitle} by {userInputAuthor} has been added to the library.");
                    UI.PressAKeyToContinue();
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
    private static bool ValidDate(int year)
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
            UI.PressAKeyToContinue();
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

        long socialSecurityNumber = BorrowerHandling.GetSocialSecurityNumber();
        if (borrowerHandling.BorrowerExist(socialSecurityNumber, out Borrower currentBorrower))
        {
            selectedBook.BorrowBook(currentBorrower);
            currentBorrower.borrowedBooks.Add(selectedBook);
            Console.WriteLine($"{selectedBook.title} has been borrowed by {currentBorrower.FirstName} {currentBorrower.LastName}.");
            UI.PressAKeyToContinue();
        }
        else
        {
            Console.WriteLine("Borrower not found or not valid. Please create a valid borrower first.");
            UI.PressAKeyToContinue();
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
        Borrower currentBorrower = userChoiceOfBook.BorrowedTo;
        userChoiceOfBook.Return();
        currentBorrower.borrowedBooks.Remove(userChoiceOfBook);
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
            UI.PressAKeyToContinue();
        }


    }
    internal void ListAllBooks()
    {
        DisplayBooks(allLibraryBooks);
        UI.PressAKeyToContinue();
    }
    private void DisplayBooks(List<Book> books)
    {
        for (int i = 0; i < books.Count; i++)
        {
            Console.WriteLine($"{i + 1}: {books[i].PrintOut()}");
        }
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

    #endregion
}

public class Borrower
{
    public string FirstName { get; init; }
    public string LastName { get; init; }
    public long socialSecurityNumber { get; init; }
    public List<Book> borrowedBooks = new List<Book>();

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

    public string PrintOut()
    {
        string message = "Books in loan:";
        if (borrowedBooks.Count == 0)
        {
            message += "\nNo books in loan.";
        }
        else
            foreach (Book book in borrowedBooks)
            {
                message += $"\n";
                message += book.PrintOut();
            }

        return ($"Borrower: {this.FirstName} {this.LastName}, social security number: {this.socialSecurityNumber.ToString()} \n" + message);

    }
}

public class BorrowerHandling
{
    List<Borrower> allLibraryBorrowers;
    private DataRepository _dataRepository;

    public BorrowerHandling(DataRepository dataRepository)
    {
        this.allLibraryBorrowers = dataRepository.LoadBorrowersFromFile();
        this._dataRepository = dataRepository;
    }

    public void SaveCurrentStatusOfBorrowers()
    {
        _dataRepository.SaveBorrowersToFile(this.allLibraryBorrowers);
    }

    #region Methods that handle borrowers in the library
    public void AddNewBorrower()
    {
        Console.WriteLine("You are about to add a new user.");
        Console.Write("Please enter borrowers first name:");
        string firstName = UI.GetInputWithCancel();
        Console.WriteLine(); // To make a new line in console for better design.
        if (firstName == null)
        {
            return; // User pressed 'esc', exit the method
        }
        Console.Write("Please enter borrowers last name:");
        string lastName = UI.GetInputWithCancel();
        Console.WriteLine(); // To make a new line in console for better design.
        if (lastName == null)
        {
            return; // User pressed 'esc', exit the method
        }
        long socialSecurityNumber = GetSocialSecurityNumber();
        Console.WriteLine(); // To make a new line in console for better design.

        if (socialSecurityNumber == 0)
        {
            return; // User pressed 'esc' in the GetSocialSecurityNumber() which returned '0'
        }

        else if (BorrowerExist(socialSecurityNumber, out Borrower CurrentBorrower))
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

    public static long GetSocialSecurityNumber()
    {
        bool isValidInput = false;
        long socialSecurityNumber = 0;
        while (!isValidInput)
        {
            Console.Write("Please enter borrowers social security number (12 digits):");
            string userInput = UI.GetInputWithCancel();

            if (userInput == null)
            {
                return 0; // User pressed 'esc', return 0 or any other action to indicate cancellation
            }

            if (long.TryParse(userInput, out long parsedUserInput))
            {
                if (Borrower.IsValidSocialSecurityNumber(parsedUserInput))
                {
                    socialSecurityNumber = parsedUserInput;
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

    internal void FindABorrower()
    {
        List<Borrower> foundBorrowers = BorrowerSearch();

        if (foundBorrowers == null)
        {
            Console.WriteLine("No mathes found");
        }
        else
        {
            Console.WriteLine("Mathes found: \n");

            DisplayBorrower(foundBorrowers);
            UI.PressAKeyToContinue();
        }
    }

    private void DisplayBorrower(List<Borrower> foundBorrowers)
    {
        for (int i = 0; i < foundBorrowers.Count; i++)
        {
            Console.WriteLine($"{i + 1}: {foundBorrowers[i].PrintOut()}");
        }
    }
    private List<Borrower> BorrowerSearch()
    {
        Console.Clear();
        Console.Write("Please enter a searchword: ");
        string searchWord = Console.ReadLine();

        List<Borrower> results = allLibraryBorrowers.
            Where(borrower => borrower.FirstName.Contains(searchWord, StringComparison.OrdinalIgnoreCase) ||
            borrower.LastName.Contains(searchWord, StringComparison.OrdinalIgnoreCase) ||
            borrower.socialSecurityNumber.ToString().Contains(searchWord, StringComparison.OrdinalIgnoreCase)).ToList();
        return results;
    }

    internal void ListAllBorrowers()
    {
        DisplayBorrower(allLibraryBorrowers);
        UI.PressAKeyToContinue();
    }
    #endregion
}
public class UI
{
    private BookHandling bookLibrary;
    private BorrowerHandling borrowerLibrary;

    public UI(BookHandling bookLibrary, BorrowerHandling borrowerLibrary)
    {
        this.bookLibrary = bookLibrary;
        this.borrowerLibrary = borrowerLibrary;
    }

    public void MainMenu()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("=================");
            Console.WriteLine("Library Main menu");
            Console.WriteLine("=================");
            Console.WriteLine("1 - Find a book");
            Console.WriteLine("2 - Lend out a book");
            Console.WriteLine("3 - Return a book");
            Console.WriteLine("4 - Manage books");
            Console.WriteLine("5 - Manage borrowers");
            Console.WriteLine("6 - Exit program");
            Console.WriteLine("=================");

            string userChoice = Console.ReadLine();
            switch (userChoice)
            {
                case "1":
                    SearchABookMenu();
                    break;
                case "2":
                    LoanBookMenu();
                    break;
                case "3":
                    ReturnBookMenu();
                    break;
                case "4":
                    ManageBooksMenu();
                    break;
                case "5":
                    ManageBorrowersMenu();
                    break;
                case "6":
                    bookLibrary.SaveCurrentStatusOfBooks();
                    borrowerLibrary.SaveCurrentStatusOfBorrowers();
                    Environment.Exit(0);
                    break;
                default:
                    Console.WriteLine("Invalid choice");
                    break;
            }
        }
    }
    private void SearchABookMenu()
    {
        Console.Clear();
        while (true)
        {
            Console.WriteLine("====================================");
            Console.WriteLine("Find a book menu");
            Console.WriteLine("====================================");
            Console.WriteLine();
            Console.WriteLine("1 - Search for a book");
            Console.WriteLine();
            Console.WriteLine("2 - List all books");
            Console.WriteLine();
            Console.WriteLine("3 - Return to main menu");
            Console.WriteLine();
            Console.WriteLine("====================================");
            ChooseAnOptionAndPressEnter();

            string userchoice = Console.ReadLine();

            switch (userchoice)
            {
                case "1":
                    bookLibrary.FindABook();
                    break;
                case "2":
                    bookLibrary.ListAllBooks();
                    break;
                case "3": return;
                default: Console.WriteLine("Invalid choice"); break;
            }
        }
    } // KLAR
    private void LoanBookMenu() //KLAR
    {
        Console.Clear();
        while (true)
        {
            Console.WriteLine("====================================");
            Console.WriteLine("Lend out a book menu");
            Console.WriteLine("====================================");
            Console.WriteLine();
            Console.WriteLine("1 - Search for a free book to loan");
            Console.WriteLine();
            Console.WriteLine("2 - List all free books to loan");
            Console.WriteLine();
            Console.WriteLine("3 - Return to main menu");
            Console.WriteLine();
            Console.WriteLine("====================================");
            ChooseAnOptionAndPressEnter();

            string userchoice = Console.ReadLine();

            switch (userchoice)
            {
                case "1":
                    bookLibrary.SearchABookToLoan();
                    break;
                case "2":
                    bookLibrary.LoanABookFromAList();
                    break;
                case "3": return; ;
                default: Console.WriteLine("Invalid choice"); break;
            }
        }


    }
    private void ReturnBookMenu() //KLAR
    {
        Console.Clear();
        while (true)
        {
            Console.WriteLine("====================================");
            Console.WriteLine("Return a book menu");
            Console.WriteLine("====================================");
            Console.WriteLine();
            Console.WriteLine("1. Show a list of all lend out books");
            Console.WriteLine();
            Console.WriteLine("2. Search of for the book to return");
            Console.WriteLine();
            Console.WriteLine("3. Return to main menu");
            Console.WriteLine();
            Console.WriteLine("====================================");
            ChooseAnOptionAndPressEnter();

            string userchoice = Console.ReadLine();

            switch (userchoice)
            {
                case "1":
                    bookLibrary.ReturnABookFromAList();
                    break;
                case "2":
                    bookLibrary.SearchABookToReturn();
                    break;
                case "3": return; ;
                default: Console.WriteLine("Invalid choice"); break;
            }
        }


    }
    private void ManageBooksMenu() // KLAR
    {
        while (true)
        {
            Console.WriteLine("====================================");
            Console.WriteLine("Manage books menu");
            Console.WriteLine("====================================");
            Console.WriteLine();
            Console.WriteLine("1 - Add a book to the library");
            Console.WriteLine();
            Console.WriteLine("2 - Return to main menu");
            Console.WriteLine();
            Console.WriteLine("====================================");
            ChooseAnOptionAndPressEnter();

            string userchoice = Console.ReadLine();

            switch (userchoice)
            {
                case "1":
                    bookLibrary.AddNewBook();
                    break;
                case "2": return;
                default:
                    Console.WriteLine("Invalid choice"); break;
            }
        }
    }
    private void ManageBorrowersMenu()// KLAR
    {
        Console.Clear();
        while (true)
        {
            Console.WriteLine("====================================");
            Console.WriteLine("Manage borrower");
            Console.WriteLine("====================================");
            Console.WriteLine();
            Console.WriteLine("1 - Search for a borrower ");
            Console.WriteLine();
            Console.WriteLine("2 - Show a list of current borrowers");
            Console.WriteLine();
            Console.WriteLine("3 - Add a borrower to the library");
            Console.WriteLine();
            Console.WriteLine("4 - Return to main menu");
            Console.WriteLine();
            Console.WriteLine("====================================");
            ChooseAnOptionAndPressEnter();

            string userchoice = Console.ReadLine();

            switch (userchoice)
            {
                case "1":
                    borrowerLibrary.FindABorrower();
                    break;
                case "2":
                    borrowerLibrary.ListAllBorrowers();
                    break;
                case "3":
                    borrowerLibrary.AddNewBorrower();
                    break;
                case "4": return;
                default:
                    Console.WriteLine("Invalid choice"); break;
            }
        }
    }

    private void ChooseAnOptionAndPressEnter()
    {
        Console.WriteLine();
        Console.Write("Choose an options from the menu a press Enter: ");
    }

    /// <summary>
    /// Provides a way to pause the program and prompt the user to press a key to continue
    /// </summary>
    public static void PressAKeyToContinue()
    {
        Console.WriteLine();
        Console.WriteLine("Please press a key to continue.");
        Console.ReadKey();
        Console.Clear();
    }

    public static string GetInputWithCancel()
    {
        string input = "";
        while (true)
        {
            ConsoleKeyInfo keyInfo = Console.ReadKey(true);

            // Check for "esc" key press
            if (keyInfo.Key == ConsoleKey.Escape)
            {
                Console.Clear();
                return null; // or any other action to indicate cancellation
            }
            else if (keyInfo.Key == ConsoleKey.Enter)
            {
                break; // exit the loop when Enter key is pressed
            }
            else
            {
                input += keyInfo.KeyChar; // append the entered character to the input
                Console.Write(keyInfo.KeyChar); // echo the character to the console
            }
        }

        return input;
    }
}

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
            new Borrower("Test2", "Testare2", 222222222222)
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
            new Book("A Tale of Two Cities", "Charles Dickens", 1859),
        new Book("The Little Prince", "Antoine de Saint-Exupéry", 1943),
        new Book("The Lord of the Rings", "J.R.R. Tolkien", 1955),
        new Book("Harry Potter and the Philosopher's Stone", "J. K. Rowling", 1997)
        };
    }
}