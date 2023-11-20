/// <summary>
/// Manages operations related to borrowers in the library, including adding, searching, and displaying borrower information.
/// </summary>
public class BorrowerHandling
{
    /// <summary>
    /// Gets or initializes a list of all borrowers in the library.
    /// </summary>
    public List<Borrower> AllLibraryBorrowers { get; set; }

    private readonly DataRepository _dataRepository;

    /// <summary>
    /// Initializes a new instance of the BorrowerHandling class with a provided DataRepository.
    /// </summary>
    public BorrowerHandling(DataRepository dataRepository)
    {
        this.AllLibraryBorrowers = dataRepository.LoadBorrowersFromFile();
        this._dataRepository = dataRepository;
    }

    /// <summary>
    /// Saves the current status of borrowers to a file using the data repository.
    /// </summary>
    public void SaveCurrentStatusOfBorrowers()
    {
        _dataRepository.SaveBorrowersToFile(this.AllLibraryBorrowers);
    }

    #region Methods that handle borrowers in the library

    /// <summary>
    /// Adds a new borrower to the library, prompting the user for input and validating the information.
    /// </summary>
    public void AddNewBorrower()
    {
        Console.Clear();
        Console.Title = "Add new borrower menu";
        Console.WriteLine("================================");
        Console.WriteLine("Add new borrower menu");
        Console.WriteLine("================================");
        Console.WriteLine();
        Console.Write("Please enter borrowers first name: ");

        // Prompt user for first name
        string firstName = UI.GetInputWithCancel();
        Console.WriteLine();

        // Check if user pressed 'esc', exit the method
        if (firstName == null)
        {
            return;
        }

        // Prompt user for last name
        Console.Write("Please enter borrowers last name: ");
        string lastName = UI.GetInputWithCancel();
        Console.WriteLine();

        // Check if user pressed 'esc', exit the method
        if (lastName == null)
        {
            return;
        }

        // Get valid social security number
        long socialSecurityNumber = GetSocialSecurityNumber();
        Console.WriteLine();

        // Check if user pressed 'esc' in the GetSocialSecurityNumber(), which returned '0'
        if (socialSecurityNumber == 0)
        {
            return;
        }

        // Check if borrower with the given social security number already exists
        else if (BorrowerExist(socialSecurityNumber, out Borrower CurrentBorrower))
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("A person with that social security number already exist.");
            Console.ResetColor();
            UI.PressAKeyToContinue();
        }
        else
        {
            // Create a new borrower and add it to the list of Borrowers
            AllLibraryBorrowers.Add(new Borrower(firstName, lastName, socialSecurityNumber)); // Creates a new user and adds it to the list of Borrowers.
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine($"{firstName} {lastName} added as a borrower to the library.");
            Console.ResetColor();
            UI.PressAKeyToContinue();

        }

    }

    /// <summary>
    /// Gets a valid social security number from the user with input validation.
    /// </summary>
    /// <returns>The valid social security number.</returns>
    public static long GetSocialSecurityNumber()
    {
        bool isValidInput = false;
        long socialSecurityNumber = 0;

        // Continue prompting the user until valid input is provided
        while (!isValidInput)
        {
            Console.Write("Please enter borrowers social security number (12 digits):");
            string userInput = UI.GetInputWithCancel();

            // User pressed 'esc', return 0 or any other action to indicate cancellation
            if (userInput == null)
            {
                return 0;
            }

            // Attempt to parse the user input as a long
            if (long.TryParse(userInput, out long parsedUserInput))
            {
                // Check if the parsed number is a valid social security number
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

    /// <summary>
    /// Checks if a borrower with the given social security number already exists in the library.
    /// </summary>
    /// <param name="socialSecurityNumberToControll">The social security number to check.</param>
    /// <param name="currentBorrower">The matching borrower if found.</param>
    /// <returns>True if the borrower exists; otherwise, false.</returns>
    public bool BorrowerExist(long socialSecurityNumberToControll, out Borrower currentBorrower)
    {
        currentBorrower = null;

        // Iterate through all library borrowers to find a match
        foreach (Borrower borrower in AllLibraryBorrowers)
        {
            // Check if the social security number matches
            if (borrower.socialSecurityNumber == socialSecurityNumberToControll)
            {
                // Assign the matching borrower and return true
                currentBorrower = borrower;
                return true;
            }
        }
        // No match found, return false
        return false;
    }

    /// <summary>
    /// Finds and displays borrowers based on a user-entered search word.
    /// </summary>
    /// <param name="bookLibrary">The BookHandling instance to access book information.</param>
    internal void FindABorrower(BookHandling bookLibrary)
    {
        Console.Title = "Borrower search result";

        // Perform a borrower search based on a user-entered search word
        List<Borrower> foundBorrowers = BorrowerSearch();

        // Check if the search operation was canceled (user pressed 'esc')
        if (foundBorrowers == null)
        {
            return;
        }

        // Check if any matches were found
        if (foundBorrowers.Count == 0)
        {
            Console.WriteLine("No mathes found");
            UI.PressAKeyToContinue();
        }
        else
        {
            Console.WriteLine("Mathes found: \n");

            // Display the found borrowers along with their borrowed books
            DisplayBorrower(foundBorrowers, bookLibrary);
            UI.PressAKeyToContinue();
        }
    }

    /// <summary>
    /// Displays borrower information, including borrowed books, to the console.
    /// </summary>
    /// <param name="foundBorrowers">The list of borrowers to display.</param>
    /// <param name="bookLibrary">The BookHandling instance to access book information.</param>
    private void DisplayBorrower(List<Borrower> foundBorrowers, BookHandling bookLibrary)
    {
        Console.Clear();

        // Loop through each found borrower
        for (int i = 0; i < foundBorrowers.Count; i++)
        {
            // Display basic borrower information
            Console.WriteLine($"{foundBorrowers[i].PrintOut()}");
            Console.WriteLine();

            // Check if the borrower has borrowed books
            if (foundBorrowers[i].borrowedBooksByID.Count > 0)
            {
                Console.WriteLine("Borrowed books: ");
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("Title".PadRight(45) + "Author".PadRight(30) + "Published".PadRight(15) + "Book ID".PadRight(15) + "Loan status");
                Console.ResetColor(); // Reset the color to the default

                // Create a list to store books associated with the current borrower
                List<Book> booksToPrint = new List<Book>();

                // Loop through borrowed book IDs
                foreach (int bookID in foundBorrowers[i].borrowedBooksByID)
                {
                    int intToControll = bookID;

                    // Loop through all library books to find the matching book
                    foreach (Book book in bookLibrary.AllLibraryBooks)
                    {
                        if (book.bookID == intToControll)
                        {
                            booksToPrint.Add(book);
                        }
                    }
                }

                // Display borrowed books
                foreach (Book book in booksToPrint)
                {
                    Console.WriteLine(book.PrintOut());
                }
            }
            else
            {
                Console.WriteLine("No borrowed books.");
            }

            Console.WriteLine();
            Console.WriteLine("==========================================================================================");
            Console.WriteLine();
        }
    }

    /// <summary>
    /// Searches for borrowers based on a user-entered search word.
    /// </summary>
    /// <returns>The list of borrowers matching the search criteria.</returns>
    private List<Borrower> BorrowerSearch()
    {
        Console.Clear();
        Console.Title = "Search borrower";

        // Prompt the user to enter a search word
        Console.Write("Please enter a search word: ");
        string searchWord = UI.GetInputWithCancel();

        // Check if user pressed 'esc', exit the method
        if (searchWord == null)
        {
            return null; 
        }

        // Filter and return the list of borrowers based on the search criteria
        List<Borrower> results = AllLibraryBorrowers.
            Where(borrower => borrower.FirstName.Contains(searchWord, StringComparison.OrdinalIgnoreCase) ||
            borrower.LastName.Contains(searchWord, StringComparison.OrdinalIgnoreCase) ||
            borrower.socialSecurityNumber.ToString().Contains(searchWord, StringComparison.OrdinalIgnoreCase)).ToList();
        return results;
    }

    /// <summary>
    /// Displays information for all borrowers in the library to the console.
    /// </summary>
    /// <param name="bookLibrary">The BookHandling instance to access book information.</param>
    internal void ListAllBorrowers(BookHandling bookLibrary)
    {
        Console.Title = "All library borrowers";

        // Display information for all borrowers along with their borrowed books
        DisplayBorrower(AllLibraryBorrowers, bookLibrary);

        UI.PressAKeyToContinue();
    }
    #endregion
}
