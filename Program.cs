//The following is notes to my teacher where you can read my thought on the assignment:
/*
The following is notes to my teacher where you can read my thought on the assignment. 
Below are thoughts and considerations that I want to share with you.

I have divided the program into 7 classes, which is my attempt to implement object-oriented programming.

1. Program.cs - This part of the program initializes other classes and starts the main menu.
2. UI.cs - User interface. It contains various menus that "communicate" with the user. Depending on the user's choice, other parts of the program are activated.
3. Borrower.cs - Contains information and methods about the specific borrower, such as name, social security number, details about borrowed books, etc.
4. BorrowerHandling.cs - Manages borrowers in general. For example, how they are created, how the program displays information about them, and a list of all borrowers.
5. Book.cs - Contains information and methods for handling the specific book, such as title, status, details about the borrower, etc.
6. BookHandling - Manages books in general. Adding books, borrowing and returning books, searching for books, and a list of all books.
7. DataRepository - Handles fetching and saving data externally. In my program, data is saved in JSON format.

Overall, I am satisfied with the distribution of classes. I believe I have succeeded well with the Single-responsibility principle, meaning each class has a specific responsibility.
However, there are challenges. I find the balance between UI and other parts complex. For example, there is communication between the program and the user that doesn't happen in the 
UI object. I'm unsure about how strictly one should adhere to having all communication through UI. For instance, BookHandling contains methods that ask the user for input.

For your information, I have created some pre-made objects for books and borrowers to test the program. You can find them under DataRepository.cs.

---INHERITANCE ---
I have not implemented inheritance in my program. During the planning stage and the initial execution of the program, I found it challenging to identify clear relationships where 
inheritance could have been implemented. Of course, there were simple ways, but they felt outside the scope of the task. For example, having a base class Book or Borrower and 
derived classes like Adult, Child, AdultBook, and ChildBook.

As I progressed in the project, I could see relationships where inheritance with polymorphism could have been applicable, mainly between the Borrower and Book classes. Both are 
classes where their objects are created in multiple instances. Both classes' objects are saved and loaded to/from JSON. Both classes have a PrintOut method used in the program 
to print information about the objects. I attempted to create an interface for these objects, but being deep into the project, implementing an interface that created common 
contracts for the two classes resulted in many conflicts. It felt like solving one conflict created two new ones, so I gave up on implementing this. Still, I want to highlight 
that I did consider how inheritance could be applied in the program.

--- CHALLENGES ---
I faced several challenges along the way and had to rework parts of the project several times. A major bug I encountered was a "Self referencing loop" error when the program 
tried to save/load from JSON. It took a long time to find and address the issue, and I'm still not entirely sure I fully understood it. But I believe the problem was having two 
lists of objects, Borrower and Books. Each object had its list of each other's objects. For example, an instance of the borrower had a list of instances of books they borrowed. 
When the program ran, a list of borrowers would be loaded first, then a list of books. The problem arose when the program ran and tried to create an instance of Borrower that 
included a list of Books, but the instances of Book were not yet created.

Other challenges included getting the right Access modifiers for the class members, especially their fields and properties. Sometimes, when I thought a property should have a 
public get and private set, certain parts of the program would stop working correctly.

--- LESSONS LEARNED ---
The most significant lesson I take away is to start with a Minimum Viable Product (MVP). Instead of creating a program with many extra features and methods, such as validating 
social security numbers, when I started getting close to completing the program, I realized that a fundamental function, saving and loading data, wasn't working correctly. Trying 
to troubleshoot and fix the problem after writing over 1000 lines of code was not easy. In the future, I will create an MVP and then add additional code to improve the program.

 */


using System;
using System.ComponentModel.Design;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;
using static System.Reflection.Metadata.BlobBuilder;

// Create an instance of DataRepository to handle data file reading and writing.
DataRepository repository = new DataRepository();

// Create an instance of BorrowerHandling to manage borrowers and their operations.
BorrowerHandling myBorrowerHandling = new BorrowerHandling(repository);

// Create an instance of BookHandling to manage books and their operations.
BookHandling myBookHandling = new BookHandling(myBorrowerHandling, repository);

// Create an instance of UI (user interface and its actions) and pass along the instances for book and borrower management.
UI ui = new UI(myBookHandling, myBorrowerHandling);

// Start the main menu in the user interface
ui.MainMenu();
