using System.Collections.Generic;

namespace TicTacToe.Core.Entities;

public class Board
{
    private readonly Mark[,] _grid;
    public int Size { get; }

    public Board(int size = 3)
    {
        Size = size;
        _grid = new Mark[size, size];
    }

    public Mark Get(int row, int col) => _grid[row, col];

    public bool ApplyMove(Move move)
    {
        if (_grid[move.Row, move.Col] != Mark.Empty) return false;
        _grid[move.Row, move.Col] = move.Mark;
        return true;
    }

    public void UndoMove(int row, int col) => _grid[row, col] = Mark.Empty;

    public IEnumerable<Move> GetAvailableMoves(Mark mark)
    {
        for (int r = 0; r < Size; r++)
        {
            for (int c = 0; c < Size; c++)
            {
                if (_grid[r, c] == Mark.Empty)
                    yield return new Move(r, c, mark);
            }
        }
    }

    public bool IsFull()
    {
        for (int r = 0; r < Size; r++)
        {
            for (int c = 0; c < Size; c++)
            {
                if (_grid[r, c] == Mark.Empty)
                    return false;
            }
        }
        return true;
    }
}
