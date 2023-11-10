//TODO: hur implementerar vi arv?
//TODO:Hantera exceptions och liknande
//TODO: innan man lägger till bok eller låntagare, bekfräfta rätt uppgifter NEJ
//TODO: lägg till en funktion som gör det möjligt att redigera uppgifter NEJ
//TODO: gå igenom att public, private, static osv är korrekt
//TODO: lägg till kommentarer

// Kommentar till arv. Klassen borrower och book har flertal likheter, t.ex. metoden PrintOut() och sättet programmet spar ner data i Json format. Här ser jag en möjlighet att implementera ett interface
// för att möjliggöra att båda klasserna delar på gemensamma metoder.

public class BorrowerHandling
{
    public List<Borrower> allLibraryBorrowers;
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
        Console.Clear();
        Console.Title = "Add new borrower menu";
        Console.WriteLine("================================");
        Console.WriteLine("Add new borrower menu");
        Console.WriteLine("================================");
        Console.WriteLine();
        Console.Write("Please enter borrowers first name: ");
        string firstName = UI.GetInputWithCancel();
        Console.WriteLine(); // To make a new line in console for better design.
        if (firstName == null)
        {
            return; // User pressed 'esc', exit the method
        }
        Console.Write("Please enter borrowers last name: ");
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
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("A person with that social security number already exist.");
            Console.ResetColor();
            UI.PressAKeyToContinue();
        }
        else
        {
            allLibraryBorrowers.Add(new Borrower(firstName, lastName, socialSecurityNumber)); // Creates a new user and adds it to the list of Borrowers.
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine($"{firstName} {lastName} added as a borrower to the library.");
            Console.ResetColor();
            UI.PressAKeyToContinue();

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

    internal void FindABorrower(BookHandling bookLibrary)
    {
        List<Borrower> foundBorrowers = BorrowerSearch();
        Console.Title = "Borrower search result";
        if (foundBorrowers.Count == 0)
        {
            Console.WriteLine("No mathes found");
            UI.PressAKeyToContinue();
        }
        else
        {
            Console.WriteLine("Mathes found: \n");

            DisplayBorrower(foundBorrowers, bookLibrary);
            UI.PressAKeyToContinue();
        }
    }

    private void DisplayBorrower(List<Borrower> foundBorrowers, BookHandling bookLibrary)
    {
        Console.Clear();
        for (int i = 0; i < foundBorrowers.Count; i++)
        {
            Console.WriteLine($"{foundBorrowers[i].PrintOut()}");
            Console.WriteLine();
            if (foundBorrowers[i].borrowedBooksByID.Count > 0)
            {
                Console.WriteLine("Borrowed books: ");
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("Title".PadRight(45) + "Author".PadRight(30) + "Published".PadRight(15) + "Book ID".PadRight(15)+ "Loan status");
                Console.ResetColor(); // Reset the color to the default

                //TODO: LÄGG TILL EN LOOP SOM LOOPAR BÖCKERNA HÄR
                List<Book> booksToPrint = new List<Book>();

                foreach (int bookID in foundBorrowers[i].borrowedBooksByID)
                {
                    int intToControll = bookID;
                    foreach (Book book in bookLibrary.allLibraryBooks)
                    {
                        if (book.bookID == intToControll)
                        {
                            booksToPrint.Add(book);
                        }
                    }
                }
                
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
    private List<Borrower> BorrowerSearch()
    {
        Console.Clear();
        Console.Title = "Search borrower";
        Console.Write("Please enter a searchword: ");
        string searchWord = Console.ReadLine();

        List<Borrower> results = allLibraryBorrowers.
            Where(borrower => borrower.FirstName.Contains(searchWord, StringComparison.OrdinalIgnoreCase) ||
            borrower.LastName.Contains(searchWord, StringComparison.OrdinalIgnoreCase) ||
            borrower.socialSecurityNumber.ToString().Contains(searchWord, StringComparison.OrdinalIgnoreCase)).ToList();
        return results;
    }

    internal void ListAllBorrowers(BookHandling bookLibrary)
    {
        Console.Title = "All library borrowers";
        DisplayBorrower(allLibraryBorrowers, bookLibrary);
        UI.PressAKeyToContinue();
    }
    #endregion
}
