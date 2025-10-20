using System.Collections.Generic;
using VendingMachineApp.Domain.Models;

namespace VendingMachineApp.Domain.Interfaces
{
    public interface IInventoryRepository
    {
        IEnumerable<Slot> GetSlots();
        Slot? GetSlot(string slotId);
        void AddSlot(Slot slot);
        void AddProductToSlot(string slotId, int quantity);
    }
}
