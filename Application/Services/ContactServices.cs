using Contacts.Application.Domain;
using Contacts.Application.Repositories;
using Contacts.Application.UseCases;
using Contacts.Application.UseCases.DTOs;

namespace Contacts.Application.Services;

public class ContactServices(
    IContactRepository contactRepository,
    ICategoryRepository categoryRepository,
    IUnitOfWork unitOfWork
) : IContactUseCases
{
    public AddContactResult AddContact(AddContactDTO dto)
    {
        // „studencko”: tworzymy kontakt bez maili, z pustą listą
        var contact = new Contact(
            id: 0,
            firstName: dto.FirstName,
            lastName: dto.LastName,
            sex: dto.Sex,
            emails: new List<Email>(),
            age: new Age(dto.Age)
        );

        contactRepository.Add(contact);
        unitOfWork.Save();
        return contact;
    }

    public void UpdateContact(UpdateContactDTO dto)
    {
        var contact = contactRepository.GetContact(dto.Id);
        if (contact is null)
            throw new ServiceException($"Kontakt o id: {dto.Id} nie istnieje");

        contact.Update(dto.FirstName, dto.LastName, dto.Sex, new Age(dto.Age));
        unitOfWork.Save();
    }

    public void RemoveContact(int contactId)
    {
        var contact = contactRepository.GetContact(contactId);
        if (contact is null)
            throw new ServiceException($"Kontakt o id: {contactId} nie istnieje");

        // Prosto: usuwamy kontakt; EF domyślnie kaskadowo usunie jego emaile
        contactRepository.Remove(contact);
        unitOfWork.Save();
    }

    public void AddEmail(AddEmailDTO dto)
    {
        var contact = contactRepository.GetContactWithEmails(dto.ContactId);
        if (contact is null)
            throw new ServiceException($"Kontakt o id: {dto.ContactId} nie istnieje");

        var category = categoryRepository.GetCategory(dto.CategoryId)
            ?? throw new ServiceException($"Kategoria o id: {dto.CategoryId} nie istnieje");

        var emailAddress = new EmailAddress(dto.Address);
        var email = new Email(
            id: 0,
            contact: contact,
            category: category,
            email: emailAddress
        );

        // Domain-level duplicate guard
        contact.AddEmail(email);
        // Ensure EF tracks new email as well
        contactRepository.AddEmail(email);
        unitOfWork.Save();
    }

    public void RemoveEmail(RemoveEmailDTO dto)
    {
        // Prefer lookup by EmailId to avoid duplicates by address
        Email? email = null;
        if (dto.EmailId.HasValue)
        {
            email = contactRepository.GetEmail(dto.EmailId.Value);
        }
        else if (!string.IsNullOrWhiteSpace(dto.Address))
        {
            // When address given, we need contact with emails to find the right email entity
            var contact = contactRepository.GetContactWithEmails(dto.ContactId)
                ?? throw new ServiceException($"Kontakt o id: {dto.ContactId} nie istnieje");
            var addr = new EmailAddress(dto.Address);
            email = contact.Emails.FirstOrDefault(e => e.Address.Address == addr.Address);
        }

        if (email is null)
            throw new ServiceException("Nie znaleziono adresu email do usunięcia.");

        if (email.ContactId != dto.ContactId)
            throw new ServiceException("Podany email nie należy do wskazanego kontaktu.");

        contactRepository.RemoveEmail(email);
        unitOfWork.Save();
    }

    public void AddPhone(AddPhoneDTO dto)
    {
        var contact = contactRepository.GetContactWithPhones(dto.ContactId);
        if (contact is null)
            throw new ServiceException($"Kontakt o id: {dto.ContactId} nie istnieje");

        var category = categoryRepository.GetCategory(dto.CategoryId)
            ?? throw new ServiceException($"Kategoria o id: {dto.CategoryId} nie istnieje");

        var phoneNumber = new PhoneNumber(dto.Number);
        var phone = new Phone(
            id: 0,
            contact: contact,
            category: category,
            number: phoneNumber
        );

        contact.AddPhone(phone);
        contactRepository.AddPhone(phone);
        unitOfWork.Save();
    }

    public void RemovePhone(RemovePhoneDTO dto)
    {
        Phone? phone = null;
        if (dto.PhoneId.HasValue)
        {
            phone = contactRepository.GetPhone(dto.PhoneId.Value);
        }
        else if (!string.IsNullOrWhiteSpace(dto.Number))
        {
            var contact = contactRepository.GetContactWithPhones(dto.ContactId)
                ?? throw new ServiceException($"Kontakt o id: {dto.ContactId} nie istnieje");
            var phoneNum = new PhoneNumber(dto.Number);
            phone = contact.Phones.FirstOrDefault(p => p.Number.Number == phoneNum.Number);
        }

        if (phone is null)
            throw new ServiceException("Nie znaleziono numeru telefonu do usunięcia.");

        if (phone.ContactId != dto.ContactId)
            throw new ServiceException("Podany numer telefonu nie należy do wskazanego kontaktu.");

        contactRepository.RemovePhone(phone);
        unitOfWork.Save();
    }

    public void AddImportantDate(AddImportantDateDTO dto)
    {
        var contact = contactRepository.GetContactWithImportantDates(dto.ContactId);
        if (contact is null)
            throw new ServiceException($"Kontakt o id: {dto.ContactId} nie istnieje");

        var importantDate = new ImportantDate(
            id: 0,
            contact: contact,
            date: dto.Date,
            type: dto.Type,
            description: dto.Description
        );

        contact.AddImportantDate(importantDate);
        contactRepository.AddImportantDate(importantDate);
        unitOfWork.Save();
    }

    public void UpdateImportantDate(UpdateImportantDateDTO dto)
    {
        var importantDate = contactRepository.GetImportantDate(dto.DateId);
        if (importantDate is null)
            throw new ServiceException($"Ważna data o id: {dto.DateId} nie istnieje");

        if (importantDate.ContactId != dto.ContactId)
            throw new ServiceException("Podana data nie należy do wskazanego kontaktu.");

        importantDate.Update(dto.Date, dto.Type, dto.Description);
        unitOfWork.Save();
    }

    public void RemoveImportantDate(RemoveImportantDateDTO dto)
    {
        var importantDate = contactRepository.GetImportantDate(dto.DateId);
        if (importantDate is null)
            throw new ServiceException($"Ważna data o id: {dto.DateId} nie istnieje");

        if (importantDate.ContactId != dto.ContactId)
            throw new ServiceException("Podana data nie należy do wskazanego kontaktu.");

        contactRepository.RemoveImportantDate(importantDate);
        unitOfWork.Save();
    }
}