using System;
using System.Collections.Generic;
using System.Linq;
using VendingMachineApp.Domain.Enums;
using VendingMachineApp.Domain.Interfaces;

namespace VendingMachineApp.Infrastructure.Hardware
{
    /// <summary>
    /// Simple coin acceptor that:
    /// - Tracks inserted coins in a temporary list (current transaction)
    /// - Keeps an internal reservoir for change-making
    /// - Uses greedy algorithm to make change (sufficient for canonical US coins)
    /// </summary>
    public class SimpleCoinAcceptor : ICoinAcceptor
    {
        private readonly object _lock = new();
        private readonly Dictionary<Coin, int> _reservoir = new();
        private readonly List<Coin> _currentTransactionCoins = new();

        public SimpleCoinAcceptor()
        {
            // init reservoir with zero counts
            foreach (Coin c in Enum.GetValues(typeof(Coin)))
                _reservoir[c] = 0;
        }

        public decimal CurrentBalance
        {
            get
            {
                lock (_lock)
                {
                    return _currentTransactionCoins.Sum(c => c.ToDecimal());
                }
            }
        }

        public void InsertCoin(Coin coin)
        {
            lock (_lock)
            {
                _currentTransactionCoins.Add(coin);
            }
        }

        public Dictionary<Coin, int>? MakeChange(int changeInCents)
        {
            lock (_lock)
            {
                if (changeInCents == 0) return new Dictionary<Coin, int>();

                // Work on a temp copy of reservoir to try greedy change
                var temp = _reservoir.ToDictionary(k => k.Key, v => v.Value);
                var result = new Dictionary<Coin, int>();
                var coinTypes = Enum.GetValues(typeof(Coin))
                                    .Cast<Coin>()
                                    .OrderByDescending(c => c.ToCents());

                int remaining = changeInCents;
                foreach (var coin in coinTypes)
                {
                    int denom = coin.ToCents();
                    int available = temp[coin];
                    if (available <= 0) continue;
                    int needed = Math.Min(available, remaining / denom);
                    if (needed > 0)
                    {
                        result[coin] = needed;
                        remaining -= needed * denom;
                        temp[coin] -= needed;
                    }
                }

                if (remaining != 0)
                {
                    return null; // cannot make exact change
                }

                // Update reservoir to remove coins used for change (but don't commit until caller commits transaction)
                foreach (var kv in result)
                    _reservoir[kv.Key] -= kv.Value;

                return result;
            }
        }

        public void CommitTransaction()
        {
            lock (_lock)
            {
                // Move current inserted coins into reservoir
                foreach (var c in _currentTransactionCoins)
                    _reservoir[c] += 1;
                _currentTransactionCoins.Clear();
            }
        }

        public Dictionary<Coin, int> Refund()
        {
            lock (_lock)
            {
                var map = new Dictionary<Coin, int>();
                foreach (var c in _currentTransactionCoins)
                {
                    if (!map.ContainsKey(c)) map[c] = 0;
                    map[c]++;
                }

                _currentTransactionCoins.Clear();
                return map;
            }
        }

        public void RefillCoin(Coin coin, int count)
        {
            lock (_lock)
            {
                _reservoir[coin] = _reservoir.TryGetValue(coin, out var v) ? v + count : count;
            }
        }
    }
}
