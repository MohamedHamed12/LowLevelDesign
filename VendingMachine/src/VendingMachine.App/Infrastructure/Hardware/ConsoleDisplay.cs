using System;
using VendingMachineApp.Domain.Interfaces;

namespace VendingMachineApp.Infrastructure.Hardware
{
    public class ConsoleDisplay : IDisplay
    {
        public void Show(string message)
        {
            Console.WriteLine(message);
        }
    }
}
