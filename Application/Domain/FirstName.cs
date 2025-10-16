using System;

namespace Contacts.Application.Domain;

public class FirstName
{
    public string Value { get; private set; } = null!;
    private FirstName() { }

    public FirstName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException(nameof(value));
        Value = value.Trim();
    }

    public override string ToString() => Value;
}