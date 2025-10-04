using System.Threading.Tasks;

namespace Minesweeper.Input
{
    public interface IInputHandler
    {
        Task<UserCommand?> ReadCommandAsync();
    }
}
