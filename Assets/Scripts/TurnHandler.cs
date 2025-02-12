using KemothStudios.Utility;
using KemothStudios.Utility.Events;
using UnityEngine;

namespace KemothStudios
{
    public class TurnHandler : MonoBehaviour
    {
        [SerializeField] private GameDataSO _gameDataSO;
        
        public Player CurrentPlayer { get; private set; }
        public Player LastPlayer { get; private set; }

        private int _currentPlayerIndex;
        private EventBinding<CellAcquireCompletedEvent> _cellAcquireCompleted;
        private EventBinding<GameStartedEvent> _gameStarted;
        private EventBinding<GameResultCheckedEvent> _gameResultChecked;
        private EventBinding<PlayerWonEvent> _playerWon;
        private bool _canChangeTurn;

        private void Start()
        {
            LastPlayer = default;
            _canChangeTurn = true;
            
            _cellAcquireCompleted = new EventBinding<CellAcquireCompletedEvent>(DisableTurnChange);
            EventBus<CellAcquireCompletedEvent>.RegisterBinding(_cellAcquireCompleted);
            
            _gameResultChecked = new EventBinding<GameResultCheckedEvent>(ChangeTurn);
            EventBus<GameResultCheckedEvent>.RegisterBinding(_gameResultChecked);
            
            _gameStarted = new EventBinding<GameStartedEvent>(StartGame);
            EventBus<GameStartedEvent>.RegisterBinding(_gameStarted);
            
            _playerWon = new EventBinding<PlayerWonEvent>(EndCurrentPlayerTurnOnGameOver);
            EventBus<PlayerWonEvent>.RegisterBinding(_playerWon);
        }


        private void OnDestroy()
        {
            CurrentPlayer = default;
            LastPlayer = default;
            _currentPlayerIndex = 0;
            EventBus<CellAcquireCompletedEvent>.UnregisterBinding(_cellAcquireCompleted);
            EventBus<GameResultCheckedEvent>.UnregisterBinding(_gameResultChecked);
            EventBus<GameStartedEvent>.UnregisterBinding(_gameStarted);
            EventBus<PlayerWonEvent>.UnregisterBinding(_playerWon);
        }

        private void EndCurrentPlayerTurnOnGameOver() => EventBus<TurnEndedEvent>.RaiseEvent(new TurnEndedEvent(CurrentPlayer));
        private void StartGame() => RaiseTurnStartEvent(0);

        private void DisableTurnChange()
        {
            _canChangeTurn = false;
        }

        private void ChangeTurn()
        {
            if (_canChangeTurn)
            {
                LastPlayer = CurrentPlayer;
                if (!LastPlayer.Equals(default(Player)))
                {
                    EventBus<TurnEndedEvent>.RaiseEvent(new TurnEndedEvent(LastPlayer));
                    RaiseTurnStartEvent((_currentPlayerIndex + 1) % _gameDataSO.PlayerCount);
                }
                else DebugUtility.LogError("No valid player found to change turn");
            }
            _canChangeTurn = true;
        }

        private void RaiseTurnStartEvent(int playerIndex)
        {
            if (_gameDataSO.TryGetPlayer(playerIndex, out var player))
            {
                CurrentPlayer = player;
                _currentPlayerIndex = playerIndex;
                EventBus<TurnStartedEvent>.RaiseEvent(new TurnStartedEvent(player));
            }
            else DebugUtility.LogError($"Could not find player on index {playerIndex}, turn could not be started");
        }
    }
}