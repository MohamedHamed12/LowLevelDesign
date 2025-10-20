namespace VendingMachineApp.DTOs
{
    public sealed class TransactionResult
    {
        public bool Success { get; }
        public string Message { get; }
        public string? ProductName { get; }
        public decimal ChangeAmount { get; }

        private TransactionResult(bool success, string message, string? productName = null, decimal changeAmount = 0m)
        {
            Success = success;
            Message = message;
            ProductName = productName;
            ChangeAmount = changeAmount;
        }

        public static TransactionResult Success(string productName, decimal changeAmount)
            => new(true, "OK", productName, changeAmount);

        public static TransactionResult Fail(string reason) => new(false, reason);
    }
}
