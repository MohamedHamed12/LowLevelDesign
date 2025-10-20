namespace VendingMachineApp.Domain.Models
{
    public sealed class Product
    {
        public string Name { get; }
        public string Sku { get; }
        public decimal Price { get; }
        public Product(string name, string sku, decimal price)
        {
            Name = name;
            Sku = sku;
            Price = price;
        }
    }
}