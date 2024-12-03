using KemothStudios.Board;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KemothStudios
{
    public class GameResultManager : MonoBehaviour
    {
        [SerializeField] private BoardDataSO _boardData;
        [SerializeField] private GameDataSO _gameData;

        public static GameResultManager Instance { get; private set; }

        // Passed argument is the index of the winning player index
        public Action<int> PlayerWon;

        private int _completedCellCount;

        private void Awake()
        {
            Instance = this;
        }

        void Start()
        {
            GameManager.Instance.OnCellCompleted += CellCompleted;
        }

        private void OnDestroy()
        {
            Instance = null;
            GameManager.Instance.OnCellCompleted -= CellCompleted;
        }

        private void CellCompleted(IEnumerable<Cell> enumerable)
        {
            IEnumerator enumerator = enumerable.GetEnumerator();
            while(enumerator.MoveNext()) _completedCellCount++;
            if (_completedCellCount == _boardData.TotalCellsCount)
            {
                int flagIndex = 0;
                int flagScore = 0;
                int currentIndex = 0;
                foreach (Player player in _gameData.Players)
                {
                    if(player.Score > flagScore)
                    {
                        flagScore = player.Score;
                        flagIndex = currentIndex;
                    }
                    currentIndex++;
                }
                PlayerWon?.Invoke(flagIndex);
            }
        }
    } 
}
