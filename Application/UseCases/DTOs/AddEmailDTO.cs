using Contacts.Application.Domain;

namespace Contacts.Application.UseCases.DTOs;

public class AddEmailDTO
{
    public int ContactId { get; init; }
    public string Address { get; init; } = string.Empty;
    public int CategoryId { get; init; }
}
