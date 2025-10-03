using TicTacToe.Core.Entities;

namespace TicTacToe.Application.Players;

public interface IPlayer
{
    string Name { get; }
    Mark Mark { get; }
    Move GetMove(Board board);
}
