using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using KemothStudios.Utility;
using KemothStudios.Utility.Events;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;
using ProgressBar = KemothStudios.UI.ProgressBar;

namespace KemothStudios
{
    public class TurnTimer : MonoBehaviour
    {
        [SerializeField] private PlayerTimeConfigSO _playerTimeConfig;
        [SerializeField] private UIDocument _uiDocument;
        [SerializeField] private GameDataSO _gameData;

        private EventBinding<TurnStartedEvent> _playerTurnStarted;
        private EventBinding<TurnEndedEvent> _playerDrawLine;
        private CancellationTokenSource _stopTimerToken; // Use this to stop timer while it is still running
        private CancellationTokenSource _cancellationToken; // this token will be used in task and will be a linked token
        private ProgressBar[] _turnTimerBars;
        private Label[] _playerLifeLabels;

        private void Start()
        {
            Statics.Assert(_playerTimeConfig != null, $"{nameof(PlayerTimeConfigSO)} is not provided to {nameof(TurnTimer)}");
            Statics.Assert(_gameData != null, $"{nameof(GameDataSO)} is not provided to {nameof(TurnTimer)}");
            Statics.Assert(_uiDocument != null, $"UI document is not provided to {nameof(TurnTimer)}");

            _playerTurnStarted = new EventBinding<TurnStartedEvent>(StartTimer);
            EventBus<TurnStartedEvent>.RegisterBinding(_playerTurnStarted);

            _playerDrawLine = new EventBinding<TurnEndedEvent>(StopTimer);
            EventBus<TurnEndedEvent>.RegisterBinding(_playerDrawLine);

            int playerCount = _gameData.PlayerCount;
            try
            {
                _playerLifeLabels = new Label[playerCount];
                UQueryBuilder<Label> query = _uiDocument.rootVisualElement.Query<Label>("playerLives");
                for (int i = 0; i < playerCount; i++)
                {
                    Label playerLifeLabel = query.AtIndex(i);
                    _playerLifeLabels[i] = playerLifeLabel;
                    UpdatePlayerTime(i, _playerTimeConfig.MaxPlayerTime);
                }
            }
            catch (Exception e)
            {
                EventBus<ShowMessageEvent>.RaiseEvent(new ShowMessageEvent("Exception creating player lives labels, game will not continue"));
                DebugUtility.LogException(e);
            }

            try
            {
                _turnTimerBars = new ProgressBar[playerCount];
                UQueryBuilder<ProgressBar> query = _uiDocument.rootVisualElement.Query<ProgressBar>("turnTimer");
                for (int i = 0; i < playerCount; i++)
                {
                    ProgressBar p = query.AtIndex(i);
                    p.FillAmount = 100f;
                    _turnTimerBars[i] = p;
                }
            }
            catch (Exception e)
            {
                EventBus<ShowMessageEvent>.RaiseEvent(new ShowMessageEvent("Exception in building timer objects, game will not continue"));
                DebugUtility.LogException(e);
            }
        }

        private void OnDestroy()
        {
            _stopTimerToken.Dispose();
            _cancellationToken.Dispose();
            EventBus<TurnStartedEvent>.UnregisterBinding(_playerTurnStarted);
            EventBus<TurnEndedEvent>.UnregisterBinding(_playerDrawLine);
        }

        private void StartTimer(TurnStartedEvent turnData)
        {
            _stopTimerToken = new CancellationTokenSource();
            _cancellationToken = CancellationTokenSource.CreateLinkedTokenSource(_stopTimerToken.Token, destroyCancellationToken);

            RunTimer(turnData.Player).Forget();
        }

        private void StopTimer(TurnEndedEvent turnData)
        {
            _stopTimerToken.Cancel();
            _stopTimerToken.Dispose();
            _cancellationToken.Dispose();
        }

        private async UniTaskVoid RunTimer(Player forPlayer)
        {
            try
            {
                int maxTime = _playerTimeConfig.MaxPlayerTime;
                int currentTime = forPlayer.GetRemainingTime;
                int playerIndex = forPlayer.PlayerIndex;
                while (_turnTimerBars[playerIndex].FillAmount > 0f)
                {
                    await UniTask.WaitForSeconds(1f, cancellationToken: _cancellationToken.Token);
                    currentTime = Mathf.Max(0, currentTime - 1);
                    _turnTimerBars[playerIndex].FillAmount = Mathf.InverseLerp(0, maxTime, currentTime) * 100f;
                    UpdatePlayerTime(playerIndex, currentTime);
                }

                EventBus<TurnTimerElapsedEvent>.RaiseEvent(new TurnTimerElapsedEvent());
            }
            catch (OperationCanceledException)
            {
            } // when a task will be cancelled this will prevent catch block below from executing
            catch (Exception e)
            {
                DebugUtility.LogException(e);
            }
        }

        private void UpdatePlayerTime(int playerIndex, int seconds)
        {
            if (_gameData.TrySetPlayerTime(playerIndex, seconds))
            {
                int m = seconds / 60;
                int s = seconds % 60;
                _playerLifeLabels[playerIndex].text = $"{m}:{s}";
            }
            else
            {
                EventBus<ShowMessageEvent>.RaiseEvent(new ShowMessageEvent("Failed to set player data, game will not continue"));
                throw new ArgumentOutOfRangeException($"TrySetPlayerLives failed for player index {playerIndex} when player count is {_gameData.PlayerCount}");
            }
        }
    }
}