//TODO: hur implementerar vi arv?
//TODO:Hantera exceptions och liknande
//TODO: innan man lägger till bok eller låntagare, bekfräfta rätt uppgifter NEJ
//TODO: lägg till en funktion som gör det möjligt att redigera uppgifter NEJ
//TODO: gå igenom att public, private, static osv är korrekt
//TODO: lägg till kommentarer

// Kommentar till arv. Klassen borrower och book har flertal likheter, t.ex. metoden PrintOut() och sättet programmet spar ner data i Json format. Här ser jag en möjlighet att implementera ett interface
// för att möjliggöra att båda klasserna delar på gemensamma metoder.

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
        Book selectedBook = null;
        bool isValidChoice = false;

        // Ask the user to select a book to borrow
        while (!isValidChoice)
        {
            Console.Write($"Enter the number of the book you want to {input}: ");
            string userChoiceOfBookNumber = UI.GetInputWithCancel();
            Console.WriteLine();

            if (userChoiceOfBookNumber == null)
            {
                return null;
            }


            //TODO: LOOP?
            if (int.TryParse(userChoiceOfBookNumber, out int parsedBookNumber) && parsedBookNumber >= 1 && parsedBookNumber <= listOfBooks.Count)
            {
                selectedBook = listOfBooks[parsedBookNumber - 1];
                isValidChoice = true;
            }
            else
            {
                Console.WriteLine();
                Console.WriteLine("Invalid book selection.");
                Console.WriteLine();
            }
        }
        return selectedBook;


        /*while (true)
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
        }*/
    }

    public void SaveCurrentStatusOfBooks()
    {
        _dataRepository.SaveBooksToFile(this.allLibraryBooks);
    }

    #region Methods to handle adding book to the library
    internal void AddNewBook()
    {
        Console.Clear();
        Console.Title = "Adding a book menu";
        Console.WriteLine("===============================================");
        Console.WriteLine("You are about to add a new book to the library.");
        Console.WriteLine("===============================================");
        Console.WriteLine();
        Console.Write("Please enter the title of the book: ");
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
                Console.WriteLine($"There is a book with the title '{existingBook.Title}' by {existingBook.Author} already existing in the library. Is this the book you have in mind? [y/n]");
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
        Console.Write("Please enter the author of the book ");
        string userInputAuthor = UI.GetInputWithCancel();
        Console.WriteLine(); // To make a new line in console for better design.
        if (userInputAuthor == null)
        {
            return;
        }
        if (existingBook != null && userInputAuthor.ToLower() == existingBook.Author.ToLower())
        {
            Console.WriteLine($"{existingBook.Title} by {existingBook.Author} already exists.");
            UI.PressAKeyToContinue();
            return; // Exit the method
        }
        while (true)
        {
            Console.Write("Please enter the publishing year of the book: ");
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
                    int nextBookID = allLibraryBooks.Count + 1; // The first ID is 1. So to get the next free ID, look how many book there are and add +1. Example: if there is 2 book. allLibraryBooks.Count + 1 => 2 + 1 => 3 
                    allLibraryBooks.Add(new Book(userInputTitle, userInputAuthor, parsedYear, nextBookID));
                    Console.WriteLine();
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

        if (year > DateTime.Now.Year)
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
            if (book.Title.ToLower() == userInputTitle.ToLower())
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
        Console.Title = "Free books";
        if (books.Count == 0)
        {
            Console.WriteLine("No available books at the moment.");
            UI.PressAKeyToContinue();
            return;
        }

        DisplayBooks(books); // Display all available books

        Book selectedBook = SelectABookFromAList(books, "borrow");

        if (selectedBook == null)
        {
            return;
        }
        /*bool isValidChoice = false;
        
        // Ask the user to select a book to borrow
        
        while (!isValidChoice)
        {
            Console.Write("Enter the number of the book you want to borrow: ");
            string userChoiceOfBookNumber = UI.GetInputWithCancel();
            Console.WriteLine();

            if (userChoiceOfBookNumber == null)
            {
                return;
            }


            //TODO: LOOP?
            if (int.TryParse(userChoiceOfBookNumber, out int parsedBookNumber) && parsedBookNumber >= 1 && parsedBookNumber <= books.Count)
            {
                selectedBook = books[parsedBookNumber - 1];
                isValidChoice = true;
            }
            else
            {
                Console.WriteLine();
                Console.WriteLine("Invalid book selection.");
                Console.WriteLine();
            }
        }
        */

        long socialSecurityNumber = BorrowerHandling.GetSocialSecurityNumber();
        if (borrowerHandling.BorrowerExist(socialSecurityNumber, out Borrower currentBorrower))
        {
            string fullNameOfBorrower = $"{currentBorrower.FirstName} {currentBorrower.LastName}";
            int idOfBook = selectedBook.bookID;
            selectedBook.BorrowBook(fullNameOfBorrower);
            currentBorrower.borrowedBooksByID.Add(idOfBook);
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"{selectedBook.Title} has been borrowed by {fullNameOfBorrower}.");
            Console.ResetColor();
            UI.PressAKeyToContinue();
        }
        else
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Borrower not found or not valid. Please create a valid borrower first.");
            Console.ResetColor();
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
        Console.Title = "Return a book";
        if (bookList.Count == 0)
        {
            Console.Clear();
            Console.WriteLine("No books found");
            UI.PressAKeyToContinue();
            return;
        }

        DisplayBooks(bookList);

        Book userChoiceOfBook = SelectABookFromAList(bookList, "return");

        if (userChoiceOfBook == null)
        {
            return;
        }

        userChoiceOfBook.Return();
        //TODO: HÄR ÄR JAG OSÄKER OM RÄTT. KOM tillbaka och kontrollera om det inte funkar
        foreach (Borrower borrower in borrowerHandling.allLibraryBorrowers)
        {
            // Create a copy of the list to avoid modifying it while iterating
            List<int> booksToRemove = new List<int>();

            foreach (int bookID in borrower.borrowedBooksByID)
            {
                if (bookID == userChoiceOfBook.bookID)
                {
                    // Add the book ID to the list of books to remove
                    booksToRemove.Add(bookID);
                }
            }

            // Remove the books outside of the inner loop
            foreach (int bookIDToRemove in booksToRemove)
            {
                borrower.borrowedBooksByID.Remove(bookIDToRemove);
            }
        }

        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"{userChoiceOfBook.Title} by {userChoiceOfBook.Author} has been returned.");
        Console.ResetColor();
        UI.PressAKeyToContinue();
    }
    #endregion

    #region Methods used to display books
    /// <summary>
    /// Ask the user for a search word. If a matching book exist, the books are printed out.
    /// </summary>
    internal void FindABook()
    {

        List<Book> foundBooks = BookSearch();
        Console.Title = "Found books";
        if (foundBooks.Count == 0)
        {
            Console.WriteLine("No matches found");
            UI.PressAKeyToContinue();
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
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine("Nr".PadRight(5) + "Title".PadRight(45) + "Author".PadRight(30) + "Published".PadRight(15) + "Book ID".PadRight(15) + "Loan status");
        Console.ResetColor(); // Reset the color to the default
        for (int i = 0; i < books.Count; i++)
        {
            Console.Write($"{i + 1}".PadRight(5));
            Console.WriteLine(books[i].PrintOut());
        }
        Console.WriteLine();
    }
    /// <summary>
    /// Asks the user of a search word and returns a list of books containing that word.
    /// </summary>
    /// <returns></returns>
    internal List<Book> BookSearch()
    {
        Console.Title = "Search menu";
        Console.Clear();
        Console.WriteLine("Please enter a search word:");
        string searchWord = Console.ReadLine();

        List<Book> results = allLibraryBooks.
        Where(book => book.Title.Contains(searchWord, StringComparison.OrdinalIgnoreCase) ||
        book.Author.Contains(searchWord, StringComparison.OrdinalIgnoreCase))
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
