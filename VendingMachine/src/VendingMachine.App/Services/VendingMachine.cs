using System;
using System.Collections.Generic;
using System.Linq;
using VendingMachineApp.Domain.Enums;
using VendingMachineApp.Domain.Interfaces;
using VendingMachineApp.DTOs;
using VendingMachineApp.Exceptions;

namespace VendingMachineApp.Services
{
    /// <summary>
    /// Coordinates operations: accept coins, select product, dispense and give change.
    /// Business logic resides here.
    /// </summary>
    public class VendingMachine
    {
        private readonly IInventoryRepository _inventory;
        private readonly ICoinAcceptor _coinAcceptor;
        private readonly IDispenser _dispenser;
        private readonly IDisplay _display;
        private readonly object _transactionLock = new();

        public VendingMachine(IInventoryRepository inventory, ICoinAcceptor coinAcceptor, IDispenser dispenser, IDisplay display)
        {
            _inventory = inventory;
            _coinAcceptor = coinAcceptor;
            _dispenser = dispenser;
            _display = display;
        }

        public void InsertCoin(Coin coin)
        {
            _coinAcceptor.InsertCoin(coin);
        }

        public TransactionResult SelectProduct(string slotId)
        {
            lock (_transactionLock)
            {
                var slot = _inventory.GetSlot(slotId);
                if (slot == null)
                    return TransactionResult.Fail("Invalid slot");

                if (slot.Quantity <= 0)
                    return TransactionResult.Fail("Out of stock");

                var price = slot.Product.Price;
                var balance = _coinAcceptor.CurrentBalance;

                if (balance < price)
                    return TransactionResult.Fail($"Need ${price - balance:0.00} more");

                // Calculate change in cents
                int paidCents = (int)Math.Round(balance * 100m);
                int priceCents = (int)Math.Round(price * 100m);
                int change = paidCents - priceCents;

                // Try to make change, if required
                Dictionary<Domain.Enums.Coin, int>? changeCoins = null;
                if (change > 0)
                {
                    changeCoins = _coinAcceptor.MakeChange(change);
                    if (changeCoins == null)
                    {
                        return TransactionResult.Fail("Cannot provide exact change. Please cancel or insert exact amount.");
                    }
                }

                // Commit transaction (move inserted coins into reservoir)
                _coinAcceptor.CommitTransaction();

                // Attempt to dispense product
                var dispensed = slot.TryDecrement();
                if (!dispensed)
                {
                    // rollback: cannot dispense
                    // (In real machine we might re-credit coin acceptor's transaction state or physically refund)
                    return TransactionResult.Fail("Dispense failed (mechanical). Contact support.");
                }

                var dispensedOk = _dispenser.Dispense(slot);
                if (!dispensedOk)
                {
                    // If dispenser failed, restore inventory and ideally refund
                    slot.Add(1);
                    throw new VendingException("Dispenser hardware failure");
                }

                // Return change amount decimal
                decimal changeAmount = change / 100m;
                _display.Show($"Thank you! Dispensed {slot.Product.Name}.");

                return TransactionResult.Success(slot.Product.Name, changeAmount);
            }
        }

        public decimal Cancel()
        {
            lock (_transactionLock)
            {
                var refundMap = _coinAcceptor.Refund();
                decimal total = refundMap.Sum(kv => kv.Key.ToDecimal() * kv.Value);
                return total;
            }
        }

        // Admin functions
        public void RefillProduct(string slotId, int qty) => _inventory.AddProductToSlot(slotId, qty);
        public void RefillCoins(Coin coin, int qty) => _coinAcceptor.RefillCoin(coin, qty);
    }
}
