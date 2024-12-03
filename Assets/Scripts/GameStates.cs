using System;

namespace KemothStudios
{
    public sealed class GameStates
    {
        public enum States
        {
            NONE = 0,
            MAIN_MENU = 1,
            GAME = 2
        }

        public static GameStates Instance
        {
            get
            {
                if(_instance == null)
                    _instance = new GameStates();
                return _instance;
            }
        }

        private static GameStates _instance;

        public Action<States> GameStateChanged;

        private States _currentState;

        public States CurrentState
        {
            get => _currentState;
            set
            {
                if (_currentState != value)
                {
                    _currentState = value;
                    GameStateChanged?.Invoke(value);
                }
            }
        }
    }
}