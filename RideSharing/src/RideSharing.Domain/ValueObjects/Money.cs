namespace RideSharing.Domain.ValueObjects;

/// <summary>
/// Represents a monetary value
/// Value Object - immutable
/// </summary>
public record Money
{
    public decimal Amount { get; init; }
    public string Currency { get; init; }

    public Money(decimal amount, string currency = "USD")
    {
        if (amount < 0)
            throw new ArgumentException("Amount cannot be negative", nameof(amount));

        if (string.IsNullOrWhiteSpace(currency))
            throw new ArgumentException("Currency is required", nameof(currency));

        Amount = amount;
        Currency = currency.ToUpper();
    }

    public static Money Zero(string currency = "USD") => new(0, currency);

    public Money Add(Money other)
    {
        ValidateSameCurrency(other);
        return new Money(Amount + other.Amount, Currency);
    }

    public Money Subtract(Money other)
    {
        ValidateSameCurrency(other);
        if (Amount < other.Amount)
            throw new InvalidOperationException("Cannot subtract larger amount from smaller amount");
        return new Money(Amount - other.Amount, Currency);
    }

    public Money Multiply(decimal multiplier)
    {
        if (multiplier < 0)
            throw new ArgumentException("Multiplier cannot be negative", nameof(multiplier));
        return new Money(Amount * multiplier, Currency);
    }

    public bool IsGreaterThan(Money other)
    {
        ValidateSameCurrency(other);
        return Amount > other.Amount;
    }

    public bool IsLessThan(Money other)
    {
        ValidateSameCurrency(other);
        return Amount < other.Amount;
    }

    private void ValidateSameCurrency(Money other)
    {
        if (Currency != other.Currency)
            throw new InvalidOperationException($"Cannot operate on different currencies: {Currency} and {other.Currency}");
    }

    // Operator overloads
    public static Money operator +(Money left, Money right) => left.Add(right);
    public static Money operator -(Money left, Money right) => left.Subtract(right);
    public static Money operator *(Money money, decimal multiplier) => money.Multiply(multiplier);
    public static Money operator *(decimal multiplier, Money money) => money.Multiply(multiplier);
    public static bool operator >(Money left, Money right) => left.IsGreaterThan(right);
    public static bool operator <(Money left, Money right) => left.IsLessThan(right);
    public static bool operator >=(Money left, Money right) => left.Amount >= right.Amount;
    public static bool operator <=(Money left, Money right) => left.Amount <= right.Amount;

    public override string ToString()
    {
        return $"{Amount:F2} {Currency}";
    }
}
