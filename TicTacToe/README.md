# Tic-Tac-Toe Game

This is a classic Tic-Tac-Toe game implemented in C# as a console application. It follows a clean architecture pattern, separating the core game logic from the user interface.

## Features

*   **Two-player mode:** Play against another human player.
*   **AI opponent:** Play against a simple AI.
*   **Clean architecture:** The project is structured with a clear separation of concerns, making it easy to maintain and extend.
*   **Extensible design:** The game is designed to be easily extensible with new features.

## Project Structure

The project is divided into the following layers:

*   **TicTacToe.ConsoleUI:** The presentation layer, responsible for rendering the game in the console and handling user input.
*   **TicTacToe.Application:** The application layer, which contains the main game logic and orchestrates the flow of the game.
*   **TicTacToe.Core:** The core layer, containing the business entities and rules of the game.
*   **TicTacToe.Infrastructure:** The infrastructure layer, which can be used to implement data persistence or other external services.
*   **TicTacToe.Tests:** Unit tests for the application.

## Getting Started

To play the game, you'll need the .NET SDK installed on your machine.

1.  Clone the repository:
    ```bash
    git clone <repository-url>
    ```
2.  Navigate to the project directory:
    ```bash
    cd TicTacToe
    ```
3.  Run the game:
    ```bash
    dotnet run --project TicTacToe.ConsoleUI/TicTacToe.ConsoleUI.csproj
    ```

## How to Play

The game is played on a 3x3 grid. One player is 'X' and the other is 'O'. Players take turns putting their marks in empty squares. The first player to get 3 of their marks in a row (up, down, across, or diagonally) is the winner. When all 9 squares are full, the game is over. If no player has 3 marks in a row, the game is a tie.
