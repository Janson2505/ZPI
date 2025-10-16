namespace Contacts.Application.UseCases.DTOs;

public class RemovePhoneDTO
{
    public int ContactId { get; init; }
    public int? PhoneId { get; init; }
    public string? Number { get; init; }
}
