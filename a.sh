#!/usr/bin/env bash
set -euo pipefail

PROJECT_NAME="Minesweeper"
FRAMEWORK="net9.0"

# Create project
dotnet new console -n ${PROJECT_NAME} -f ${FRAMEWORK}
cd ${PROJECT_NAME}

# Create folders
mkdir -p Models Services Game UI Input Exceptions

# Overwrite Program.cs and add files
cat > Program.cs <<'CS'
using Minesweeper.Game;
using Minesweeper.Services;
using Minesweeper.UI;
using Minesweeper.Input;

var renderer = new ConsoleRenderer();
var inputHandler = new ConsoleInputHandler();
var boardGen = new RandomBoardGenerator();
var engine = new GameEngine(renderer, inputHandler, boardGen);

await engine.RunAsync();
CS

cat > Models/Cell.cs <<'CS'
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
CS

cat > Models/Board.cs <<'CS'
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
CS

cat > Services/IBoardGenerator.cs <<'CS'
using System.Collections.Generic;
using Minesweeper.Models;

namespace Minesweeper.Services
{
    public interface IBoardGenerator
    {
        IEnumerable<(int r, int c)> Generate(int rows, int cols, int mines, int? seed = null);
    }
}
CS

cat > Services/RandomBoardGenerator.cs <<'CS'
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
CS

cat > UI/IRenderer.cs <<'CS'
using Minesweeper.Models;

namespace Minesweeper.UI
{
    public interface IRenderer
    {
        void Render(Board board);
        void RenderGameOver(Board board, bool won);
    }
}
CS

cat > UI/ConsoleRenderer.cs <<'CS'
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
CS

cat > Input/IInputHandler.cs <<'CS'
using System.Threading.Tasks;

namespace Minesweeper.Input
{
    public interface IInputHandler
    {
        Task<UserCommand?> ReadCommandAsync();
    }
}
CS

cat > Input/UserCommand.cs <<'CS'
namespace Minesweeper.Input
{
    public record UserCommand(CommandType Type, int Row, int Col);

    public enum CommandType
    {
        Reveal,
        Flag,
        Exit,
        Help,
        Invalid
    }
}
CS

cat > Input/ConsoleInputHandler.cs <<'CS'
using System;
using System.Globalization;
using System.Threading.Tasks;

namespace Minesweeper.Input
{
    public class ConsoleInputHandler : IInputHandler
    {
        public async Task<UserCommand?> ReadCommandAsync()
        {
            Console.Write("> ");
            var line = await Task.Run(() => Console.ReadLine());
            if (line == null) return null;
            var parts = line.Trim().Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0) return null;
            var cmd = parts[0].ToLowerInvariant();
            if (cmd == "exit" || cmd == "e") return new UserCommand(CommandType.Exit, 0, 0);
            if (cmd == "help" || cmd == "h") return new UserCommand(CommandType.Help, 0, 0);
            if ((cmd == "r" || cmd == "reveal") && parts.Length >= 3 && TryParseCoord(parts[1], parts[2], out var r, out var c))
                return new UserCommand(CommandType.Reveal, r, c);
            if ((cmd == "f" || cmd == "flag") && parts.Length >= 3 && TryParseCoord(parts[1], parts[2], out r, out c))
                return new UserCommand(CommandType.Flag, r, c);
            Console.WriteLine("Invalid command. Type 'help' for usage.");
            return new UserCommand(CommandType.Invalid, 0, 0);
        }

        private bool TryParseCoord(string s1, string s2, out int r, out int c)
        {
            r = c = 0;
            if (!int.TryParse(s1, NumberStyles.Integer, CultureInfo.InvariantCulture, out var rr)) return false;
            if (!int.TryParse(s2, NumberStyles.Integer, CultureInfo.InvariantCulture, out var cc)) return false;
            r = rr; c = cc;
            return true;
        }
    }
}
CS

cat > Game/GameEngine.cs <<'CS'
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
CS

cat > Exceptions/InvalidMoveException.cs <<'CS'
using System;

namespace Minesweeper.Exceptions
{
    public class InvalidMoveException : Exception
    {
        public InvalidMoveException(string message) : base(message) { }
    }
}
CS

cat > README.md <<'MD'
# Minesweeper (.NET 9)

Console Minesweeper implementation.

Usage:
1. Build and run:
   dotnet run --project Minesweeper.csproj
2. Enter initial parameters: rows cols mines [seed]
   Example: `9 9 10` or `9 9 10 42`
3. Commands while playing:
   - `r 1 1` or `reveal 1 1` to reveal cell (row 1 col 1)
   - `f 1 1` or `flag 1 1` to toggle flag
   - `help` to show commands
   - `exit` to quit

MD

cat > .gitignore <<'GIT'
bin/
obj/
.vs/
*.user
GIT

echo "Scaffold complete. You can run: dotnet run --project ${PROJECT_NAME}.csproj"
