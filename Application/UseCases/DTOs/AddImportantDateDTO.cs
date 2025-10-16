using Contacts.Application.Domain;

namespace Contacts.Application.UseCases.DTOs;

public class AddImportantDateDTO
{
    public int ContactId { get; init; }
    public DateTime Date { get; init; }
    public DateType Type { get; init; }
    public string Description { get; init; } = string.Empty;
}
