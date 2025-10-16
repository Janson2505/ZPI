using Contacts.Application.Domain;
using Contacts.Application.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Contacts.Data;

public class ContactRepository(AppDbContext context) : IContactRepository
{
    public IEnumerable<Contact> GetContacts()
        => context.Contacts;

    public Contact? GetContact(int contactId)
        => context.Contacts.Find(contactId);

    public Contact? GetContactWithEmails(int contactId)
        => context.Contacts
            .Include(c => c.Emails)
            .SingleOrDefault(c => c.Id == contactId);

    public void Add(Contact contact)
        => context.Contacts.Add(contact);

    public void Remove(Contact contact)
        => context.Contacts.Remove(contact);

    public void AddEmail(Email email)
        => context.Emails.Add(email);

    public Email? GetEmail(int emailId)
        => context.Emails
            .Include(e => e.Contact)
            .SingleOrDefault(e => e.Id == emailId);

    public void RemoveEmail(Email email)
        => context.Emails.Remove(email);

    public Contact? GetContactWithPhones(int contactId)
        => context.Contacts
            .Include(c => c.Phones)
            .SingleOrDefault(c => c.Id == contactId);

    public void AddPhone(Phone phone)
        => context.Phones.Add(phone);

    public Phone? GetPhone(int phoneId)
        => context.Phones
            .Include(p => p.Contact)
            .SingleOrDefault(p => p.Id == phoneId);

    public void RemovePhone(Phone phone)
        => context.Phones.Remove(phone);

    public Contact? GetContactWithImportantDates(int contactId)
        => context.Contacts
            .Include(c => c.ImportantDates)
            .SingleOrDefault(c => c.Id == contactId);

    public void AddImportantDate(ImportantDate importantDate)
        => context.ImportantDates.Add(importantDate);

    public ImportantDate? GetImportantDate(int dateId)
        => context.ImportantDates
            .Include(d => d.Contact)
            .SingleOrDefault(d => d.Id == dateId);

    public void RemoveImportantDate(ImportantDate importantDate)
        => context.ImportantDates.Remove(importantDate);
}