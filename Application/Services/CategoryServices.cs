using Contacts.Application.UseCases;
using Contacts.Application.UseCases.DTOs;
using Contacts.Application.Repositories;
using Contacts.Application.Domain;
namespace Contacts.Application.Services;

public class CategoryServices(
ICategoryRepository categoryRepository,
IUnitOfWork unitOfWork
) : ICategoryUseCases
{
    public AddCategoryResult AddCategory(AddCategoryDTO categoryDTO)
    {
        if (categoryRepository.GetCategoryByName(categoryDTO.Name) == null)
        {
            var category = (Category)categoryDTO;
            categoryRepository.Add(category);
            unitOfWork.Save();
            return category;
        }
        else
        {
            throw new ServiceException("Kategoria o tej nazwie już istnieje");
        }
    }
    public void RemoveCategory(int categoryID)
    {
        var category = categoryRepository.GetCategory(categoryID);
        if (category != null)
        {
            categoryRepository.RemoveCategory(category);
            unitOfWork.Save();
        }
        else
        {
            throw new ServiceException(
            $"Kategoria o id: {categoryID} nie istnieje");
        }
    }
}