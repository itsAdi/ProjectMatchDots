using KemothStudios.Board;
using System;
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

        void Start()
        {
            _checkGameResult = new EventBinding<BoardReadyAfterDrawLineEvent>(CheckGameOver);
            EventBus<BoardReadyAfterDrawLineEvent>.RegisterBinding(_checkGameResult);
        }

        private void OnDestroy()
        {
            EventBus<BoardReadyAfterDrawLineEvent>.UnregisterBinding(_checkGameResult);
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
        }
    }
}