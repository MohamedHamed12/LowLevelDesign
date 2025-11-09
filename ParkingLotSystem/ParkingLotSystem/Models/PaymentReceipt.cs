namespace ParkingLotSystem.Models;

public class PaymentReceipt
{
    public string ReceiptId { get; }
    public ParkingTicket Ticket { get; }
    public DateTime ExitTime { get; }
    public decimal Amount { get; }

    public PaymentReceipt(ParkingTicket ticket, DateTime exitTime, decimal amount)
    {
        ReceiptId = Guid.NewGuid().ToString();
        Ticket = ticket ?? throw new ArgumentNullException(nameof(ticket));
        ExitTime = exitTime;
        Amount = amount;
    }

    public override string ToString() 
        => $"Receipt {ReceiptId}: {Ticket.Vehicle.LicensePlate} - ${Amount:F2} " +
           $"(Duration: {(ExitTime - Ticket.EntryTime).TotalHours:F2}h)";
}
