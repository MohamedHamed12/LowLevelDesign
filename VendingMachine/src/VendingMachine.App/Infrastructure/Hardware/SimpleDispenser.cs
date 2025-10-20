using System;
using VendingMachineApp.Domain.Interfaces;
using VendingMachineApp.Domain.Models;

namespace VendingMachineApp.Infrastructure.Hardware
{
    /// <summary>Dummy dispenser - in real world link to hardware</summary>
    public class SimpleDispenser : IDispenser
    {
        public bool Dispense(Slot slot)
        {
            // Simulate mechanical dispense. Always succeed in this demo.
            Console.WriteLine($"[Hardware] Dispensing {slot.Product.Name} from slot {slot.SlotId}...");
            return true;
        }
    }
}
