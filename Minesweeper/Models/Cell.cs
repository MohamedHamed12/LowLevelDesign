using System;

namespace Minesweeper.Models
{
    public sealed class Cell
    {
        public bool IsMine { get; internal set; }
        public bool IsRevealed { get; internal set; }
        public bool IsFlagged { get; internal set; }
        public int AdjacentMines { get; internal set; }

        public char DisplayChar()
        {
            if (!IsRevealed)
                return IsFlagged ? '⚑' : '■';
            if (IsMine) return '*';
            return AdjacentMines == 0 ? ' ' : AdjacentMines.ToString()[0];
        }
    }
}
