using System;
using System.Collections.Generic;
using System.Linq;
using SnakeLadder.Core.Domain;
using SnakeLadder.Core.Interfaces;
using SnakeLadder.Core.Exceptions;

namespace SnakeLadder.Core.Services
{
    public class GameService : IGameService
    {
        private readonly Board _board;
        private readonly Dice _dice;
        private readonly List<Player> _players;
        private int _currentPlayerIndex;
        private bool _isStarted;
        public IReadOnlyList<Player> Players => _players.AsReadOnly();

        public event Action<Player, int, int>? OnMove; // (player, from, to)
        public event Action<Player>? OnWin;

        public GameService(Board board, Dice dice, IEnumerable<Player> players)
        {
            _board = board ?? throw new ArgumentNullException(nameof(board));
            _dice = dice ?? throw new ArgumentNullException(nameof(dice));
            if (players == null) throw new ArgumentNullException(nameof(players));

            _players = players.ToList();
            if (_players.Count < 2) throw new ArgumentException("At least two players required.");
            _currentPlayerIndex = 0;
        }

        public void Start()
        {
            if (_isStarted) throw new InvalidOperationException("Game already started.");
            _isStarted = true;
            // initial positions already 0
            while (true)
            {
                var current = _players[_currentPlayerIndex];
                PlayTurn(current);
                if (!_isStarted) break; // game ended
                _currentPlayerIndex = (_currentPlayerIndex + 1) % _players.Count;
            }
        }

        private void PlayTurn(Player player)
        {
            var from = player.Position;
            var roll = _dice.Roll();

            var attempted = from + roll;
            if (attempted > _board.Size)
            {
                // invalid move — stays in place
                OnMove?.Invoke(player, from, from);
                return;
            }

            var final = attempted;
            var jump = _board.GetDestination(final);
            if (jump.HasValue) final = jump.Value;

            player.SetPosition(final);
            OnMove?.Invoke(player, from, final);

            if (final == _board.Size)
            {
                OnWin?.Invoke(player);
                _isStarted = false;
            }
        }
    }
}
