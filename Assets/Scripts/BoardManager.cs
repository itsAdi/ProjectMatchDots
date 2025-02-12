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

        [SerializeField, Tooltip("Camera clear color when we are on board scene")]
        private Color _boardSceneColor = Color.black;

        [SerializeField] private BoardDataSO _boardData;

        private EventBinding<DrawLineEvent> _drawLineEvent;
        private EventBinding<PlayerWonEvent> _playerWonEvent;
        private EventBinding<GameStartedEvent> _startGameEvent;
        private EventBinding<CellAcquireCompletedEvent> _gameResultCheckedEvent;
        private FiniteStateMachine _boardStateMachine;
        private TriggerPredicate _enableInputTrigger = new(), _cellAcquireTrigger = new(), _gameOverTrigger = new();
        private BoardStateAcquiringCell.StateData _cellAcquireStateData;
        private EventBinding<SceneLoadingCompleteEvent> _sceneLoadComplete;

        private Color _originalCameraBackGroundColor;

        void Start()
        {
            _cellAcquireStateData = new BoardStateAcquiringCell.StateData(_cellCompletionDelay);
            _boardStateMachine = new FiniteStateMachine();
            _boardStateMachine.AddAnyTransition(new BoardStateIdle(), _gameOverTrigger);
            _boardStateMachine.AddTransition(new BoardStateIdle(), new BoardStateTakeInput(_boardData), _enableInputTrigger);
            _boardStateMachine.AddTransition(new BoardStateAcquiringCell(_cellAcquireStateData), new BoardStateTakeInput(_boardData), _enableInputTrigger);
            _boardStateMachine.AddTransition(new BoardStateTakeInput(_boardData), new BoardStateAcquiringCell(_cellAcquireStateData), _cellAcquireTrigger);
            _boardStateMachine.Initialize(new BoardStateIdle());

            _drawLineEvent = new EventBinding<DrawLineEvent>(CheckForCompletedCells);
            EventBus<DrawLineEvent>.RegisterBinding(_drawLineEvent);
            _startGameEvent = new EventBinding<GameStartedEvent>(_enableInputTrigger.EnableTrigger);
            EventBus<GameStartedEvent>.RegisterBinding(_startGameEvent);
            _sceneLoadComplete = new EventBinding<SceneLoadingCompleteEvent>(StartGame);
            EventBus<SceneLoadingCompleteEvent>.RegisterBinding(_sceneLoadComplete);
            _gameResultCheckedEvent = new EventBinding<CellAcquireCompletedEvent>(() =>
            {
                if (_boardData.CompletedCellsCount != _boardData.TotalCellsCount)
                    _enableInputTrigger.EnableTrigger();
                else
                    _gameOverTrigger.EnableTrigger();
            });
            EventBus<CellAcquireCompletedEvent>.RegisterBinding(_gameResultCheckedEvent);
            _playerWonEvent = new EventBinding<PlayerWonEvent>(GameOver);
            EventBus<PlayerWonEvent>.RegisterBinding(_playerWonEvent);

            _originalCameraBackGroundColor = Camera.main.backgroundColor;
            Camera.main.backgroundColor = _boardSceneColor;
        }

        private void Update()
        {
            _boardStateMachine.Update();
        }

        private void OnDestroy()
        {
            if (Camera.main != null) Camera.main.backgroundColor = _originalCameraBackGroundColor;
            EventBus<DrawLineEvent>.UnregisterBinding(_drawLineEvent);
            EventBus<PlayerWonEvent>.UnregisterBinding(_playerWonEvent);
            EventBus<GameStartedEvent>.UnregisterBinding(_startGameEvent);
            EventBus<SceneLoadingCompleteEvent>.UnregisterBinding(_sceneLoadComplete);
            EventBus<CellAcquireCompletedEvent>.UnregisterBinding(_gameResultCheckedEvent);
        }

        private void StartGame()
        {
            if (GameStates.Instance.CurrentState == GameStates.States.GAME)
                EventBus<GameStartedEvent>.RaiseEvent(new GameStartedEvent());
        }

        private void GameOver()
        {
            _boardStateMachine.Enabled = false;
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