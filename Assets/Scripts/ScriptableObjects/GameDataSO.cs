using System;
using UnityEngine;

namespace KemothStudios
{
    public class GameDataSO : ScriptableObject
    {
        [NonSerialized] private Player[] _players;

        public void InitializePlayerData(params Player[] players)
        {
            _players = players;
        }
    }

    public struct Player
    {
        public string Name { get ; private set; }
        public int Score { get; private set; }
        public int AvatarIndex { get; private set; }

        public Player(string name, int avatarIndex)
        {
            Name = name;
            Score = 0;
            AvatarIndex = avatarIndex;
        }
    }
}