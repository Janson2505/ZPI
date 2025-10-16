using Contacts.Application.Domain;

namespace Contacts.Application.UseCases.DTOs;

public class UpdateImportantDateDTO
{
    public int ContactId { get; init; }
    public int DateId { get; init; }
    public DateTime Date { get; init; }
    public DateType Type { get; init; }
    public string Description { get; init; } = string.Empty;
}
