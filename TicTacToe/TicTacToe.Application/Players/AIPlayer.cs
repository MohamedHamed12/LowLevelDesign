using TicTacToe.Core.Entities;
using System;
using System.Linq;

namespace TicTacToe.Application.Players;

public class AIPlayer : IPlayer
{
    public string Name { get; }
    public Mark Mark { get; }

    private readonly Random _random = new();

    public AIPlayer(string name, Mark mark)
    {
        Name = name;
        Mark = mark;
    }

    public Move GetMove(Board board)
    {
        // For now: pick random available move
        var moves = board.GetAvailableMoves(Mark).ToList();
        return moves[_random.Next(moves.Count)];
    }
}
