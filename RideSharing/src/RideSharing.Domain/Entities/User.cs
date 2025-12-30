using RideSharing.Domain.Enums;
using RideSharing.Domain.ValueObjects;

namespace RideSharing.Domain.Entities;

/// <summary>
/// Base class for all users in the system
/// </summary>
public abstract class User
{
    public Guid Id { get; protected set; }
    public string Name { get; protected set; }
    public string Email { get; protected set; }
    public string PhoneNumber { get; protected set; }
    public string PasswordHash { get; protected set; }
    public UserType UserType { get; protected set; }
    public RatingScore Rating { get; protected set; }
    public bool IsActive { get; protected set; }
    public DateTime CreatedAt { get; protected set; }
    public DateTime? LastLoginAt { get; protected set; }

    protected User(string name, string email, string phoneNumber, string passwordHash, UserType userType)
    {
        Id = Guid.NewGuid();
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Email = email ?? throw new ArgumentNullException(nameof(email));
        PhoneNumber = phoneNumber ?? throw new ArgumentNullException(nameof(phoneNumber));
        PasswordHash = passwordHash ?? throw new ArgumentNullException(nameof(passwordHash));
        UserType = userType;
        Rating = RatingScore.Default();
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
    }

    public virtual void UpdateProfile(string name, string email, string phoneNumber)
    {
        Name = name ?? Name;
        Email = email ?? Email;
        PhoneNumber = phoneNumber ?? PhoneNumber;
    }

    public void UpdateRating(RatingScore newRating)
    {
        Rating = newRating;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void Activate()
    {
        IsActive = true;
    }

    public void RecordLogin()
    {
        LastLoginAt = DateTime.UtcNow;
    }

    public override string ToString()
    {
        return $"{Name} ({Email})";
    }
}
