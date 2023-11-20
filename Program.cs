/*
BÖRJA HÄR
 Hej Nevena. Nedan kommer tankar och funderingar som jag vill dela med mig till dig.

 Jag har delat upp programmet i 7 klasser vilket är mitt försök att implementera objektorienterad programmering.
 1. Program.cs - Den del av programmet som initerar övriga klasser och startar huvudmenyn.
 2. UI.cs - User interface. Här finns de olika menyerna som "pratar" men användaren. Beroende på användarens val så aktiveras andra delar av programmet.
 3. Borrower.cs - Innehåller uppgifter och metoder om den specifika lånetagaren. T.ex. namn, personnummer, uppgifter om lånadeböcker osv.
 4. BorrowerHandling.cs - Hanterar lånetagare i allmänhet. Tex. hur de skapas, hur programmet visar information om dem, samt lista med alla lånetagare
 5. Book.cs - Innehåller uppgifter och metoder för hantering om den specifika boken. T.ex. titel, status, uppgifter om lånetagare, osv.
 6. BookHandling - Hanterar böcker i allmänhet. Lägga till böcker, låna och returnera böcker, söka på böcker, lista med samtliga böcker.
 7. DataRepository - Hanterar hämtning och sparning av data externt. I mitt program så sparas data i json format.

Allmänt är jag nöjd med fördelningen av klasserna. Jag tycker att jag lyckats rätt väl med Single-responsibility principle. Dvs att en klass ska ha ett ansvar. 
Det finns dock utmaningar. Jag tycker balansen mellan UI och andra delar är komplex. T.ex. finns det kommunikation mellan programmet och användaren som
inte sker i UI-objektet. Jag vet inte hur strikt man ska förhålla sig till att alla kommunikation sker via UI? T.ex. innehåller BookHandling ett antal
metod som frågar användaren om input.

För din kännedom så har jag skapat ett antal färdiga objekt för böcker och lånetagare för att kunna testa programmet. Du finner dem under DataRepository.cs.

ARV: 
I mitt program har jag inte implementerat arv. I planeringstadiet och i början av utförandet av programmet hade jag svårt att se tydliga samband
där arv skulle ha kunnat implementeras. Sjävlklart fanns det enkla sätt men som kändes som att de låg utanför ramarna för uppgifte. T.ex. skulle man
kunna ha haft basklassen bok eller lånetagare samt härledda klasser som Vuxen, barn, vuxenbok och barnbok.
När jag hade kommit en bra bit in i projektet kunde jag börja se samband där arv med polymorfism hade kunna vara applicerabart. Främst mellan klasserna
Borrower och Book. Båda är klasser där deras objekt skapas i flera exemplar. Båda klassernas objekt sparas ned och laddas upp till/från JSON. Båda klasserna
har en PrintOut metod som använs i programmet för att printa ut information om objekten. Jag gav mig ett ett försök med att skapa en Interface för dessa objekt,
dock hade redan kommit så långt in i projektet, så att implementera interface som skapade gemensamma kontrakt för de två klasserna skapade massvis med konflikter.
Känslan var sådan att om jag löste en konflikt så skapade den två nya, så jag gav upp med att implementera detta. Men jag vill ändå lyfta att jag i alla fall har 
funderat över hur arv kan appliceras i programmet.

UTMANINGAR: Jag har haft en del utmaningar längst med vägen och fått arbeta om delar av projektet några gånger. En större bugg jag hade vara att jag fick
ett felmeddelande om Self referencing loop när programmet skulle spara/ladda upp från JSON. Det tog lång tid att hitta och åtgärda problemet, är 
fortfarande inte säker om jag förstod problemet fullt ut. Men jag tror problemet var att jag hade två listor med objekt, Borrower och Books. Varje objekt hade 
varsin lista av varandras objekt. T.ex. hade en instans av lånetagaren en lista med instanser av böcker den lånat. När programmet körde så laddades först en 
lista av t.ex. lånetagare, sen en lista med böcker. Problemet uppstår när programmet körs och ska skapa en instans av Borrower som inkluderar en
av en lista med Books, men instanserna av Book ännu inte är skapade. 
Andra utmaningar jag hade var att få till rätt Access modifiers för klassernas medlemmar, särskilt deras fields och properties. Ibland när jag trodde det skulle
t.ex vara en propertie med public get och private set så kunde delar av programmet sluta fungera korrekt. 

LÄRDOMAR:
Den största lärdomen jag tar med mig är att fortsättningvis att börja med en MVP (Minimum Viable Product). Istället för att skapa ett program som uppfyller minimum
kraven så började jag med en massa extra funktioner och metoder för att t.ex. validera personummer och liknande. När jag sedan, efter mycket möda, börjar bli klart
med programmet så märker jag att en grundläggande funktion, som är ett krav, att spara och ladda data inte fungerar korrekt. Att då, efter man ha skrivit +1000 rader 
kod börja felsöka och åtgärda problemet var inte enkelt. Fortsättningvis kommer jag skapa en MVP och sedan applicera extra kod som gör programmet bättre.
*/


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
