using System;

namespace SnakeLadder.Core.Domain
{
    public sealed class Dice
    {
        private readonly Random _random;
        public int Sides { get; }

        public Dice(int sides = 6, Random? random = null)
        {
            if (sides < 2) throw new ArgumentOutOfRangeException(nameof(sides));
            Sides = sides;
            _random = random ?? new Random();
        }

        public int Roll() => _random.Next(1, Sides + 1);
    }
}
