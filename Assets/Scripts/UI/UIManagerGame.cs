using System;
using KemothStudios.Board;
using Cysharp.Threading.Tasks;
using KemothStudios.Utility;
using KemothStudios.Utility.Events;
using UnityEngine;
using UnityEngine.UIElements;

namespace KemothStudios.UI
{
    /// <summary>
    /// Handles game UI, currently hardcoded for 2 players only because of how the UI is set up
    /// </summary>
    public class UIManagerGame : MonoBehaviour
    {
        [SerializeField] private UIDocument _uiDocument;
        [SerializeField] private UIDocument _gameResultUIDocument;
        [SerializeField] private GameDataSO _gameDataSO;
        [SerializeField] private PlayerAvatarsSO _playerAvatarsSO;
        [SerializeField] private BoardDataSO _boardDataSO;
        [SerializeField, Min(0)] private int _gameResultUIShowDelay = 2;

        private UIData[] _playerUI;
        private VisualElement _gameResultUI, _gameResultUIPlayerAvatar;
        private LabelAutoFit _gameResultUIPlayerName, _gameResultUIPlayerScore;
        private Button _gameResultUIHomeButton;
        private Button _gameResultUIRestartButton;
        private VisualElement[] _cellIcons;
        private EventBinding<CellAcquiredEvent> _cellCompleted;
        private EventBinding<ScoreUpdatedEvent> _scoreUpdated;
        private EventBinding<PlayerWonEvent> _playerWon;
        private EventBinding<ShowVictoryEvent> _showVictory;
        private EventBinding<TurnStartedEvent> _turnStarted;
        private EventBinding<TurnEndedEvent> _turnEnded;
        private Player _currentPlayer;

        private void Start()
        {
            // Setup score UI ...
            _playerUI = new[]
            {
                new UIData(_uiDocument, "playerAContainer"),
                new UIData(_uiDocument, "playerBContainer")
            };
            for (int i = 0; i < 2; i++)
            {
                if (_gameDataSO.TryGetPlayerAvatarIndex(i, out int avatarIndex) && _playerAvatarsSO.TryGetAvatarAtIndex(avatarIndex, out Sprite avatar))
                    _playerUI[i].PlayerAvatar.style.backgroundImage = new StyleBackground(avatar);
                if (_gameDataSO.TryGetPlayerName(i, out string playerName))
                    _playerUI[i].PlayerName.text = playerName;
                _playerUI[i].PlayerScore.text = "0";
            }

            _playerWon = new EventBinding<PlayerWonEvent>(StartVictorySequenceWrapper);
            EventBus<PlayerWonEvent>.RegisterBinding(_playerWon);

            _scoreUpdated = new EventBinding<ScoreUpdatedEvent>(UpdateScoreText);
            EventBus<ScoreUpdatedEvent>.RegisterBinding(_scoreUpdated);

            _showVictory = new EventBinding<ShowVictoryEvent>(ShowVictoryScreen);
            EventBus<ShowVictoryEvent>.RegisterBinding(_showVictory);

            _turnStarted = new EventBinding<TurnStartedEvent>(ShowTurnIndicator);
            _turnStarted.AddEvent(SetCurrentPlayer);
            EventBus<TurnStartedEvent>.RegisterBinding(_turnStarted);

            _turnEnded = new EventBinding<TurnEndedEvent>(HideTurnIndicator);
            EventBus<TurnEndedEvent>.RegisterBinding(_turnEnded);

            // Setup Game Result UI ...
            _gameResultUI = _gameResultUIDocument.rootVisualElement.GetVisualElement("gameResultUI", "gameResultUI not found in game result UI document");
            _gameResultUIPlayerName = _gameResultUI.GetVisualElement<LabelAutoFit>("playerName", "playerName not found in game result UI");
            _gameResultUIPlayerScore = _gameResultUI.GetVisualElement<LabelAutoFit>("playerScore", "playerScore not found in game result UI");
            _gameResultUIPlayerAvatar = _gameResultUI.GetVisualElement<VisualElement>("playerAvatar", "playerAvatar not found in game result UI");
            _gameResultUIHomeButton = _gameResultUI.GetVisualElement<Button>("HomeButton", "homeButton not found in game result UI");
            _gameResultUIRestartButton = _gameResultUI.GetVisualElement<Button>("restartButton", "restartButton not found in game result UI");

            // Setup Cell Owner Avatars UI ...
            VisualElement elem = _uiDocument.rootVisualElement.GetVisualElement<VisualElement>("cellOwnerAvatarContainer", "cellOwnerAvatarContainer not found in UIDocument root");
            Rect r = _boardDataSO.GetCell(0).CellTransform;
            Vector2 min = Camera.main.WorldToViewportPoint(new Vector2(r.min.x, r.max.y));
            r = _boardDataSO.GetCell(_boardDataSO.TotalCellsCount - 1).CellTransform;
            Vector2 max = Camera.main.WorldToViewportPoint(new Vector2(r.max.x, r.min.y));
            elem.style.width = Length.Percent((max.x - min.x) * 100f);
            elem.style.height = Length.Percent((min.y - max.y) * 100f);
            elem.style.left = Length.Percent(min.x * 100f);
            elem.style.top = Length.Percent((1f - min.y) * 100f);
            float cellWidth = 100f / _boardDataSO.ColumnsCount;
            float cellHeight = 100f / _boardDataSO.RowsCount;
            _cellIcons = new VisualElement[_boardDataSO.TotalCellsCount];
            for (int i = 0; i < _boardDataSO.TotalCellsCount; i++)
            {
                VisualElement avatar = new VisualElement();
                avatar.style.backgroundColor = Color.white;
                avatar.style.width = Length.Percent(cellWidth);
                avatar.style.height = Length.Percent(cellHeight);
                avatar.style.visibility = Visibility.Hidden;
                elem.Add(avatar);
                _cellIcons[i] = avatar;
            }

            _cellCompleted = new EventBinding<CellAcquiredEvent>(AddCellOwnerIcon);
            EventBus<CellAcquiredEvent>.RegisterBinding(_cellCompleted);
        }

        private void OnDestroy()
        {
            EventBus<ScoreUpdatedEvent>.UnregisterBinding(_scoreUpdated);
            EventBus<PlayerWonEvent>.UnregisterBinding(_playerWon);
            EventBus<CellAcquiredEvent>.UnregisterBinding(_cellCompleted);
            EventBus<ShowVictoryEvent>.UnregisterBinding(_showVictory);
            EventBus<TurnStartedEvent>.UnregisterBinding(_turnStarted);
            EventBus<TurnEndedEvent>.UnregisterBinding(_turnEnded);
        }

        private void SetCurrentPlayer(TurnStartedEvent turnData) => _currentPlayer = turnData.Player;
        
        private void AddCellOwnerIcon(CellAcquiredEvent cellData)
        {
            if (_boardDataSO.TryGetCellIndex(cellData.Cell, out var cellIndex))
            {
                if (_playerAvatarsSO.TryGetAvatarAtIndex(_currentPlayer.AvatarIndex, out Sprite avatar))
                {
                    _cellIcons[cellIndex].style.backgroundImage = new StyleBackground(avatar);
                    _cellIcons[cellIndex].style.visibility = Visibility.Visible;
                }
                else
                    DebugUtility.LogError($"<color=red>Could not get avatar on avatar index {_currentPlayer.AvatarIndex} for player {_currentPlayer.Name}</color>");
            }
            else DebugUtility.LogError("<color=red>Could not get cell index to add player avatar</color>");
        }

        private void UpdateScoreText()
        {
            if (_gameDataSO.TryGetPlayerScore(_currentPlayer.PlayerIndex, out int score))
                _playerUI[_currentPlayer.PlayerIndex].PlayerScore.text = score.ToString();
        }

        // Wrapper method for StartVictorySequence because it returns an UniTaskVoid and we cant use it directly in our event binding
        private void StartVictorySequenceWrapper(PlayerWonEvent wonEvent) => StartVictorySequence(wonEvent).Forget();
        
        /// <summary>
        /// Stops turn indicator and after a delay shows victory screen
        /// </summary>
        private async UniTaskVoid StartVictorySequence(PlayerWonEvent winnerData)
        {
            try
            {
                await UniTask.Delay(_gameResultUIShowDelay * 1000);
                EventBus<ShowVictoryEvent>.RaiseEvent(new ShowVictoryEvent(winnerData.WinnerPlayer));
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                throw;
            }
        }

        private void ShowVictoryScreen(ShowVictoryEvent victoryEvent)
        {
            _gameResultUI.AddToClassList("showGameResultUI");
            _gameResultUIPlayerName.Text = victoryEvent.Player.Name;
            _gameResultUIPlayerScore.Text = $"{victoryEvent.Player.GetScore}";
            if (_playerAvatarsSO.TryGetAvatarAtIndex(victoryEvent.Player.AvatarIndex, out Sprite avatar))
                _gameResultUIPlayerAvatar.style.backgroundImage = new StyleBackground(avatar);
            else DebugUtility.LogError($"Could not find player avatar for avatar index {victoryEvent.Player.AvatarIndex}");
            _gameResultUI.RegisterCallbackOnce<TransitionEndEvent>(_ => { _gameResultUI.Q<VisualElement>("window").AddToClassList("showGameResultWindow"); });
            _gameResultUIHomeButton.clicked += () =>
            {
                EventBus<MajorButtonClickedEvent>.RaiseEvent(new MajorButtonClickedEvent());
                GameStates.Instance.CurrentState = GameStates.States.MAIN_MENU;
            };
            _gameResultUIRestartButton.clicked += () =>
            {
                EventBus<MajorButtonClickedEvent>.RaiseEvent(new MajorButtonClickedEvent());
                Player[] players = new Player[_gameDataSO.PlayerCount];
                foreach (Player player in _gameDataSO.Players)
                {
                    Player p = new Player(player.Name, player.AvatarIndex, player.PlayerIndex, _gameDataSO);
                    players[player.PlayerIndex] = p;
                }
                _gameDataSO.InitializePlayerData(players);
                EventBus<RestartSceneEvent>.RaiseEvent(new RestartSceneEvent());
            };
        }

        private void ShowTurnIndicator(TurnStartedEvent turnData)
        {
            int playerIndex = turnData.Player.PlayerIndex;
            UIData data = _playerUI[playerIndex];
            data.ShowTurnIndicator();
            _playerUI[playerIndex] = data;
        }

        private void HideTurnIndicator(TurnEndedEvent turnData)
        {
            int playerIndex = turnData.Player.PlayerIndex;
            UIData data = _playerUI[playerIndex];
            data.HideTurnIndicator();
            _playerUI[playerIndex] = data;
        }

        // Struct to handle each player details panel
        private struct UIData
        {
            public VisualElement PlayerAvatar { get; }
            public Label PlayerName { get; }
            public Label PlayerScore { get; }

            private VisualElement _turnIndicator;
            private bool _turnIndicatorActive;

            // Saving callback here because RegisterCallback//UnRegisterCallback methods saves a reference to the method
            // we are using a struct and structs are not reference types, so after registering we would not be able
            // to unregister it because reference will be different
            EventCallback<TransitionEndEvent> _callback;

            public UIData(UIDocument iuDocument, string containerName)
            {
                VisualElement container = iuDocument.rootVisualElement.Q<VisualElement>(containerName);
                PlayerAvatar = container.Q<VisualElement>("playerAvatar");
                PlayerName = container.Q<Label>("playerName");
                PlayerScore = container.Q<Label>("playerScore");
                _turnIndicator = PlayerAvatar.Q<VisualElement>("turnIndicator");
                _turnIndicatorActive = false;
                _callback = null;
            }

            public void ShowTurnIndicator()
            {
                if (_turnIndicatorActive) return;
                _turnIndicatorActive = true;
                if (_callback == null)
                    _callback = ToggleTurnIndicatorAnimation;
                _turnIndicator.RegisterCallback(_callback);
                _turnIndicator.ToggleInClassList("turnIndicatorShow");
            }

            public void HideTurnIndicator()
            {
                if (!_turnIndicatorActive) return;
                _turnIndicatorActive = false;
                if (_callback != null)
                    _turnIndicator.UnregisterCallback(_callback);

                if (_turnIndicator.ClassListContains("turnIndicatorShow"))
                    _turnIndicator.RemoveFromClassList("turnIndicatorShow");
            }

            private void ToggleTurnIndicatorAnimation(TransitionEndEvent _)
            {
                _turnIndicator.ToggleInClassList("turnIndicatorShow");
            }
        }
    }
}