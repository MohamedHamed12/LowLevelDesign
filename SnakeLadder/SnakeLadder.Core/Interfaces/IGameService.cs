using System.Collections.Generic;
using SnakeLadder.Core.Domain;

namespace SnakeLadder.Core.Interfaces
{
    public interface IGameService
    {
        void Start();
        IReadOnlyList<Player> Players { get; }
    }
}
