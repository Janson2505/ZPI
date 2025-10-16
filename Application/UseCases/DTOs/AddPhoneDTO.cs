namespace Contacts.Application.UseCases.DTOs;

public class AddPhoneDTO
{
    public int ContactId { get; init; }
    public string Number { get; init; } = string.Empty;
    public int CategoryId { get; init; }
}
