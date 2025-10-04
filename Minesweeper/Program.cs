using Minesweeper.Game;
using Minesweeper.Services;
using Minesweeper.UI;
using Minesweeper.Input;

var renderer = new ConsoleRenderer();
var inputHandler = new ConsoleInputHandler();
var boardGen = new RandomBoardGenerator();
var engine = new GameEngine(renderer, inputHandler, boardGen);

await engine.RunAsync();
