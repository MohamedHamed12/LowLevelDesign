using System;

namespace SnakeLadder.Core.Domain
{
    public sealed class Player
    {
        public Guid Id { get; }
        public string Name { get; }
        public int Position { get; private set; }

        public Player(string name)
        {
            Id = Guid.NewGuid();
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Position = 0;
        }

        public void SetPosition(int newPosition)
        {
            if (newPosition < 0) throw new ArgumentOutOfRangeException(nameof(newPosition));
            Position = newPosition;
        }
    }
}
