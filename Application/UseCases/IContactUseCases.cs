using Contacts.Application.UseCases.DTOs;

namespace Contacts.Application.UseCases;

public interface IContactUseCases
{
    AddContactResult AddContact(AddContactDTO addContactDTO);
    void UpdateContact(UpdateContactDTO updateContactDTO);
    void RemoveContact(int contactId);
    // zarzadzanie emailami
    void AddEmail(AddEmailDTO dto);
    void RemoveEmail(RemoveEmailDTO dto);
    // zarzadzanie telefonami
    void AddPhone(AddPhoneDTO dto);
    void RemovePhone(RemovePhoneDTO dto);
    // zarzadzanie waznymi datami
    void AddImportantDate(AddImportantDateDTO dto);
    void UpdateImportantDate(UpdateImportantDateDTO dto);
    void RemoveImportantDate(RemoveImportantDateDTO dto);
}