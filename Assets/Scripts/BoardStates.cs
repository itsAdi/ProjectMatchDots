using System.Collections.Generic;
using KemothStudios.Board;
using KemothStudios.Utility;
using KemothStudios.Utility.Events;
using KemothStudios.Utility.States;
using UnityEngine;
using UnityEngine.InputSystem;

namespace KemothStudios
{
    /// <summary>
    /// An empty state to keep board in idle state
    /// </summary>
    public class BoardStateIdle : BaseState { }

    /// <summary>
    /// State to enable board to take input
    /// </summary>
    public class BoardStateTakeInput : BaseState
    {
        private BoardDataSO _boardData;

        public BoardStateTakeInput(BoardDataSO boardData) => _boardData = boardData;

        public override void Update()
        {
            base.Update();
            Pointer pointer;
#if UNITY_EDITOR
            pointer = Mouse.current;
#else
        pointer = Touchscreen.current;
#endif
            if (pointer.press.wasPressedThisFrame)
            {
                Ray r = Camera.main.ScreenPointToRay(pointer.position.value);
                if (new Plane(Vector3.back, Vector3.zero).Raycast(r, out float hit))
                {
                    Vector3 hitPoint = r.GetPoint(hit);
                    if (_boardData.TryGetCellIndex(hitPoint, out int cellIndex))
                        _boardData.GetCell(cellIndex).CellClicked(hitPoint);
                }
            }
        }
    }

    /// <summary>
    /// State to handle cell acquire animations
    /// </summary>
    public class BoardStateAcquiringCell : BaseState
    {
        private StateData _stateData;
        private float _elapsedCellAcquiringTime;
        private IEnumerator<Cell> _cellsToAcquireEnumerator;

        public BoardStateAcquiringCell(StateData stateData) => _stateData = stateData;

        public override void Enter()
        {
            base.Enter();
            EventBus<CellsAcquireStartedEvent>.RaiseEvent(new CellsAcquireStartedEvent(_stateData.CellsCompleted));
            _cellsToAcquireEnumerator = _stateData.CellsCompleted.GetEnumerator();
        }

        public override void Update()
        {
            base.Update();
            if (_elapsedCellAcquiringTime == 0f)
            {
                if (_cellsToAcquireEnumerator.MoveNext())
                    _cellsToAcquireEnumerator.Current?.MarkCellCompleted();
                else
                    EventBus<CellAcquireCompletedEvent>.RaiseEvent(new CellAcquireCompletedEvent());
            }

            _elapsedCellAcquiringTime += Time.deltaTime;
            if (_elapsedCellAcquiringTime >= _stateData.CellCompletionDelay)
                _elapsedCellAcquiringTime = 0f;
        }

        public override void Exit()
        {
            base.Exit();
            _cellsToAcquireEnumerator.Dispose();
            _elapsedCellAcquiringTime = 0f;
            EventBus<BoardReadyAfterDrawLineEvent>.RaiseEvent(new BoardReadyAfterDrawLineEvent());
        }

        public sealed class StateData
        {
            public float CellCompletionDelay { get; }
            public IEnumerable<Cell> CellsCompleted;

            public StateData(float cellCompletionDelay)
            {
                CellCompletionDelay = cellCompletionDelay;
            }
        }
    }
}