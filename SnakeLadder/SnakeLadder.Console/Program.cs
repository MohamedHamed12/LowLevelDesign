using System;
using System.Collections.Generic;
using SnakeLadder.Core.Domain;
using SnakeLadder.Core.Services;

namespace SnakeLadder.ConsoleApp
{
    internal static class Program
    {
        static void Main()
        {
            Console.WriteLine("Snake & Ladder - .NET 9");
            // Default board: size 100, simple example snakes & ladders
            var snakes = new (int start, int end)[]
            {
                (99, 78), (95, 75), (92, 88), (74, 53), (64, 60), (62, 19), (49, 11), (46, 25), (16, 6)
            };
            var ladders = new (int start, int end)[]
            {
                (2, 38), (7, 14), (8, 31), (15, 26), (21, 42), (28, 84), (36, 44), (51, 67), (71, 91), (78, 98)
            };

            var board = new Board(size: 100, snakes: snakes, ladders: ladders);
            var dice = new Dice(6);

            var players = new List<Player>
            {
                new Player("Alice"),
                new Player("Bob")
            };

            var game = new GameService(board, dice, players);
            game.OnMove += (player, from, to) =>
            {
                var verb = from == to ? "stays" : "moves";
                Console.WriteLine($"{player.Name}: {verb} from {from} to {to}");
            };
            game.OnWin += (winner) =>
            {
                Console.WriteLine($"🎉 {winner.Name} wins the game! 🎉");
            };

            Console.WriteLine("Press Enter to start. Press Ctrl+C to exit.");
            Console.ReadLine();
            game.Start();
        }
    }
}
