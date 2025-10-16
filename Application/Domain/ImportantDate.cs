namespace Contacts.Application.Domain;

public class ImportantDate
{
    public int Id { get; private set; }
    public DateTime Date { get; private set; }
    public DateType Type { get; private set; }
    public string Description { get; private set; } = string.Empty;
    public Contact Contact { get; private set; } = null!;
    public int ContactId { get; private set; }
    
    private ImportantDate()
    {
    }
    
    public ImportantDate(int id, Contact contact, DateTime date, DateType type, string description)
    {
        Id = id;
        ContactId = contact.Id;
        Contact = contact;
        Date = date;
        Type = type;
        Description = string.IsNullOrWhiteSpace(description) 
            ? string.Empty 
            : description.Trim();
    }
    
    public void Update(DateTime date, DateType type, string description)
    {
        Date = date;
        Type = type;
        Description = string.IsNullOrWhiteSpace(description) 
            ? string.Empty 
            : description.Trim();
    }
}
