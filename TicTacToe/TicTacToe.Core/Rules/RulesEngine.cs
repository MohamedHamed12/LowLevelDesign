using TicTacToe.Core.Entities;

namespace TicTacToe.Core.Rules;

public class RulesEngine
{
    public (bool Win, Mark Winner) CheckWin(Board board, Move lastMove)
    {
        var mark = lastMove.Mark;
        int r = lastMove.Row, c = lastMove.Col, n = board.Size;

        bool rowWin = true, colWin = true, diagWin = true, antiDiagWin = true;

        for (int i = 0; i < n; i++)
        {
            if (board.Get(r, i) != mark) rowWin = false;
            if (board.Get(i, c) != mark) colWin = false;
            if (board.Get(i, i) != mark) diagWin = false;
            if (board.Get(i, n - 1 - i) != mark) antiDiagWin = false;
        }

        if (rowWin || colWin || diagWin || antiDiagWin)
            return (true, mark);

        return (false, Mark.Empty);
    }

    public bool IsDraw(Board board)
    {
        return board.IsFull();
    }
}
