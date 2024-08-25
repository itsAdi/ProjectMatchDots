using KemothStudios.Board;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace KemothStudios
{
    public class ScoreManager : MonoBehaviour
    {
        [SerializeField] private GameDataSO _gameData;

        public static ScoreManager Instance;

        public Action ScoreUpdated;

        private void Awake()
        {
            if(Instance == null)
                Instance = this;
            else if (Instance != this)
                Destroy(gameObject);
        }

        private void Start()
        {
            GameManager.Instance.OnCellCompleted += UpdateScore;
        }

        private void OnDestroy()
        {
            ScoreUpdated = null;
            if (GameManager.Instance != null)
                GameManager.Instance.OnCellCompleted -= UpdateScore;
            if(Instance == this)
                Instance = null;
        }

        private void UpdateScore(IEnumerable<Cell> enumerable)
        {
            if (_gameData.TryGetPlayerScore(TurnHandler.Instance.CurrentPlayerIndex, out int score))
            {
                foreach (Cell cell in enumerable) score++;
                _gameData.TrySetPlayerScore(TurnHandler.Instance.CurrentPlayerIndex, score);
                ScoreUpdated?.Invoke();
            }
        }
    }
}