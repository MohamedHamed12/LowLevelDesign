using TicTacToe.Core.Entities;
using TicTacToe.Core.Rules;
using TicTacToe.Application.Players;
using System;

namespace TicTacToe.Application.Game;

public class GameController
{
    private readonly Board _board;
    private readonly RulesEngine _rules;
    private readonly IPlayer _playerX;
    private readonly IPlayer _playerO;
    private IPlayer _currentPlayer;

    public GameController(IPlayer playerX, IPlayer playerO, int boardSize = 3)
    {
        _board = new Board(boardSize);
        _rules = new RulesEngine();
        _playerX = playerX;
        _playerO = playerO;
        _currentPlayer = _playerX;
    }

    public void Start()
    {
        Console.WriteLine("🎮 Tic Tac Toe Game Started!");
        PrintBoard();

        while (true)
        {
            var move = _currentPlayer.GetMove(_board);

            if (!_board.ApplyMove(move))
            {
                Console.WriteLine("❌ Invalid move, cell already taken. Try again.");
                continue;
            }

            PrintBoard();

            var (win, winner) = _rules.CheckWin(_board, move);
            if (win)
            {
                Console.WriteLine($"🏆 { _currentPlayer.Name } ({winner}) wins!");
                break;
            }

            if (_rules.IsDraw(_board))
            {
                Console.WriteLine("🤝 It's a draw!");
                break;
            }

            _currentPlayer = _currentPlayer == _playerX ? _playerO : _playerX;
        }
    }

    private void PrintBoard()
    {
        Console.WriteLine();
        for (int r = 0; r < _board.Size; r++)
        {
            for (int c = 0; c < _board.Size; c++)
            {
                var mark = _board.Get(r, c) switch
                {
                    Mark.X => "X",
                    Mark.O => "O",
                    _ => "."
                };
                Console.Write(mark + " ");
            }
            Console.WriteLine();
        }
        Console.WriteLine();
    }
}
