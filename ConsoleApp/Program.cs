using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Contacts.Application.Domain;
using Contacts.Application.Services;
using Contacts.Application.UseCases.DTOs;
using Contacts.Application.Repositories;
using Contacts.Data;

// ====================================================================
// Zadanie 1: FirstName i LastName jako obiekty wartosci (value objects)
// Zadanie 2: EmailAddress z walidacja regex
// Zadanie 3: Modyfikowanie kategorii (UpdateCategory)
// Zadanie 4: Usuwanie kategorii z blokadem gdy sa emaile
// Zadanie 5: Dodawanie, edycja i usuwanie kontaktu
// Zadanie 6: Dodawanie i usuwanie emaila z kontaktu
// Zadanie 7: Dodawanie i usuwanie telefonu do kontaktu
// Zadanie 8: Dodawanie, edycja i usuwanie waznych dat kontaktu
// ====================================================================

// Wczytanie pliku konfiguracyjnego
var builder = new ConfigurationBuilder();
builder.SetBasePath(AppContext.BaseDirectory)
.AddJsonFile(
"appsettings.json",
optional: false,
reloadOnChange: true);

// Skonfigurowanie polaczenia z baza danych
IConfiguration config = builder.Build();
var dbOptionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
dbOptionsBuilder.UseSqlite(config.GetConnectionString("default"));

// Stworzenie kontekstu
using var context = new AppDbContext(dbOptionsBuilder.Options);
context.Database.Migrate();
context.Database.EnsureCreated();

// Utworzenie serwisow i repozytoriow
var categoryRepository = new CategoryRepository(context);
var contactRepository = new ContactRepository(context);
var unitOfWork = new UnitOfWork(context);

var categoryServices = new CategoryServices(categoryRepository, unitOfWork);
var contactServices = new ContactServices(contactRepository, categoryRepository, unitOfWork);

// ====================================================================
// ZADANIE 3: Modyfikowanie kategorii
// ====================================================================
var newsletter = categoryRepository.GetCategoryByName("Newsletter");
if (newsletter is null)
{
    newsletter = (await context.Categories.AddAsync(new Category("Newsletter"))).Entity;
    await context.SaveChangesAsync();
}

// ====================================================================
// ZADANIE 5: Dodawanie kontaktu
// ZADANIE 1: Uzycie obiektow wartosci FirstName i LastName
// ====================================================================
var contact = context.Contacts.FirstOrDefault();
if (contact is null)
{
    var result = contactServices.AddContact(new AddContactDTO
    {
        FirstName = "Jan",
        LastName = "Kowalski",
        Sex = Sex.Male,
        Age = 30
    });
    contact = await context.Contacts.FindAsync(result.Id);
}

// ====================================================================
// ZADANIE 6: Dodawanie i usuwanie emaila z kontaktu
// ZADANIE 2: Walidacja emaila przez regex w EmailAddress
// ====================================================================
contactServices.AddEmail(new AddEmailDTO
{
    ContactId = contact!.Id,
    Address = "jan.kowalski@example.com",
    CategoryId = newsletter.Id
});

Console.WriteLine("Po dodaniu emaili:");
var withEmails = context.Contacts.Include(c => c.Emails).First(c => c.Id == contact!.Id);
foreach (var e in withEmails.Emails)
{
    Console.WriteLine($" - {e.Address.Address} (kat: {e.CategoryId})");
}

// Usuniecie emaila
contactServices.RemoveEmail(new RemoveEmailDTO
{
    ContactId = contact.Id,
    Address = "jan.kowalski@example.com"
});

Console.WriteLine("Po usunieciu emaila:");
withEmails = context.Contacts.Include(c => c.Emails).First(c => c.Id == contact.Id);
foreach (var e in withEmails.Emails)
{
    Console.WriteLine($" - {e.Address.Address} (kat: {e.CategoryId})");
}

// ====================================================================
// ZADANIE 7: Dodawanie i usuwanie telefonu do kontaktu
// Obiekty: PhoneNumber (value object), Phone (entity)
// ====================================================================
contactServices.AddPhone(new AddPhoneDTO
{
    ContactId = contact!.Id,
    Number = "+48 123 456 789",
    CategoryId = newsletter.Id
});

// Wyswietlanie telefonow
var withPhones = context.Contacts.Include(c => c.Phones).ThenInclude(p => p.Category).First(c => c.Id == contact!.Id);
Console.WriteLine($"\nTelefony {withPhones.FirstName.Value}:");
foreach (var p in withPhones.Phones)
{
    Console.WriteLine($" - {p.Number.Number} [{p.Category.Name}]");
}

// ====================================================================
// ZADANIE 8: Dodawanie, edycja i usuwanie waznych dat kontaktu
// Obiekty: ImportantDate (entity), DateType (enum)
// ====================================================================
contactServices.AddImportantDate(new AddImportantDateDTO
{
    ContactId = contact!.Id,
    Date = new DateTime(1990, 5, 15),
    Type = DateType.Birthday,
    Description = "Urodziny Jana"
});

// Wyswietlanie waznych dat
var withDates = context.Contacts.Include(c => c.ImportantDates).First(c => c.Id == contact!.Id);
Console.WriteLine($"\nWazne daty dla {withDates.FirstName.Value}:");
foreach (var d in withDates.ImportantDates)
{
    Console.WriteLine($" - {d.Date:dd.MM.yyyy} [{d.Type}]: {d.Description}");
}






