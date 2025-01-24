using KemothStudios.Board;
using System.Collections.Generic;
using KemothStudios.Utility;
using KemothStudios.Utility.Events;
using UnityEngine;

namespace KemothStudios
{
    public class ScoreManager : MonoBehaviour
    {
        [SerializeField] private GameDataSO _gameData;

        private EventBinding<CellsAcquireStartedEvent> _cellAcquireStarted;
        private EventBinding<TurnStartedEvent> _turnStarted;
        private Player _currentPlayer;

        private void Start()
        {
            _cellAcquireStarted = new EventBinding<CellsAcquireStartedEvent>(UpdateScore);
            EventBus<CellsAcquireStartedEvent>.RegisterBinding(_cellAcquireStarted);
            _turnStarted = new EventBinding<TurnStartedEvent>(SetCurrentPlayer);
            EventBus<TurnStartedEvent>.RegisterBinding(_turnStarted);
        }

        private void OnDestroy()
        {
            EventBus<CellsAcquireStartedEvent>.UnregisterBinding(_cellAcquireStarted);
            EventBus<TurnStartedEvent>.UnregisterBinding(_turnStarted);
        }

        private void SetCurrentPlayer(TurnStartedEvent turnData) => _currentPlayer = turnData.Player;

        private void UpdateScore(CellsAcquireStartedEvent lineData)
        {
            using IEnumerator<Cell> dataEnum = lineData.Cells.GetEnumerator();
            int score = _currentPlayer.GetScore;
            while (dataEnum.MoveNext()) score++;
            if(!_gameData.TrySetPlayerScore(_currentPlayer.PlayerIndex, score))
                DebugUtility.LogError($"Failed to set player score for player on index {_currentPlayer.PlayerIndex}");
            EventBus<ScoreUpdatedEvent>.RaiseEvent(new ScoreUpdatedEvent());
        }
    }
}