using KemothStudios.Board;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace KemothStudios.UI
{
    /// <summary>
    /// Handles game UI, currently hardcoded for 2 players only because of how the UI is setup
    /// </summary>
    public class UIManagerGame : MonoBehaviour
    {
        [SerializeField] private UIDocument _uiDocument;
        [SerializeField] private UIDocument _gameResultUIDocument;
        [SerializeField] private GameDataSO _gameDataSO;
        [SerializeField] private PlayerAvatarsSO _playerAvatarsSO;
        [SerializeField] private BoardDataSO _boardDataSO;
        [SerializeField, Min(0)] private int _gameResultUIShowDelay = 2;
        [SerializeField] private UIData[] _playerUI;

        private VisualElement _gameResultUI, _gameResultUIPlayerAvatar;
        private LabelAutoFit _gameResultUIPlayerName, _gameResultUIPlayerScore;
        private Button _gameResultUIHomeButton;
        private VisualElement[] _cellIcons;

        private IEnumerator Start()
        {
            // Setup score UI ...
            _playerUI = new[] {
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
            yield return new WaitUntil(() => TurnHandler.Instance == null || TurnHandler.Instance.IsReady);
            _playerUI[TurnHandler.Instance.CurrentPlayerIndex].ShowTurnIndicator();
            TurnHandler.Instance.TurnUpdated += ShowTurnIndicator;
            yield return new WaitUntil(() => ScoreManager.Instance != null);
            ScoreManager.Instance.ScoreUpdated += UpdateScoreText;

            // Setup Game Result UI ...
            yield return new WaitUntil(() => GameResultManager.Instance != null);
            _gameResultUI = _gameResultUIDocument.rootVisualElement.Q<VisualElement>("gameResultUI");
            _gameResultUIPlayerName = _gameResultUI.Q<LabelAutoFit>("playerName");
            _gameResultUIPlayerScore = _gameResultUI.Q<LabelAutoFit>("playerScore");
            _gameResultUIPlayerAvatar = _gameResultUI.Q<VisualElement>("playerAvatar");
            _gameResultUIHomeButton = _gameResultUI.Q<Button>("HomeButton");
            GameResultManager.Instance.PlayerWon += ShowGameResultUI;

            // Setup Cell Owner Avatars UI ...
            VisualElement elem = _uiDocument.rootVisualElement.Q<VisualElement>("cellOwnerAvatarContainer");
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

            GameManager.Instance.OnCellCompleted += AddCellOwnerIcon;
        }

        private void OnDestroy()
        {
            if (TurnHandler.Instance != null)
                TurnHandler.Instance.TurnUpdated -= ShowTurnIndicator;
            if (ScoreManager.Instance != null)
                ScoreManager.Instance.ScoreUpdated -= UpdateScoreText;
            if (GameResultManager.Instance != null)
                GameResultManager.Instance.PlayerWon -= ShowGameResultUI;
            GameManager.Instance.OnCellCompleted -= AddCellOwnerIcon;
        }

        private void AddCellOwnerIcon(IEnumerable<Cell> enumerable)
        {
            foreach (Cell cell in enumerable)
            {
                if (_boardDataSO.TryGetCellIndex(cell, out var index))
                {
                    if (_playerAvatarsSO.TryGetAvatarAtIndex(TurnHandler.Instance.CurrentPlayerIndex, out Sprite avatar))
                    {
                        _cellIcons[index].style.backgroundImage = new StyleBackground(avatar);
                        _cellIcons[index].style.visibility = Visibility.Visible;
                    }else Debug.LogError($"Could not get player avatar for player on index {TurnHandler.Instance.CurrentPlayerIndex}");
                }
                else Debug.LogError("Could not get cell index to add player avatar");
            }
        }

        private void UpdateScoreText()
        {
            if (_gameDataSO.TryGetPlayerScore(TurnHandler.Instance.CurrentPlayerIndex, out int score))
                _playerUI[TurnHandler.Instance.CurrentPlayerIndex].PlayerScore.text = score.ToString();
        }

        private async void ShowGameResultUI(int winnerPlayerIndex)
        {
            UIData data = _playerUI[TurnHandler.Instance.CurrentPlayerIndex];
            data.HideTurnIndicator();
            _playerUI[winnerPlayerIndex] = data;
            await Task.Delay(_gameResultUIShowDelay * 1000);
            _gameResultUI.AddToClassList("showGameResultUI");
            if (_gameDataSO.TryGetPlayerName(winnerPlayerIndex, out string name))
                _gameResultUIPlayerName.Text = name;
            else Debug.LogError($"Could not find player name on index {winnerPlayerIndex}");
            if (_gameDataSO.TryGetPlayerScore(winnerPlayerIndex, out int score))
                _gameResultUIPlayerScore.Text = $"{score}";
            else Debug.LogError($"Could not find player score on index {winnerPlayerIndex}");
            if (_gameDataSO.TryGetPlayerAvatarIndex(winnerPlayerIndex, out int avatarIndex))
                if (_playerAvatarsSO.TryGetAvatarAtIndex(avatarIndex, out Sprite avatar))
                    _gameResultUIPlayerAvatar.style.backgroundImage = new StyleBackground(avatar);
                else Debug.LogError($"Could not find player avatar for avatar index {avatarIndex}");
            else Debug.LogError($"Could not find avatar index for player on index {winnerPlayerIndex}");
            _gameResultUI.RegisterCallbackOnce<TransitionEndEvent>(_ =>
            {
                _gameResultUI.Q<VisualElement>("window").AddToClassList("showGameResultWindow");
            });
            _gameResultUIHomeButton.clicked += () =>
            {
                GameStates.Instance.CurrentState = GameStates.States.MAIN_MENU;
            };
        }

        private void ShowTurnIndicator()
        {
            UIData data = _playerUI[TurnHandler.Instance.LastPlayerIndex];
            data.HideTurnIndicator();
            _playerUI[TurnHandler.Instance.LastPlayerIndex] = data;
            data = _playerUI[TurnHandler.Instance.CurrentPlayerIndex];
            data.ShowTurnIndicator();
            _playerUI[TurnHandler.Instance.CurrentPlayerIndex] = data;
        }

        // Struct to handle each player details panel
        private struct UIData
        {
            public VisualElement PlayerAvatar { get; private set; }
            public Label PlayerName { get; private set; }
            public Label PlayerScore { get; private set; }

            private VisualElement _turnIndicator;
            private bool _turnIndicatorActive;

            // Saving callback here because RegisterCallback//UnRegisterCallback methods saves a reference to the method
            // and we are using a struct and structs are not reference types, so after registering we would not be able
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