using RideSharing.Domain.ValueObjects;

namespace RideSharing.Domain.Entities;

/// <summary>
/// Represents a rating given by one user to another
/// </summary>
public class Rating
{
    public Guid Id { get; private set; }
    public Trip Trip { get; private set; }
    public User RatedBy { get; private set; }
    public User RatedUser { get; private set; }
    public RatingScore Score { get; private set; }
    public string? Comment { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public Rating(Trip trip, User ratedBy, User ratedUser, RatingScore score, string? comment = null)
    {
        if (trip == null)
            throw new ArgumentNullException(nameof(trip));

        if (trip.Status != Enums.TripStatus.Completed)
            throw new InvalidOperationException("Can only rate completed trips");

        Id = Guid.NewGuid();
        Trip = trip;
        RatedBy = ratedBy ?? throw new ArgumentNullException(nameof(ratedBy));
        RatedUser = ratedUser ?? throw new ArgumentNullException(nameof(ratedUser));
        Score = score;
        Comment = comment;
        CreatedAt = DateTime.UtcNow;

        // Update the rated user's average rating
        ratedUser.UpdateRating(score);
    }

    public void UpdateComment(string newComment)
    {
        Comment = newComment;
    }

    public override string ToString()
    {
        return $"{RatedBy.Name} rated {RatedUser.Name}: {Score}";
    }
}
