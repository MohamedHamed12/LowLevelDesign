using VendingMachineApp.Domain.Models;

namespace VendingMachineApp.Domain.Interfaces
{
    public interface IDispenser
    {
        /// <summary>Physically dispense the product. Returns true if successful.</summary>
        bool Dispense(Slot slot);
    }
}
