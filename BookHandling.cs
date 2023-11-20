public class BookHandling
{
    private BorrowerHandling _borrowerHandling;
    private DataRepository _dataRepository;
    public List<Book> AllLibraryBooks { get; private set; }

    public BookHandling(BorrowerHandling borrowerHandling, DataRepository repository)
    {
        this._borrowerHandling = borrowerHandling;
        _dataRepository = repository;
        AllLibraryBooks = repository.LoadBooksFromFile();
    }

    /// <summary>
    ///Method to select a book from a list based on user input
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
            Console.WriteLine();
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
    }

    /// <summary>
    /// Method to save the current status of books to a file via the repository.
    /// </summary>
    public void SaveCurrentStatusOfBooks()
    {
        _dataRepository.SaveBooksToFile(this.AllLibraryBooks);
    }

    #region Methods to handle adding book to the library
    /// <summary>
    /// Allows and guides the user to add a book to the library.
    /// </summary>
    internal void AddNewBook()
    {
        Console.Clear();
        Console.Title = "Adding a book menu";

        // Display a header to inform the user about the operation
        Console.WriteLine("===============================================");
        Console.WriteLine("You are about to add a new book to the library.");
        Console.WriteLine("===============================================");
        Console.WriteLine();

        // Prompt the user to enter the title of the book
        Console.Write("Please enter the title of the book: ");
        string userInputTitle = UI.GetInputWithCancel();
        Console.WriteLine(); // To make a new line in console for better design.

        // Check if the user canceled the operation
        if (userInputTitle == null)
        {
            return;
        }

        Book existingBook = null;
        

        // Check if a book with the entered title already exists and give the user a choice on how to continue
        if (DoesBookExist(userInputTitle, out existingBook))
        {
            while (true)
            {
                // Ask the user if the existing book is the one they intended to add
                Console.WriteLine($"There is a book with the title '{existingBook.Title}' by {existingBook.Author} already existing in the library. Is this the book you have in mind? [y/n]");
                string userChoice = Console.ReadLine().ToLower();

                switch (userChoice)
                {
                    case "y": 
                        // If yes, exit the method
                        UI.PressAKeyToContinue();
                        return; // Exit the method
                    case "n": 
                        // If no, set a flag to continue adding a new book
                        goto labelContinueHere;
                    default:
                        Console.WriteLine("Invalid choice!");
                        break;
                }
            }

        }
    //If the user wants to add a book with an title that already exist in the library,
    //the method continues here

    labelContinueHere:
        // Prompt the user to enter the author of the book
        Console.Write("Please enter the author of the book ");
        string userInputAuthor = UI.GetInputWithCancel();
        Console.WriteLine(); // To make a new line in console for better design.

        // Check if the user canceled the operation
        if (userInputAuthor == null)
        {
            return;
        }

        // Check if the entered book already exists based on title and author. 
        // If it does: inform the user and exit the method.
        if (existingBook != null && userInputAuthor.ToLower() == existingBook.Author.ToLower())
        {
            Console.WriteLine($"{existingBook.Title} by {existingBook.Author} already exists.");
            UI.PressAKeyToContinue();
            return; // Exit the method
        }

        while (true)
        {
            // Prompt the user to enter the publishing year of the book
            Console.Write("Please enter the publishing year of the book: ");
            string year = UI.GetInputWithCancel();
            Console.WriteLine(); // To make a new line in console for better design.

            // Check if the user canceled the operation
            if (year == null)
            {
                return;
            }

            //Try parse the userinput to a number
            if (Int32.TryParse(year, out int parsedYear))
            {
                // Validate if the entered year is valid
                if (ValidDate(parsedYear))
                {
                    // Generate the next book ID
                    int nextBookID = AllLibraryBooks.Count + 1; // The first ID is 1. So to get the next free ID, look how many book there are and add +1. Example: if there is 2 book. allLibraryBooks.Count + 1 => 2 + 1 => 3 

                    // Add the new book to the list
                    AllLibraryBooks.Add(new Book(userInputTitle, userInputAuthor, parsedYear, nextBookID));

                    // Display a success message
                    Console.WriteLine();
                    Console.WriteLine($"{userInputTitle} by {userInputAuthor} has been added to the library.");
                    UI.PressAKeyToContinue();
                    return;
                }
                else
                {
                    //Display if invalid year and iterate a new loop
                    Console.WriteLine("Invalid year.");
                }
            }
            //Display if invalid input and iterate a new loop
            else { Console.WriteLine("Invalid input. Please enter digits only."); }
        }
    }

    /// <summary>
    /// Helper method to validate if a year is a valid date and not set in the future
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

    /// <summary>
    /// Helper method to check if a book with a given title already exists
    /// </summary>
    /// <param name="userInputTitle"></param>
    /// <param name="foundBook"></param>
    /// <returns></returns>
    private bool DoesBookExist(string userInputTitle, out Book foundBook)
    {
        foundBook = null;
        foreach (Book book in AllLibraryBooks)
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
    /// <summary>
    /// Initiates the process of loaning a book by presenting a list of available books to the user.
    /// </summary>
    internal void LoanABookFromAList()
    {
        // Delegate the loaning process to HandleLoaningABook method,
        // providing a list of all currently free books in the library.
        HandleLoaningABook(ListAllFreeBooks());
    }

    /// <summary>
    /// Searches for books that are not currently loaned, and then initiates the process of loaning a book.
    /// </summary>
    internal void SearchABookToLoan()
    {
        // Retrieve a list of books based on user search criteria.
        List<Book> listOfBooks = BookSearch();

        //If user cancelled the book search, BookSearch return null. Thereby cancel this method.
        if (listOfBooks == null)
        {
            return;
        }

        // Filter the list to include only books that are not currently loaned.
        List<Book> listOfBooksFilteredByNotLoaned = new List<Book>();
        foreach (Book book in listOfBooks)
        {
            if (!book.IsBorrowed)
            {
                listOfBooksFilteredByNotLoaned.Add(book);
            }
        }

        // Delegate the loaning process to HandleLoaningABook method,
        // providing the filtered list of books.
        HandleLoaningABook(listOfBooksFilteredByNotLoaned);
    }

    /// <summary>
    /// Handles the process of loaning a book, interacting with the user to select and confirm the loan.
    /// </summary>
    /// <param name="books">The list of books available for loan.</param>
    private void HandleLoaningABook(List<Book> books)
    {
        Console.Title = "Free books";

        // Check if there are available books for loan.
        if (books.Count == 0)
        {
            Console.WriteLine("No available books at the moment.");
            UI.PressAKeyToContinue();
            return;
        }

        // Display a list of available books for the user to choose from.
        DisplayBooks(books); // Display all available books

        // Prompt the user to select a book for borrowing.
        Book selectedBook = SelectABookFromAList(books, "borrow");

        // Check if the user canceled the operation.
        if (selectedBook == null)
        {
            return;
        }

        // Retrieve the borrower's social security number.
        long socialSecurityNumber = BorrowerHandling.GetSocialSecurityNumber();

        // Check if the given SSN is valid and if the borrower exists.
        if (_borrowerHandling.BorrowerExist(socialSecurityNumber, out Borrower currentBorrower))
        {
            // Construct the borrower's full name.
            string fullNameOfBorrower = $"{currentBorrower.FirstName} {currentBorrower.LastName}";

            // Record the loaned book in the library and the borrower's record.
            int idOfBook = selectedBook.bookID;
            selectedBook.BorrowBook(fullNameOfBorrower);
            currentBorrower.borrowedBooksByID.Add(idOfBook);

            // Display a success message to the user.
            Console.WriteLine();
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"{selectedBook.Title} has been borrowed by {fullNameOfBorrower}.");
            Console.ResetColor();
            UI.PressAKeyToContinue();
        }
        else
        {
            // Display an error message if the borrower is not found or not valid.
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Borrower not found or not valid. Please create a valid borrower first.");
            Console.ResetColor();
            UI.PressAKeyToContinue();
        }
    }
    #endregion

    #region Methods for returning a book
    /// <summary>
    /// Initiates the process of returning a borrowed book by presenting a list of borrowed books to the user.
    /// </summary>
    internal void ReturnABookFromAList()
    {
        // Retrieve a list of all currently borrowed books in the library.
        List<Book> listOfBorrowedBooks = ListAllBorrowedBooks();

        // Delegate the returning process to HandleReturningABook method,
        // providing a list of borrowed books.
        HandleReturningABook(listOfBorrowedBooks);
    }

    /// <summary>
    /// Searches for borrowed books and initiates the process of returning a book.
    /// </summary>
    internal void SearchABookToReturn()
    {
        // Retrieve a list of books based on user search criteria.
        List<Book> listOfBooks = BookSearch();

        //If user cancelled the book search, BookSearch return null. Thereby cancel this method.
        if (listOfBooks == null)
        {
            return;
        }

        // Filter the list to include only books that are currently borrowed.
        List<Book> listOfBooksFilteredByLoaned = new List<Book>();
        foreach (Book book in listOfBooks)
        {
            if (book.IsBorrowed)
            {
                listOfBooksFilteredByLoaned.Add(book);
            }
        }

        // Delegate the returning process to HandleReturningABook method,
        // providing the filtered list of borrowed books.
        HandleReturningABook(listOfBooksFilteredByLoaned);
    }

    /// <summary>
    /// Handles the process of returning a borrowed book, interacting with the user to select and confirm the return.
    /// </summary>
    /// <param name="bookList">The list of borrowed books.</param>
    private void HandleReturningABook(List<Book> bookList)
    {
        Console.Title = "Return a book";

        // Check if there are borrowed books available for return.
        if (bookList.Count == 0)
        {
            Console.Clear();
            Console.WriteLine("No books found");
            UI.PressAKeyToContinue();
            return;
        }

        // Display a list of borrowed books for the user to choose from.
        DisplayBooks(bookList);

        // Prompt the user to select a borrowed book for return.
        Book userChoiceOfBook = SelectABookFromAList(bookList, "return");

        // Check if the user canceled the operation.
        if (userChoiceOfBook == null)
        {
            return;
        }

        // Mark the book as returned and update borrower records.
        userChoiceOfBook.Return();

        // NOTE TO SELF: FOLLOWING LOOPS DOESNT FEEL INTUITIVE. CAN IT BE MADE CLEARER?
        // Iterate through all borrowers to update their records.
        foreach (Borrower borrower in _borrowerHandling.AllLibraryBorrowers)
        {
            // Create a list where to copy the books that are to be removed
            List<int> booksToRemove = new List<int>();

            // Identify the borrowed book by its unique ID.
            foreach (int bookID in borrower.borrowedBooksByID)
            {
                if (bookID == userChoiceOfBook.bookID)
                {
                    // Add the book ID to the list of books to remove
                    booksToRemove.Add(bookID);
                }
            }

            //Remove books from from libaryusers list.
            foreach (int bookIDToRemove in booksToRemove)
            {
                borrower.borrowedBooksByID.Remove(bookIDToRemove);
            }
        }

        // Display a success message to the user.
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"{userChoiceOfBook.Title} by {userChoiceOfBook.Author} has been returned.");
        Console.ResetColor();
        UI.PressAKeyToContinue();
    }
    #endregion

    #region Methods used to display books
    /// <summary>
    /// Initiates a book search based on a user-entered search word.
    /// If matching books are found, they are displayed to the user.
    /// </summary>
    internal void FindABook()
    {
        // Retrieve a list of books based on user search criteria.
        List<Book> foundBooks = BookSearch();

        //If user cancelled the book search, BookSearch return null. Thereby exit this method.
        if (foundBooks == null)
        {
            return;
        }

        Console.Title = "Found books";

        // Check if any matching books were found.
        if (foundBooks.Count == 0)
        {
            Console.WriteLine("No matches found");
            UI.PressAKeyToContinue();
        }
        else
        {
            // Display the found books to the user.
            Console.WriteLine("Matches found:\n");
            DisplayBooks(foundBooks);
            UI.PressAKeyToContinue();
        }
    }

    /// <summary>
    /// Displays a list of all books currently in the library.
    /// </summary>
    internal void ListAllBooks()
    {
        DisplayBooks(AllLibraryBooks);
        UI.PressAKeyToContinue();
    }

    /// <summary>
    /// Displays a formatted list of books to the console.
    /// </summary>
    /// <param name="books">The list of books to display.</param>
    private void DisplayBooks(List<Book> books)
    {
        Console.Clear();

        // Set the console text color to blue for the header.
        Console.ForegroundColor = ConsoleColor.Blue;

        // Display the header for the book information.
        Console.WriteLine("Nr".PadRight(5) + "Title".PadRight(45) + "Author".PadRight(30) + "Published".PadRight(15) + "Book ID".PadRight(15) + "Loan status");
        Console.ResetColor(); // Reset the color to the default

        // Iterate through the list of books and display each book's information.
        for (int i = 0; i < books.Count; i++)
        {
            Console.Write($"{i + 1}".PadRight(5));
            Console.WriteLine(books[i].PrintOut());
        }
    }

    /// <summary>
    /// Performs a book search based on a user-entered search word.
    /// Returns a list of books that match the search criteria.
    /// </summary>
    /// <returns>The list of books matching the search criteria.</returns>
    private List<Book> BookSearch()
    {
        Console.Title = "Search menu";
        Console.Clear();

        //TODO: Fortsätt här. Ändra så att du hämtar sökord via metod med felhantering

        //Prompt the user to enter a search word.

        
        Console.WriteLine("Please enter a search word:");
        string searchWord = UI.GetInputWithCancel();
        
        if (searchWord == null)
        {
            return null; // User pressed 'esc', return nullto indicate cancellation
        }

        // Filter the library books based on the search word, ignoring case.
        List<Book> results = AllLibraryBooks.
        Where(book => book.Title.Contains(searchWord, StringComparison.OrdinalIgnoreCase) ||
        book.Author.Contains(searchWord, StringComparison.OrdinalIgnoreCase))
        .ToList();

        return results;
    }

    /// <summary>
    /// Creates a list containing books that are currently borrowed.
    /// </summary>
    /// <returns>The list of borrowed books.</returns>
    private List<Book> ListAllBorrowedBooks()
    {
        // Initialize a list to store borrowed books.
        List<Book> borrowedBooks = new List<Book>();

        // Iterate through all library books and add borrowed ones to the list.
        foreach (Book book in AllLibraryBooks)
        {
            if (book.IsBorrowed)
            {
                borrowedBooks.Add(book);
            }
        }

        return borrowedBooks;
    }

    /// <summary>
    /// Creates a list containing books that are currently available for loan.
    /// </summary>
    /// <returns>The list of books available for loan.</returns>
    private List<Book> ListAllFreeBooks()
    {
        // Initialize a list to store books available for loan.
        List<Book> freeBooks = new List<Book>();
        
        // Iterate through all library books and add free ones to the list.
        foreach (Book book in AllLibraryBooks)
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
