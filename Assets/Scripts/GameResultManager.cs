using KemothStudios.Board;
using System;
using System.Collections.Generic;
using KemothStudios.Utility;
using KemothStudios.Utility.Events;
using UnityEngine;

namespace KemothStudios
{
    public class GameResultManager : MonoBehaviour
    {
        [SerializeField] private BoardDataSO _boardData;
        [SerializeField] private GameDataSO _gameData;
        
        private EventBinding<BoardReadyAfterDrawLineEvent> _checkGameResult;
        private EventBinding<TurnTimerElapsedEvent> _turnTimer;

        void Start()
        {
            _checkGameResult = new EventBinding<BoardReadyAfterDrawLineEvent>(CheckGameOver);
            EventBus<BoardReadyAfterDrawLineEvent>.RegisterBinding(_checkGameResult);

            _turnTimer = new EventBinding<TurnTimerElapsedEvent>(CheckPlayerLivesForGameOver);
            EventBus<TurnTimerElapsedEvent>.RegisterBinding(_turnTimer);
        }

        private void OnDestroy()
        {
            EventBus<BoardReadyAfterDrawLineEvent>.UnregisterBinding(_checkGameResult);
            EventBus<TurnTimerElapsedEvent>.UnregisterBinding(_turnTimer);
        }

        private void CheckGameOver()
        {
            if (_boardData.CompletedCellsCount == _boardData.TotalCellsCount)
            {
                if (_gameData.TryGetPlayer(0, out Player flagPlayer))
                {
                    foreach (Player player in _gameData.Players)
                    {
                        if (player.GetScore > flagPlayer.GetScore)
                            flagPlayer = player;
                    }
                    EventBus<PlayerWonEvent>.RaiseEvent(new PlayerWonEvent(flagPlayer));
                }
                else DebugUtility.LogError("Failed to get player on index 0 while trying to compute game result");
            }
            else
                EventBus<GameResultCheckedEvent>.RaiseEvent(new GameResultCheckedEvent());
        }
        
        private void CheckPlayerLivesForGameOver()
        {
            int playersWithNonZeroLives = 0;
            Player flag = default;
            foreach (Player player in _gameData.Players)
            {
                if (player.GetRemainingLives > 0)
                {
                    playersWithNonZeroLives++;
                    flag = player;
                }
            }
            
            if(playersWithNonZeroLives == 1) // if only one player has more than zero lives means all other players are out and this guy is the winner
                EventBus<PlayerWonEvent>.RaiseEvent(new PlayerWonEvent(flag));
            else
                EventBus<GameResultCheckedEvent>.RaiseEvent(new GameResultCheckedEvent());
        }
    }
}