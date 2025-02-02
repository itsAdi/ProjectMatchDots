using System.Collections;
using System.Collections.Generic;
using KemothStudios.Utility;
using KemothStudios.Utility.Events;
using KemothStudios.Utility.States;
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
        private EventBinding<GameStartedEvent> _startGameEvent;
        private EventBinding<CellAcquireCompletedEvent> _cellAcquireCompletedEvent;
        private FiniteStateMachine _boardStateMachine;
        private TriggerPredicate _enableInputTrigger = new(), _cellAcquireTrigger = new(), _gameOverTrigger = new();
        private BoardStateAcquiringCell.StateData _cellAcquireStateData;

        void Start()
        {
            _cellAcquireStateData = new BoardStateAcquiringCell.StateData(_cellCompletionDelay);
            _boardStateMachine = new FiniteStateMachine();
            _boardStateMachine.AddTransition(new BoardStateAcquiringCell(_cellAcquireStateData), new BoardStateIdle(), _gameOverTrigger);
            _boardStateMachine.AddTransition(new BoardStateIdle(), new BoardStateTakeInput(_boardData), _enableInputTrigger);
            _boardStateMachine.AddTransition(new BoardStateAcquiringCell(_cellAcquireStateData), new BoardStateTakeInput(_boardData), _enableInputTrigger);
            _boardStateMachine.AddTransition(new BoardStateTakeInput(_boardData), new BoardStateAcquiringCell(_cellAcquireStateData), _cellAcquireTrigger);
            _boardStateMachine.SetState(new BoardStateIdle());
            _drawLineEvent = new EventBinding<DrawLineEvent>(CheckForCompletedCells);
            EventBus<DrawLineEvent>.RegisterBinding(_drawLineEvent);
            _startGameEvent = new EventBinding<GameStartedEvent>(() => { _enableInputTrigger.EnableTrigger(); });
            EventBus<GameStartedEvent>.RegisterBinding(_startGameEvent);
            _cellAcquireCompletedEvent = new EventBinding<CellAcquireCompletedEvent>(() =>
            {
                if (_boardData.CompletedCellsCount != _boardData.TotalCellsCount)
                    _enableInputTrigger.EnableTrigger();
                else _gameOverTrigger.EnableTrigger();
            });
            EventBus<CellAcquireCompletedEvent>.RegisterBinding(_cellAcquireCompletedEvent);
        }

        private void Update()
        {
            _boardStateMachine.Update();
        }

        private void OnDestroy()
        {
            EventBus<DrawLineEvent>.UnregisterBinding(_drawLineEvent);
            EventBus<GameStartedEvent>.UnregisterBinding(_startGameEvent);
            EventBus<CellAcquireCompletedEvent>.UnregisterBinding(_cellAcquireCompletedEvent);
        }

        private void CheckForCompletedCells(DrawLineEvent lineData)
        {
            List<Cell> completedCells = new();
            foreach (Cell cell in lineData.Line.SharedCells)
            {
                if (cell.IsCellCompleted)
                {
                    completedCells.Add(cell);
                }
            }

            int completedCellsCount = completedCells.Count;
            if (completedCellsCount > 0)
            {
                _boardData.CompletedCellsCount += completedCellsCount;
                _cellAcquireStateData.CellsCompleted = completedCells;
                _cellAcquireTrigger.EnableTrigger();
            }
            else
                EventBus<BoardReadyAfterDrawLineEvent>.RaiseEvent(new BoardReadyAfterDrawLineEvent());
        }
    }
}