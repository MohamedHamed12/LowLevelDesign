using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using VendingMachineApp.Domain.Interfaces;
using VendingMachineApp.Domain.Models;

namespace VendingMachineApp.Infrastructure.Repositories
{
    /// <summary>
    /// Simple in-memory implementation for demo & testing.
    /// </summary>
    public class InMemoryInventoryRepository : IInventoryRepository
    {
        private readonly ConcurrentDictionary<string, Slot> _slots = new();

        public IEnumerable<Slot> GetSlots() => _slots.Values.OrderBy(s => s.SlotId);

        public Slot? GetSlot(string slotId)
        {
            _slots.TryGetValue(slotId, out var slot);
            return slot;
        }

        public void AddSlot(Slot slot)
        {
            _slots[slot.SlotId] = slot;
        }

        public void AddProductToSlot(string slotId, int quantity)
        {
            if (_slots.TryGetValue(slotId, out var slot))
            {
                slot.Add(quantity);
            }
            else
            {
                // If slot absent, ignore or create new slot with a placeholder product
                // For simplicity create dumb product
                var p = new Product($"Unknown-{slotId}", $"SKU-{slotId}", 1.00m);
                _slots[slotId] = new Slot(slotId, p, quantity);
            }
        }
    }
}
