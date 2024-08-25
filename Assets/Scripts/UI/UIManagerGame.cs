using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

namespace KemothStudios.UI
{
    /// <summary>
    /// Handles game UI, crrently hardcoded for 2 players only becase of how the UI is setup
    /// </summary>
    public class UIManagerGame : MonoBehaviour
    {
        [SerializeField] private UIDocument _uiDocument;
        [SerializeField] private GameDataSO _gameDataSO;
        [SerializeField] private PlayerAvatarsSO _playerAvatarsSO;

        [SerializeField] private UIData[] _playerUI;

        private IEnumerator Start()
        {
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
        }

        private void OnDestroy()
        {
            if (TurnHandler.Instance != null)
                TurnHandler.Instance.TurnUpdated -= ShowTurnIndicator;
            if (ScoreManager.Instance != null)
                ScoreManager.Instance.ScoreUpdated += UpdateScoreText;
        }

        private void UpdateScoreText()
        {
            if(_gameDataSO.TryGetPlayerScore(TurnHandler.Instance.CurrentPlayerIndex, out int score))
                _playerUI[TurnHandler.Instance.CurrentPlayerIndex].PlayerScore.text = score.ToString();
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

            public UIData(UIDocument iuDocment, string containerName)
            {
                VisualElement container = iuDocment.rootVisualElement.Q<VisualElement>(containerName);
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