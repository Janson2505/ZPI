namespace Contacts.Application.Domain;

using System;
using System.Text.RegularExpressions;

public class PhoneNumber
{
    // regex do walidacji numeru telefonu
    private static readonly Regex PhoneRegex =
        new(@"^\+?[1-9]\d{0,3}[-\s]?\(?\d{1,4}\)?[-\s]?\d{1,4}[-\s]?\d{1,9}$", 
            RegexOptions.Compiled | RegexOptions.CultureInvariant);

    public string Number { get; private set; } = string.Empty;

    // konstruktor prywatny dla EF
    private PhoneNumber() { }

    public PhoneNumber(string number)
    {
        if (string.IsNullOrWhiteSpace(number))
            throw new ArgumentException("Phone number cannot be empty.", nameof(number));

        var normalized = number.Trim();
        
        if (!PhoneRegex.IsMatch(normalized))
            throw new ArgumentException("Invalid phone number format.", nameof(number));

        Number = normalized;
    }

    public static implicit operator string(PhoneNumber phoneNumber) => phoneNumber.Number;
    public static implicit operator PhoneNumber(string text) => new(text);
}
