/// <summary>
/// The UI class handles the user interface and menu navigation for the library management system.
/// </summary>
public class UI
{
    /// <summary>
    /// The BookHandling instance for managing book-related operations in the library.
    /// </summary>
    private BookHandling bookLibrary;

    /// <summary>
    /// The BorrowerHandling instance for managing borrower-related operations in the library.
    /// </summary>
    private BorrowerHandling borrowerLibrary;

    /// <summary>
    /// Initializes a new instance of the UI class with references to BookHandling and BorrowerHandling instances.
    /// </summary>
    /// <param name="bookLibrary">The BookHandling instance for managing books.</param>
    /// <param name="borrowerLibrary">The BorrowerHandling instance for managing borrowers.</param>
    public UI(BookHandling bookLibrary, BorrowerHandling borrowerLibrary)
    {
        this.bookLibrary = bookLibrary;
        this.borrowerLibrary = borrowerLibrary;
    }

    /// <summary>
    /// Displays the main menu and handles user choices for library management.
    /// </summary>
    public void MainMenu()
    {
        Console.Title = "Main Menu";

        // Main menu loop
        while (true)
        {
            Console.Clear();
            Console.Title = "Main Menu";
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
            ChooseAnOptionAndPressEnter();

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
    /// <summary>
    /// Displays the search options for finding a book.
    /// </summary>
    private void SearchABookMenu()
    {

        while (true)
        {
            Console.Clear();
            Console.Title = "Find a book menu";
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
                    Console.Title = "Found books";
                    bookLibrary.ListAllBooks();
                    break;
                case "3": return;
                default: break;
            }
        }
    }
    /// <summary>
/// Displays the options for lending out books.
/// </summary>
    private void LoanBookMenu()
    {

        while (true)
        {
            Console.Clear();
            Console.Title = "Lend out menu";
            Console.WriteLine("====================================");
            Console.WriteLine("Lend out menu");
            Console.WriteLine("====================================");
            Console.WriteLine();
            Console.WriteLine("1 - Search for a free book to lend out");
            Console.WriteLine();
            Console.WriteLine("2 - List all free books to lend out");
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
                case "3": return;
                default: break;
            }
        }
    }

    /// <summary>
    /// Displays the options for returning books.
    /// </summary>
    private void ReturnBookMenu()
    {
        while (true)
        {
            Console.Clear();
            Console.Title = "Return menu";
            Console.WriteLine("====================================");
            Console.WriteLine("Return a book menu");
            Console.WriteLine("====================================");
            Console.WriteLine();
            Console.WriteLine("1 - Search for a book to return");
            Console.WriteLine();
            Console.WriteLine("2 - Show a list of all lend out books");
            Console.WriteLine();
            Console.WriteLine("3 - Return to main menu");
            Console.WriteLine();
            Console.WriteLine("====================================");
            ChooseAnOptionAndPressEnter();

            string userchoice = Console.ReadLine();

            switch (userchoice)
            {
                case "1":
                    bookLibrary.SearchABookToReturn();
                    break;
                case "2":
                    bookLibrary.ReturnABookFromAList();
                    break;
                case "3": return; ;
                default: Console.WriteLine("Invalid choice"); break;
            }
        }
    }

    /// <summary>
    /// Displays the options for managing books, allowing the user to add a new book or return to the main menu.
    /// </summary>
    private void ManageBooksMenu()
    {
        while (true)
        {
            Console.Clear();
            Console.Title = "Manage books menu";
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

    /// <summary>
    /// Displays the options for managing borrowers, allowing the user to search for a borrower, list all borrowers,
    /// add a new borrower to the library, or return to the main menu.
    /// </summary>
    private void ManageBorrowersMenu()
    {
        while (true)
        {
            Console.Clear();
            Console.Title = "Manage borrowers menu";
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
                    borrowerLibrary.FindABorrower(bookLibrary);
                    break;
                case "2":
                    borrowerLibrary.ListAllBorrowers(bookLibrary);
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

    /// <summary>
    /// Displays a message prompting the user to choose an option.
    /// </summary>
    private void ChooseAnOptionAndPressEnter()
    {
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write("Choose an options from the menu and press Enter: ");
        Console.ResetColor(); // Reset the color to the default
    }

    /// <summary>
    /// Provides a way to pause the program and prompt the user to press a key to continue
    /// </summary>
    public static void PressAKeyToContinue()
    {
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine("Please press a key to continue.");
        Console.ResetColor(); // Reset the color to the default
        Console.ReadKey();
        Console.Clear();
    }

    /// <summary>
    /// Reads input from the user, allowing cancellation by pressing the "Esc" key.
    /// Supports backspace to delete the last character and Enter to submit the input.
    /// </summary>
    /// <returns>The entered input as a string or null if the operation was canceled.</returns>
    public static string GetInputWithCancel()
    {
        // Initialize an empty string to store user input
        string input = "";

        // Continue reading input until Enter och ESC key is pressed
        while (true)
        {
            // Read a key without displaying it on the console
            ConsoleKeyInfo keyInfo = Console.ReadKey(true);

            // Check for "esc" key press
            if (keyInfo.Key == ConsoleKey.Escape)
            {
                Console.Clear();
                return null; // Indicate cancellation by returning null
            }

            // Check for backspace key press
            else if (keyInfo.Key == ConsoleKey.Backspace)
            {
                // Handle backspace: remove the last character from input
                if (input.Length > 0)
                {
                    input = input.Substring(0, input.Length - 1);
                    Console.Write("\b \b"); // move the cursor back and erase the character
                }
            }

            // Check for Enter key press
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

        return input; // Return the entered input as a string
    }
}