using System;

namespace VendingMachineApp.Exceptions
{
    public class VendingException : Exception
    {
        public VendingException() { }
        public VendingException(string message) : base(message) { }
        public VendingException(string message, Exception inner) : base(message, inner) { }
    }
}
