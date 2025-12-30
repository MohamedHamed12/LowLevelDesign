using RideSharing.Domain.Entities;
using RideSharing.Domain.ValueObjects;

namespace RideSharing.Application.Services;

/// <summary>
/// Service for managing ratings
/// </summary>
public class RatingService
{
    private readonly List<Rating> _ratings; // In real app, repository

    public RatingService()
    {
        _ratings = new List<Rating>();
    }

    public Rating RateDriver(Trip trip, Rider rider, RatingScore score, string? comment = null)
    {
        if (trip.Driver == null)
            throw new InvalidOperationException("Trip has no assigned driver");

        var rating = new Rating(trip, rider, trip.Driver, score, comment);
        _ratings.Add(rating);
        
        return rating;
    }

    public Rating RateRider(Trip trip, Driver driver, RatingScore score, string? comment = null)
    {
        var rating = new Rating(trip, driver, trip.Rider, score, comment);
        _ratings.Add(rating);
        
        return rating;
    }

    public List<Rating> GetRatingsForUser(User user)
    {
        return _ratings
            .Where(r => r.RatedUser.Id == user.Id)
            .OrderByDescending(r => r.CreatedAt)
            .ToList();
    }

    public double GetAverageRating(User user)
    {
        var userRatings = GetRatingsForUser(user);
        
        if (!userRatings.Any())
            return 5.0;

        return userRatings.Average(r => r.Score.Value);
    }
}
