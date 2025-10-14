namespace Contacts.Application.Queries.DTOs;



using Contacts.Application.Domain;



public class CategoryDTO

{

    public int Id { get; init; }

    public string Name { get; init; } = string.Empty;



    // Mapowanie domeny -> DTO (jak w materiałach)

    public static implicit operator CategoryDTO(Category category)

        => new CategoryDTO { Id = category.Id, Name = category.Name };

}