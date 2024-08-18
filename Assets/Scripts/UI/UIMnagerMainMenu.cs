using UnityEngine;
using UnityEngine.UIElements;

namespace KemothStudios
{
    /// <summary>
    /// UI manager for main menu
    /// </summary>
    public class UIMnagerMainMenu : MonoBehaviour
    {
        [SerializeField] private UIDocument _uiDoument;
        [SerializeField] private GameDataSO _gameData;
        [SerializeField] private PlayerAvatarsSO _playerAvatars;

        private TextField _playerAName, _playerBName;
        private Button _startGameButton;
        private RadioButton[] _playerAvatarControls;
        private int _playerAAvatarIndex = 0, _playerBAvatarIndex = 1; // two players cannot have same avatar so starting with different avatars

        private void Start()
        {
            _playerAName = _uiDoument.rootVisualElement.Q<TextField>("playerAName");
            _playerBName = _uiDoument.rootVisualElement.Q<TextField>("playerBName");
            _startGameButton = _uiDoument.rootVisualElement.Q<Button>("startGameButton");

            if (_playerAvatars.GetAvatarsCount > 0)
            {
                _playerAvatarControls = new RadioButton[_playerAvatars.GetAvatarsCount];
                VisualElement group = _uiDoument.rootVisualElement.Query<VisualElement>("iconsGrid").Children<GroupBox>();
                for (int i = 0; i < _playerAvatars.GetAvatarsCount; i++)
                {
                    RadioButton r = new();
                    r.AddToClassList("gridIcon");
                    if (_playerAvatars.TryGetAvatarAtIndex(i, out Sprite avatarImage))
                    {
                        r.Q(className: "unity-radio-button__checkmark-background").style.backgroundImage = new StyleBackground(avatarImage);
                    }
                    if (i == 0) r.value = true;
                    int index = i;
                    r.RegisterValueChangedCallback(x =>
                    {
                        if (x.newValue)
                            Debug.Log(index);
                    });
                    _playerAvatarControls[i] = r;
                    group.Add(r);
                }
            }

            _startGameButton.clicked += StartGame;
        }

        private void OnDestroy()
        {
            _startGameButton.clicked -= StartGame;
        }

        private void StartGame()
        {
            if (string.IsNullOrEmpty(_playerAName.text) || string.IsNullOrEmpty(_playerBName.text))
            {
                Debug.Log("<color=red>Player names are required</color>");
                return;
            }

            if (_playerAName.text.Length < 3 || _playerBName.text.Length < 3)
            {
                Debug.Log("<color=red>Player names must be atleast of 3 characters</color>");
                return;
            }

            GameStates.Instance.CurrentState = GameStates.States.GAME;
        }
    }
}