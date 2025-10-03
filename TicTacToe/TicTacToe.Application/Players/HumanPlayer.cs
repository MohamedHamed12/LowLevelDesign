using TicTacToe.Core.Entities;
using System;

namespace TicTacToe.Application.Players;

public class HumanPlayer : IPlayer
{
    public string Name { get; }
    public Mark Mark { get; }

    public HumanPlayer(string name, Mark mark)
    {
        Name = name;
        Mark = mark;
    }

    public Move GetMove(Board board)
    {
        while (true)
        {
            Console.Write($"{Name} ({Mark}), enter your move as row,col: ");
            var input = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input)) continue;

            var parts = input.Split(',');
            if (parts.Length != 2) continue;

            if (int.TryParse(parts[0], out int row) && int.TryParse(parts[1], out int col))
            {
                if (row >= 0 && row < board.Size && col >= 0 && col < board.Size)
                {
                    return new Move(row, col, Mark);
                }
            }

            Console.WriteLine("Invalid move. Try again.");
        }
    }
}
