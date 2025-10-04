namespace Minesweeper.Input
{
    public record UserCommand(CommandType Type, int Row, int Col);

    public enum CommandType
    {
        Reveal,
        Flag,
        Exit,
        Help,
        Invalid
    }
}
