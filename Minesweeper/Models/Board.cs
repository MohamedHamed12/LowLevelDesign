using System;
using System.Collections.Generic;

namespace Minesweeper.Models
{
    public sealed class Board
    {
        private readonly Cell[,] _cells;
        public int Rows { get; }
        public int Cols { get; }
        public int TotalMines { get; }
        public bool IsExploded { get; private set; }

        public Board(int rows, int cols, int totalMines)
        {
            if (rows <= 0 || cols <= 0) throw new ArgumentException("rows/cols must be positive");
            if (totalMines < 0 || totalMines >= rows * cols) throw new ArgumentException("invalid mine count");

            Rows = rows;
            Cols = cols;
            TotalMines = totalMines;
            _cells = new Cell[rows, cols];
            for (int r = 0; r < rows; r++)
                for (int c = 0; c < cols; c++)
                    _cells[r, c] = new Cell();
        }

        public Cell Get(int r, int c) => _cells[r, c];

        public void PlaceMines(IEnumerable<(int r, int c)> minePositions)
        {
            foreach (var (r, c) in minePositions)
            {
                _cells[r, c].IsMine = true;
            }
            CalculateAdjacents();
        }

        private void CalculateAdjacents()
        {
            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Cols; c++)
                {
                    if (_cells[r, c].IsMine) continue;
                    int count = 0;
                    foreach (var (nr, nc) in Neighbors(r, c))
                        if (_cells[nr, nc].IsMine) count++;
                    _cells[r, c].AdjacentMines = count;
                }
            }
        }

        private IEnumerable<(int r, int c)> Neighbors(int r, int c)
        {
            for (int dr = -1; dr <= 1; dr++)
            for (int dc = -1; dc <= 1; dc++)
            {
                if (dr == 0 && dc == 0) continue;
                int nr = r + dr, nc = c + dc;
                if (nr >= 0 && nr < Rows && nc >= 0 && nc < Cols) yield return (nr, nc);
            }
        }

        public bool ToggleFlag(int r, int c)
        {
            var cell = _cells[r, c];
            if (cell.IsRevealed) return false;
            cell.IsFlagged = !cell.IsFlagged;
            return true;
        }

        public bool Reveal(int r, int c)
        {
            var cell = _cells[r, c];
            if (cell.IsFlagged || cell.IsRevealed) return false;
            cell.IsRevealed = true;
            if (cell.IsMine)
            {
                IsExploded = true;
                return true;
            }
            if (cell.AdjacentMines == 0)
            {
                // flood-fill
                var q = new Queue<(int r, int c)>();
                q.Enqueue((r, c));
                while (q.Count > 0)
                {
                    var (cr, cc) = q.Dequeue();
                    foreach (var (nr, nc) in Neighbors(cr, cc))
                    {
                        var nCell = _cells[nr, nc];
                        if (!nCell.IsRevealed && !nCell.IsFlagged)
                        {
                            nCell.IsRevealed = true;
                            if (nCell.AdjacentMines == 0 && !nCell.IsMine)
                                q.Enqueue((nr, nc));
                        }
                    }
                }
            }
            return true;
        }

        public bool AllNonMinesRevealed()
        {
            int revealed = 0;
            for (int r = 0; r < Rows; r++)
                for (int c = 0; c < Cols; c++)
                    if (_cells[r, c].IsRevealed && !_cells[r, c].IsMine)
                        revealed++;
            return revealed == (Rows * Cols - TotalMines);
        }
    }
}
