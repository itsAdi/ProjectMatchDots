using System;
using UnityEngine;

namespace KemothStudios
{
    public class TurnHandler : MonoBehaviour
    {
        [SerializeField] private GameDataSO _gameDataSO;

        public static TurnHandler Instance;

        public bool IsReady { get; private set; }
        public Action TurnUpdated;
        public int CurrentPlayerIndex { get; private set; }
        public int LastPlayerIndex { get; private set; }

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
            {
                if (Instance != this)
                    Destroy(gameObject);
            }
        }

        private void Start()
        {
            CurrentPlayerIndex = 0;
            LastPlayerIndex = -1;
            TurnUpdated?.Invoke();
            IsReady = true;

        }

        private void OnDestroy()
        {
            IsReady = false;
            TurnUpdated = null;
            CurrentPlayerIndex = 0;
            LastPlayerIndex = -1;
            if (Instance == this)
                Instance = null;
        }

        public void ChangeTurn()
        {
            LastPlayerIndex = CurrentPlayerIndex;
            CurrentPlayerIndex = (CurrentPlayerIndex + 1) % _gameDataSO.PlayerCount;
            TurnUpdated?.Invoke();
        }
    }
}