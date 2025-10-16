using Contacts.Application.Domain;

namespace Contacts.Application.Repositories;

public interface IContactRepository
{
    IEnumerable<Contact> GetContacts();
    Contact? GetContact(int contactId);
    Contact? GetContactWithEmails(int contactId);
    Contact? GetContactWithPhones(int contactId);
    Contact? GetContactWithImportantDates(int contactId);
    void Add(Contact contact);
    void Remove(Contact contact);
    // operacje na emailach
    void AddEmail(Email email);
    Email? GetEmail(int emailId);
    void RemoveEmail(Email email);
    // operacje na telefonach
    void AddPhone(Phone phone);
    Phone? GetPhone(int phoneId);
    void RemovePhone(Phone phone);
    // operacje na waznych datach
    void AddImportantDate(ImportantDate importantDate);
    ImportantDate? GetImportantDate(int dateId);
    void RemoveImportantDate(ImportantDate importantDate);
}