using System;
using System.Collections.Generic;
using System.Linq;

namespace SnakeLadder.Core.Domain
{
    public sealed class Board
    {
        public int Size { get; }
        public IReadOnlyDictionary<int, int> Jumps { get; } // start -> end (snakes and ladders)

        public Board(int size, IEnumerable<(int start, int end)> snakes, IEnumerable<(int start, int end)> ladders)
        {
            if (size <= 0) throw new ArgumentOutOfRangeException(nameof(size));
            Size = size;

            var dict = new Dictionary<int,int>();

            if (snakes != null)
            {
                foreach (var (s,e) in snakes)
                {
                    ValidateJump(s, e);
                    dict[s] = e;
                }
            }

            if (ladders != null)
            {
                foreach (var (s,e) in ladders)
                {
                    ValidateJump(s, e);
                    dict[s] = e;
                }
            }

            // ensure no conflicting jumps
            if (dict.GroupBy(kv => kv.Key).Any(g => g.Count() > 1))
                throw new ArgumentException("Conflicting jumps on the same start tile.");

            Jumps = dict;
        }

        private void ValidateJump(int start, int end)
        {
            if (start <= 0 || start >= Size) throw new ArgumentException($"Jump start must be between 1 and {Size-1}");
            if (end <= 0 || end > Size) throw new ArgumentException($"Jump end must be between 1 and {Size}");
            if (start == end) throw new ArgumentException("Jump start and end cannot be equal.");
        }

        public int? GetDestination(int tile)
        {
            if (Jumps.TryGetValue(tile, out var dest)) return dest;
            return null;
        }
    }
}
