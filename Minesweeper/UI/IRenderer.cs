using Minesweeper.Models;

namespace Minesweeper.UI
{
    public interface IRenderer
    {
        void Render(Board board);
        void RenderGameOver(Board board, bool won);
    }
}
