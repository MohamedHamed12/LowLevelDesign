namespace RideSharing.Domain.ValueObjects;

/// <summary>
/// Represents a rating score
/// Value Object - immutable
/// </summary>
public record RatingScore
{
    public double Value { get; init; }

    public RatingScore(double value)
    {
        if (value < 1.0 || value > 5.0)
            throw new ArgumentException("Rating must be between 1.0 and 5.0", nameof(value));

        Value = Math.Round(value, 2);
    }

    public static RatingScore FromStars(int stars)
    {
        if (stars < 1 || stars > 5)
            throw new ArgumentException("Stars must be between 1 and 5", nameof(stars));

        return new RatingScore(stars);
    }

    public static RatingScore Default() => new(5.0);

    public bool IsExcellent() => Value >= 4.5;
    public bool IsGood() => Value >= 4.0 && Value < 4.5;
    public bool IsAverage() => Value >= 3.0 && Value < 4.0;
    public bool IsPoor() => Value < 3.0;

    public override string ToString()
    {
        return $"{Value:F2} ★";
    }
}
