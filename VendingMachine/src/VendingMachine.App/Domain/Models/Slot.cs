using System.Threading;

namespace VendingMachineApp.Domain.Models
{
    public sealed class Slot
    {
        private int _quantity;
        private readonly object _lock = new();

        public string SlotId { get; }
        public Product Product { get; }
        public int Quantity => _quantity;

        public Slot(string slotId, Product product, int initialQuantity)
        {
            SlotId = slotId;
            Product = product;
            _quantity = initialQuantity;
        }

        public bool TryDecrement()
        {
            lock (_lock)
            {
                if (_quantity <= 0) return false;
                _quantity--;
                return true;
            }
        }

        public void Add(int qty)
        {
            if (qty <= 0) return;
            lock (_lock) { _quantity += qty; }
        }
    }
}
