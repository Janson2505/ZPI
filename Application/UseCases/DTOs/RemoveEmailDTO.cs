namespace Contacts.Application.UseCases.DTOs;

public class RemoveEmailDTO
{
    public int ContactId { get; init; }
    // Either EmailId or Address must be provided. Prefer EmailId when present.
    public int? EmailId { get; init; }
    public string? Address { get; init; }
}
