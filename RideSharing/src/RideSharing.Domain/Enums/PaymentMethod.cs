namespace RideSharing.Domain.Enums;

/// <summary>
/// Represents available payment methods
/// </summary>
public enum PaymentMethod
{
    /// <summary>
    /// Cash payment
    /// </summary>
    Cash,

    /// <summary>
    /// Credit or debit card
    /// </summary>
    Card,

    /// <summary>
    /// Digital wallet (app wallet)
    /// </summary>
    Wallet,

    /// <summary>
    /// UPI payment
    /// </summary>
    UPI,

    /// <summary>
    /// Net banking
    /// </summary>
    NetBanking
}
