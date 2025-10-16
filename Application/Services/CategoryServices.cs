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
        if (category == null)
            throw new ServiceException($"Kategoria o id: {categoryID} nie istnieje");

        // NOWE: blokada usunięcia, jeśli są emaile przypisane do tej kategorii
        if (categoryRepository.HasEmails(categoryID))
            throw new ServiceException("Nie można usunąć kategorii: są do niej przypisane adresy email. Najpierw przenieś lub usuń te adresy.");

        categoryRepository.RemoveCategory(category);
        unitOfWork.Save();
    }

    public void UpdateCategory(UpdateCategoryDTO dto)
    {
        var category = categoryRepository.GetCategory(dto.Id);
        if (category == null)
            throw new ServiceException($"Kategoria o id: {dto.Id} nie istnieje");

        var duplicate = categoryRepository.GetCategoryByName(dto.Name);
        if (duplicate != null && duplicate.Id != dto.Id)
            throw new ServiceException("Kategoria o tej nazwie już istnieje");

        category.Rename(dto.Name);
        unitOfWork.Save();
    }
}