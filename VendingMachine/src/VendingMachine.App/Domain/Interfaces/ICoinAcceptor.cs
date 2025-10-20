using System.Collections.Generic;
using VendingMachineApp.Domain.Enums;

namespace VendingMachineApp.Domain.Interfaces
{
    public interface ICoinAcceptor
    {
        /// <summary>Insert a coin into the acceptor (adds to current transaction balance)</summary>
        void InsertCoin(Coin coin);

        /// <summary>Return currently inserted balance (for display)</summary>
        decimal CurrentBalance { get; }

        /// <summary>Attempt to make change (in cents). Returns dictionary of coins to dispense as change, or null if cannot make exact change.</summary>
        Dictionary<Coin, int>? MakeChange(int changeInCents);

        /// <summary>Deduct coins from internal reservoir when accepted to machine (used after successful purchase to move coins into reservoir)</summary>
        void CommitTransaction();

        /// <summary>Refund the current inserted coins (returns mapping and resets balance)</summary>
        Dictionary<Coin, int> Refund();

        /// <summary>Refill coin reservoir used for change-making</summary>
        void RefillCoin(Coin coin, int count);
    }
}
