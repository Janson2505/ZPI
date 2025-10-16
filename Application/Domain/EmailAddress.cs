namespace Contacts.Application.Domain;

using System;
using System.Text.RegularExpressions;

public class EmailAddress
{
    private static readonly Regex EmailRegex =
        new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

    public string Address { get; private set; } = string.Empty;

    // For EF Core materialization
    private EmailAddress() { }

    public EmailAddress(string address)
    {
        if (string.IsNullOrWhiteSpace(address) || !EmailRegex.IsMatch(address))
            throw new ArgumentException("Invalid email address format.", nameof(address));

        Address = address.Trim();
    }

    public static implicit operator string(EmailAddress address) => address.Address;
    public static implicit operator EmailAddress(string text) => new(text);
}