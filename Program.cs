//TODO: hur implementerar vi arv?
//TODO:Hantera exceptions och liknande
//TODO: innan man lägger till bok eller låntagare, bekfräfta rätt uppgifter NEJ
//TODO: lägg till en funktion som gör det möjligt att redigera uppgifter NEJ
//TODO: gå igenom att public, private, static osv är korrekt
//TODO: lägg till kommentarer

// Kommentar till arv. Klassen borrower och book har flertal likheter, t.ex. metoden PrintOut() och sättet programmet spar ner data i Json format. Här ser jag en möjlighet att implementera ett interface
// för att möjliggöra att båda klasserna delar på gemensamma metoder.

using System;
using System.ComponentModel.Design;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;
using static System.Reflection.Metadata.BlobBuilder;

// Skapa en instans av DataRepository för att hantera datafilinläsning och -skrivning
DataRepository repository = new DataRepository();

// Skapa en instans av BorrowerHandling för att hantera låntagare och deras åtgärder
BorrowerHandling myBorrowerHandling = new BorrowerHandling(repository);

// Skapa en instans av BookHandling för att hantera böcker och deras åtgärder
BookHandling myBookHandling = new BookHandling(myBorrowerHandling, repository);

// Skapa en instans av UI (användargränssnittet och deras åtgärder) och skicka med instanserna för bok- och låntagarehantering
UI ui = new UI(myBookHandling, myBorrowerHandling);

// Starta huvudmenyn i användargränssnittet
ui.MainMenu();
