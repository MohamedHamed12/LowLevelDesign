using System;
using System.Threading.Tasks;
using Minesweeper.Models;
using Minesweeper.Services;
using Minesweeper.UI;
using Minesweeper.Input;

namespace Minesweeper.Game
{
    public class GameEngine
    {
        private readonly IRenderer _renderer;
        private readonly IInputHandler _input;
        private readonly IBoardGenerator _boardGenerator;

        public GameEngine(IRenderer renderer, IInputHandler input, IBoardGenerator boardGenerator)
        {
            _renderer = renderer;
            _input = input;
            _boardGenerator = boardGenerator;
        }

        public async Task RunAsync()
        {
            Console.WriteLine("Welcome to Minesweeper (.NET 9)");
            Console.WriteLine("Enter rows cols mines [seed]");
            Console.Write("> ");
            var line = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(line)) return;
            var parts = line.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 3) { Console.WriteLine("Expected: rows cols mines [seed]"); return; }

            if (!int.TryParse(parts[0], out var rows) ||
                !int.TryParse(parts[1], out var cols) ||
                !int.TryParse(parts[2], out var mines))
            {
                Console.WriteLine("Invalid numbers"); return;
            }
            int? seed = null;
            if (parts.Length >= 4 && int.TryParse(parts[3], out var s)) seed = s;

            var board = new Board(rows, cols, mines);
            var minePositions = _boardGenerator.Generate(rows, cols, mines, seed);
            board.PlaceMines(minePositions);

            _renderer.Render(board);

            bool running = true;
            while (running)
            {
                var cmd = await _input.ReadCommandAsync();
                if (cmd is null) continue;

                switch (cmd.Type)
                {
                    case CommandType.Exit:
                        Console.WriteLine("Exiting..."); running = false; break;
                    case CommandType.Help:
                        PrintHelp(); break;
                    case CommandType.Reveal:
                        HandleReveal(board, cmd.Row, cmd.Col); break;
                    case CommandType.Flag:
                        HandleFlag(board, cmd.Row, cmd.Col); break;
                    case CommandType.Invalid:
                        break;
                }

                if (board.IsExploded)
                {
                    _renderer.RenderGameOver(board, won: false);
                    running = false;
                }
                else if (board.AllNonMinesRevealed())
                {
                    _renderer.RenderGameOver(board, won: true);
                    running = false;
                }
                else
                {
                    _renderer.Render(board);
                }
            }
        }

        private void PrintHelp()
        {
            Console.WriteLine("Commands:");
            Console.WriteLine("  r row col  - reveal cell (1-based)");
            Console.WriteLine("  f row col  - flag/unflag cell (1-based)");
            Console.WriteLine("  help       - show this help");
            Console.WriteLine("  exit       - quit");
        }

        private void HandleReveal(Board board, int userRow, int userCol)
        {
            if (!ValidateCoords(board, userRow, userCol)) return;
            int r = userRow - 1, c = userCol - 1;
            var cell = board.Get(r, c);
            if (cell.IsFlagged) { Console.WriteLine("Cell is flagged. Unflag to reveal."); return; }
            board.Reveal(r, c);
        }

        private void HandleFlag(Board board, int userRow, int userCol)
        {
            if (!ValidateCoords(board, userRow, userCol)) return;
            int r = userRow - 1, c = userCol - 1;
            board.ToggleFlag(r, c);
        }

        private bool ValidateCoords(Board board, int userRow, int userCol)
        {
            if (userRow < 1 || userRow > board.Rows || userCol < 1 || userCol > board.Cols)
            {
                Console.WriteLine("Coordinates out of range.");
                return false;
            }
            return true;
        }
    }
}
