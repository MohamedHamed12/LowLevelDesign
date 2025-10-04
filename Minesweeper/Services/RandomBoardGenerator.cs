using System;
using System.Collections.Generic;
using System.Linq;

namespace Minesweeper.Services
{
    public class RandomBoardGenerator : IBoardGenerator
    {
        public IEnumerable<(int r, int c)> Generate(int rows, int cols, int mines, int? seed = null)
        {
            var rand = seed.HasValue ? new Random(seed.Value) : new Random();
            var total = rows * cols;
            var indices = Enumerable.Range(0, total).ToList();
            // Fisher-Yates shuffle partial to select first `mines` indices
            for (int i = 0; i < mines; i++)
            {
                int j = rand.Next(i, total);
                var tmp = indices[i]; indices[i] = indices[j]; indices[j] = tmp;
            }
            for (int i = 0; i < mines; i++)
            {
                int idx = indices[i];
                yield return (idx / cols, idx % cols);
            }
        }
    }
}
