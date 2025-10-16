namespace Contacts.Application.Domain;
public class Contact {
    public int Id {get; private set;}
    public FirstName FirstName {get; private set;} = null!;
    public LastName LastName {get; private set;} = null!;
    public Sex Sex {get; private set;} 
    public ICollection<Email> Emails {get; private set;} = null!;
    public ICollection<Phone> Phones {get; private set;} = null!;
    public ICollection<ImportantDate> ImportantDates {get; private set;} = null!;
    public Age Age {get; private set;} = null!;
    private Contact() {}
    public Contact(int id, string firstName, string lastName,
                Sex sex, ICollection<Email> emails, Age age) {
        Id = id;
        FirstName = new FirstName(firstName);
        LastName = new LastName(lastName);
        Sex = sex;
        Emails = emails ?? throw new ArgumentNullException(nameof(emails));
        Phones = new List<Phone>();
        ImportantDates = new List<ImportantDate>();
        Age = age ?? throw new ArgumentNullException(nameof(age));
    }
        public bool HasEmail(Email email) {
            return Emails.Any(e => e.Address == email.Address);
        }
        public void AddEmail(Email email)
        {
            if (!HasEmail(email))
            {
                Emails.Add(email);
            }
        }

        // sprawdz czy kontakt ma juz taki telefon
        public bool HasPhone(Phone phone)
        {
            return Phones.Any(p => p.Number.Number == phone.Number.Number);
        }

        // dodaj telefon do kontaktu
        public void AddPhone(Phone phone)
        {
            if (!HasPhone(phone))
            {
                Phones.Add(phone);
            }
        }

        // dodaj wazna date do kontaktu
        public void AddImportantDate(ImportantDate importantDate)
        {
            ImportantDates.Add(importantDate);
        }

        // NOWE: prosta aktualizacja podstawowych danych kontaktu
        public void Update(string firstName, string lastName, Sex sex, Age age)
        {
            FirstName = new FirstName(firstName);
            LastName = new LastName(lastName);
            Sex = sex;
            Age = age ?? throw new ArgumentNullException(nameof(age));
        }
}