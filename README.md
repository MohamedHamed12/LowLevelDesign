# Multi-Project .NET Repository

This repository contains multiple .NET projects, each serving a different purpose. Below is a brief overview of each project.

## Projects

### 1. Tic-Tac-Toe

A classic Tic-Tac-Toe game implemented as a console application. It demonstrates a clean architecture approach in .NET, with a separation of concerns between the core game logic, application layer, and UI.

*   **To run the game:**
    ```bash
    dotnet run --project TicTacToe/TicTacToe.ConsoleUI/TicTacToe.ConsoleUI.csproj
    ```
*   **For more details:** See the [Tic-Tac-Toe README](./TicTacToe/README.md).

### 2. JsonParserApp

A custom JSON parser written in C# from scratch. This project showcases how to build a lightweight and efficient JSON parser without any external dependencies. It's a great example of how to handle complex data structures and implement a strategy pattern for parsing.

*   **To run the application:**
    ```bash
    dotnet run --project JsonParserApp/src/JsonParserApp/JsonParserApp.csproj
    ```
*   **To run the tests:**
    ```bash
    dotnet test JsonParserApp/JsonParserApp.sln
    ```
*   **For more details:** See the [JsonParserApp README](./JsonParserApp/README.md).


### 3. Snake and Ladders

A console-based implementation of the classic board game Snake and Ladders. This project demonstrates object-oriented design principles, event handling, and game loop implementation in C#.


