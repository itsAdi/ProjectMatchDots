using System.Collections;
using System.Collections.Generic;
using KemothStudios.Utility.Events;
using UnityEngine;

namespace KemothStudios.Board
{
    /// <summary>
    /// Class to listen to line click even and iterate over shared cells and check if they are completed, invokes event with the newly completed cells
    /// </summary>
    public class BoardManager : MonoBehaviour
    {
        [SerializeField, Min(0f), Tooltip("Delay after which cell completion process completes")]
        private float _cellCompletionDelay = 0.15f;
        [SerializeField] private BoardDataSO _boardData;

        private EventBinding<DrawLineEvent> _drawLineEvent;

        void Start()
        {
            _drawLineEvent = new EventBinding<DrawLineEvent>(CheckForCompletedCells);
            EventBus<DrawLineEvent>.RegisterBinding(_drawLineEvent);
        }

        private void OnDestroy()
        {
            EventBus<DrawLineEvent>.UnregisterBinding(_drawLineEvent);
        }

        private void CheckForCompletedCells(DrawLineEvent lineData)
        {
            StartCoroutine(HandleCellCompletion(lineData.Line.SharedCells));
        }

        private IEnumerator HandleCellCompletion(IEnumerable<Cell> cells)
        {
            List<Cell> completedCells = new List<Cell>();
            
            foreach (Cell cell in cells)
            {
                if (cell.IsCellCompleted)
                {
                    completedCells.Add(cell);
                }
            }
            int completedCellsCount = completedCells.Count;
            if (completedCellsCount > 0)
            {
                EventBus<CellsAcquireStartedEvent>.RaiseEvent(new CellsAcquireStartedEvent(completedCells));
                _boardData.CompletedCellsCount += completedCells.Count;
                foreach (Cell cell in completedCells)
                {
                    cell.MarkCellCompleted();
                    yield return new WaitForSeconds(_cellCompletionDelay);
                }

                EventBus<CellAcquireCompletedEvent>.RaiseEvent(new CellAcquireCompletedEvent());
            }
            EventBus<BoardReadyAfterDrawLineEvent>.RaiseEvent(new BoardReadyAfterDrawLineEvent());
        }
    }
}