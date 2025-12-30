using RideSharing.Domain.Enums;
using RideSharing.Domain.Interfaces;
using RideSharing.Domain.ValueObjects;
using RideSharing.Domain.Exceptions;

namespace RideSharing.Domain.Entities;

/// <summary>
/// Represents a payment transaction
/// </summary>
public class Payment
{
    private readonly List<INotificationObserver> _observers = new();

    public Guid Id { get; private set; }
    public Trip Trip { get; private set; }
    public Money Amount { get; private set; }
    public PaymentMethod Method { get; private set; }
    public PaymentStatus Status { get; private set; }
    public string? TransactionId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? ProcessedAt { get; private set; }
    public string? FailureReason { get; private set; }
    public int RetryCount { get; private set; }

    public Payment(Trip trip, Money amount, PaymentMethod method)
    {
        Id = Guid.NewGuid();
        Trip = trip ?? throw new ArgumentNullException(nameof(trip));
        Amount = amount ?? throw new ArgumentNullException(nameof(amount));
        Method = method;
        Status = PaymentStatus.Pending;
        CreatedAt = DateTime.UtcNow;
        RetryCount = 0;
    }

    public void MarkProcessing()
    {
        if (Status != PaymentStatus.Pending && Status != PaymentStatus.Failed)
            throw new InvalidOperationException($"Cannot process payment in {Status} state");

        Status = PaymentStatus.Processing;
    }

    public void MarkSuccess(string transactionId)
    {
        if (Status != PaymentStatus.Processing)
            throw new InvalidOperationException($"Cannot mark success from {Status} state");

        Status = PaymentStatus.Success;
        TransactionId = transactionId;
        ProcessedAt = DateTime.UtcNow;
        NotifyObservers("PaymentSuccess");
    }

    public void MarkFailed(string reason)
    {
        if (Status != PaymentStatus.Processing)
            throw new InvalidOperationException($"Cannot mark failed from {Status} state");

        Status = PaymentStatus.Failed;
        FailureReason = reason;
        RetryCount++;
        NotifyObservers("PaymentFailed");
    }

    public void Refund()
    {
        if (Status != PaymentStatus.Success)
            throw new InvalidOperationException("Can only refund successful payments");

        Status = PaymentStatus.Refunded;
        NotifyObservers("PaymentRefunded");
    }

    public void PutOnHold()
    {
        Status = PaymentStatus.OnHold;
    }

    public bool CanRetry()
    {
        return Status == PaymentStatus.Failed && RetryCount < 3;
    }

    public void AddObserver(INotificationObserver observer)
    {
        if (!_observers.Contains(observer))
        {
            _observers.Add(observer);
        }
    }

    public void RemoveObserver(INotificationObserver observer)
    {
        _observers.Remove(observer);
    }

    private void NotifyObservers(string eventType)
    {
        foreach (var observer in _observers)
        {
            observer.OnPaymentEvent(this, eventType);
        }
    }

    public override string ToString()
    {
        return $"Payment {Id.ToString().Substring(0, 8)} - {Amount} - {Status}";
    }
}
