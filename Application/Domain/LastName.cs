using System;

namespace Contacts.Application.Domain;

public class LastName
{
    public string Value { get; private set; } = null!;

    private LastName() { }

    public LastName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException(nameof(value));
        Value = value.Trim();
    }

    public override string ToString() => Value;
}