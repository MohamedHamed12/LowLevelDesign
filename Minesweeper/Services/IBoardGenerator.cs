using System.Collections.Generic;
using Minesweeper.Models;

namespace Minesweeper.Services
{
    public interface IBoardGenerator
    {
        IEnumerable<(int r, int c)> Generate(int rows, int cols, int mines, int? seed = null);
    }
}
