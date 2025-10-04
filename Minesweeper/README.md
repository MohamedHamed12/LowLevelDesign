# Minesweeper (.NET 9)

Console Minesweeper implementation.

Usage:
1. Build and run:
   dotnet run --project Minesweeper.csproj
2. Enter initial parameters: rows cols mines [seed]
   Example: `9 9 10` or `9 9 10 42`
3. Commands while playing:
   - `r 1 1` or `reveal 1 1` to reveal cell (row 1 col 1)
   - `f 1 1` or `flag 1 1` to toggle flag
   - `help` to show commands
   - `exit` to quit

