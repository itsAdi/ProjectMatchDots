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
            if (_completedCellCount == _boardData.TotalCellsCount) PlayerWon?.Invoke(TurnHandler.Instance.CurrentPlayerIndex);
        }
    } 
}
