using System;
using UnityEngine;

namespace KemothStudios
{
    public class GameDataSO : ScriptableObject
    {
        [NonSerialized] private Player[] _players;
        [NonSerialized] private int _playerCount;

        public int PlayerCount => _playerCount;

        public void InitializePlayerData(params Player[] players)
        {
            _players = players;
            _playerCount = players.Length;
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
                score = _players[playerIndex].Score;
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
        public int Score { get; private set; }
        public int AvatarIndex { get; private set; }

        public Player(string name, int avatarIndex)
        {
            Name = name;
            Score = 0;
            AvatarIndex = avatarIndex;
        }

        public void SetScore(int score) => Score = score;
    }
}