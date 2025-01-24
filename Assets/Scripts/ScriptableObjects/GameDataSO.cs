using System;
using KemothStudios.Utility;
using UnityEngine;

namespace KemothStudios
{
    public class GameDataSO : ScriptableObject
    {
        [NonSerialized] private Player[] _players;
        [NonSerialized] private int _playerCount;

        public int PlayerCount => _playerCount;
        public System.Collections.Generic.IEnumerable<Player> Players => _players;

        public void InitializePlayerData(params Player[] players)
        {
            _players = players;
            _playerCount = players.Length;
        }

        public bool TryGetPlayer(int playerIndex, out Player player)
        {
            if (playerIndex >= 0 && playerIndex < _playerCount)
            {
                player = _players[playerIndex];
                return true;
            }
            player = default;
            return false;
        }

        public bool TryGetPlayerAvatarIndex(int playerIndex, out int avatarIndex)
        {
            if (playerIndex >= 0 && playerIndex < _playerCount)
            {
                avatarIndex = _players[playerIndex].AvatarIndex;
                return true;
            }
            avatarIndex = -1;
            return false;
        }

        public bool TryGetPlayerName(int playerIndex, out string name)
        {
            if (playerIndex >= 0 && playerIndex < _playerCount)
            {
                name = _players[playerIndex].Name;
                return true;
            }
            name = string.Empty;
            return false;
        }

        public bool TryGetPlayerScore(int playerIndex, out int score)
        {
            if (playerIndex >= 0 && playerIndex < _playerCount)
            {
                score = _players[playerIndex].GetScoreLocal;
                return true;
            }
            score = -1;
            return false;
        }

        public bool TrySetPlayerScore(int playerIndex, int score)
        {
            if (playerIndex >= 0 && playerIndex < _playerCount)
            {
                Player p = _players[playerIndex];
                p.SetScore(score);
                _players[playerIndex] = p;
                return true;
            }
            return false;
        }
    }

    public struct Player
    {
        public string Name { get; private set; }
        public int AvatarIndex { get; private set; }
        public int PlayerIndex { get; private set; }
        
        private GameDataSO _gameDataSO;
        private int _score;

        public Player(string name, int avatarIndex, int playerIndex, GameDataSO gameData)
        {
            Name = name;
            AvatarIndex = avatarIndex;
            PlayerIndex = playerIndex;
            _gameDataSO = gameData;
            _score = 0;
        }
        
        public void SetScore(int score) => _score = score;
        public int GetScore {
            get
            {
                if (!_gameDataSO.TryGetPlayerScore(PlayerIndex, out var score))
                    DebugUtility.LogError($"Failed to get score for player on index {PlayerIndex}");
                return score;
            }
        }
        
        /// <summary>
        /// Should only be used from GameDataSO
        /// </summary>
        public int GetScoreLocal => _score;
    }
}