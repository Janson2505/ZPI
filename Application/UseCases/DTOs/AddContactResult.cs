using Contacts.Application.Domain;

namespace Contacts.Application.UseCases.DTOs;

public class AddContactResult
{
    public int Id           { get; init; }
    public string FirstName { get; init; } = string.Empty;
    public string LastName  { get; init; } = string.Empty;
    public Sex Sex          { get; init; }
    public int Age          { get; init; }

    public static implicit operator AddContactResult(Contact c)
        => new AddContactResult
        {
            Id = c.Id,
            FirstName = c.FirstName.Value,
            LastName = c.LastName.Value,
            Sex = c.Sex,
            Age = c.Age.Value
        };
}