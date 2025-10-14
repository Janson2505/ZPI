namespace Contacts.Application.Queries;



using Contacts.Application.Queries.DTOs;



public interface ICategoryQueries

{

    IEnumerable<CategoryDTO> GetCategories();

    CategoryDTO? GetCategory(int categoryId);

}