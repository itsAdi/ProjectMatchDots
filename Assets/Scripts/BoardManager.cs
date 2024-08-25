using System.Collections.Generic;
using UnityEngine;

namespace KemothStudios.Board
{
    /// <summary>
    /// Class to listen to line click even and iterate over shared cells and check if the are completed, invokes event with the newly completed cells
    /// </summary>
    public class BoardManager : MonoBehaviour
    {
        void Start()
        {
            GameManager.Instance.OnLineClicked += CheckForCompletedCells;
        }

        private void OnDestroy()
        {
            if (GameManager.Instance != null)
                GameManager.Instance.OnLineClicked -= CheckForCompletedCells;
        }

        private void CheckForCompletedCells(Line line)
        {
            List<Cell> cells = new List<Cell>();
            bool cellFound = false;
            foreach (Cell cell in line.SharedCells)
            {
                if (cell.IsCellCompleted)
                {
                    cells.Add(cell);
                    cellFound = true;
                }
            }

            if (cellFound)
            {
                GameManager.Instance.OnCellCompleted?.Invoke(cells);
            }
            else
            {
                TurnHandler.Instance.ChangeTurn();
            }
        }
    }
}