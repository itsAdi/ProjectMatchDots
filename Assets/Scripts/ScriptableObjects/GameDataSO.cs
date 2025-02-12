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

        public bool TryGetPlayerLives(int playerIndex, out int lives)
        {
            if (playerIndex >= 0 && playerIndex < _playerCount)
            {
                lives = _players[playerIndex].RemainingLivesLocal;
                return true;
            }
            lives = -1;
            return false;
        }

        public bool TrySetPlayerLives(int playerIndex, int lives)
        {
            if (playerIndex >= 0 && playerIndex < _playerCount)
            {
                Player p = _players[playerIndex];
                p.RemainingLivesLocal = lives;
                _players[playerIndex] = p;
                return true;
            }
            return false;
        }
        
        public bool TryGetPlayerScore(int playerIndex, out int score)
        {
            if (playerIndex >= 0 && playerIndex < _playerCount)
            {
                score = _players[playerIndex].ScoreLocal;
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
                p.ScoreLocal = score;
                _players[playerIndex] = p;
                return true;
            }
            return false;
        }
    }

    public struct Player
    {
        public string Name { get; }
        public int AvatarIndex { get; }
        public int PlayerIndex { get; }
        
        private GameDataSO _gameDataSO;
        private int _score;
        private int _remainingLives;

        public Player(string name, int avatarIndex, int playerIndex, GameDataSO gameData)
        {
            Name = name;
            AvatarIndex = avatarIndex;
            PlayerIndex = playerIndex;
            _gameDataSO = gameData;
            _score = 0;
            _remainingLives = 0;
        }
        
        public int GetRemainingLives {
            get
            {
                if (!_gameDataSO.TryGetPlayerLives(PlayerIndex, out var lives))
                    DebugUtility.LogError($"Failed to get remaining lives for player on index {PlayerIndex}");
                return lives;
            }
        }
        
        public int GetScore {
            get
            {
                if (!_gameDataSO.TryGetPlayerScore(PlayerIndex, out var score))
                    DebugUtility.LogError($"Failed to get score for player on index {PlayerIndex}");
                return score;
            }
        }

        /// <summary>
        /// <para>Used by <b>GameDataSO</b> to get and set score</para>
        /// <para>To get score either use <see cref="GetScore"/> or <see cref="GameDataSO.TryGetPlayerScore"/> and to set score only use <see cref="GameDataSO.TrySetPlayerScore"/></para>
        /// </summary>
        public int ScoreLocal
        {
            get => _score;
            set => _score = value;
        }
        
        /// <summary>
        /// <para>Used by <b>GameDataSO</b> to get and set remaining lives</para>
        /// <para>To get remaining lives either use <see cref="GetRemainingLives"/> or <see cref="GameDataSO.TryGetPlayerLives"/> and to set remaining lives only use <see cref="GameDataSO.TrySetPlayerLives"/></para>
        /// </summary>
        public int RemainingLivesLocal
        {
            get => _remainingLives;
            set => _remainingLives = value;
        }
    }
}