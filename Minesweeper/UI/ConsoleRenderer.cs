using System;
using Minesweeper.Models;
using System.Text;

namespace Minesweeper.UI
{
    public class ConsoleRenderer : IRenderer
    {
        public void Render(Board board)
        {
            Console.Clear();
            var sb = new StringBuilder();
            sb.AppendLine("Minesweeper");
            sb.AppendLine($"Size: {board.Rows}x{board.Cols} | Mines: {board.TotalMines}");
            sb.Append("   ");
            for (int c = 0; c < board.Cols; c++) sb.Append((c + 1).ToString().PadLeft(3));
            sb.AppendLine();
            sb.Append("   ");
            for (int c = 0; c < board.Cols; c++) sb.Append("---");
            sb.AppendLine();

            for (int r = 0; r < board.Rows; r++)
            {
                sb.Append((r + 1).ToString().PadLeft(3));
                for (int c = 0; c < board.Cols; c++)
                {
                    var ch = board.Get(r, c).DisplayChar();
                    sb.Append(' ');
                    sb.Append(ch);
                    sb.Append(' ');
                }
                sb.AppendLine();
            }
            Console.WriteLine(sb.ToString());
            Console.WriteLine("Commands: r row col (reveal), f row col (flag), help, exit");
        }

        public void RenderGameOver(Board board, bool won)
        {
            // reveal all mines for final display
            for (int r = 0; r < board.Rows; r++)
                for (int c = 0; c < board.Cols; c++)
                    if (board.Get(r, c).IsMine) board.Get(r, c).IsRevealed = true;

            Render(board);
            Console.WriteLine(won ? "Congratulations — you won!" : "Boom! You hit a mine. Game over.");
        }
    }
}
