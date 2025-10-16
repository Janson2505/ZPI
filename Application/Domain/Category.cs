namespace Contacts.Application.Domain;
public class Category(string name, int id)
{
    public Category(string name)
        : this(name, 0)
    {
    }

    public int Id { get; private set; } = id;
    public string Name { get; private set; } = Validate(name);

    public void Rename(string newName)
    {
        Name = Validate(newName);
    }

    private static string Validate(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException(nameof(value));
        return value.Trim();
    }
}