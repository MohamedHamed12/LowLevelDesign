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
