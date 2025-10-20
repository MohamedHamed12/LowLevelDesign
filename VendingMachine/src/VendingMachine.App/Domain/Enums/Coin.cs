namespace VendingMachineApp.Domain.Enums
{
    // Use cents as base unit to avoid floating point issues
    public enum Coin
    {
        Cent5 = 5,
        Cent10 = 10,
        Cent25 = 25,
        Cent50 = 50,
        Dollar1 = 100
    }

    public static class CoinExtensions
    {
        public static decimal ToDecimal(this Coin coin) => ((int)coin) / 100m;
        public static int ToCents(this Coin coin) => (int)coin;
    }
}
