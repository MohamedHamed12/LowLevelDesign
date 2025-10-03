using TicTacToe.Core.Entities;
using TicTacToe.Application.Players;
using TicTacToe.Application.Game;

var player1 = new HumanPlayer("Player 1", Mark.X);
var player2 = new AIPlayer("Computer", Mark.O);

var game = new GameController(player1, player2);
game.Start();
