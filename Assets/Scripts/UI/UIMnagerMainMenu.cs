using System.Collections;
using KemothStudios.Utility.Events;
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
        private RadioButton _playerAAvatarTab, _playerBAvatarTab;
        private int[] _playerAvatarIndices = new int[] { 0, 1 }; // Indices of the selected player avatar
        private int _enabledPlayerAvatarTab; // actually the index to use in _playerAvatarIndices
        private bool _updatingAvatarsView;
        private Button _settingsCog;
        private ShowMessageEvent _showMessageEvent;

        private void Start()
        {
            _playerAName = _uiDoument.rootVisualElement.Q<TextField>("playerAName");
            _playerBName = _uiDoument.rootVisualElement.Q<TextField>("playerBName");
            _startGameButton = _uiDoument.rootVisualElement.Q<Button>("startGameButton");

            // Setting up player avatar tabs
            UQueryBuilder<RadioButton> q = _uiDoument.rootVisualElement.Query<RadioButton>("playerIconTab");
            _playerAAvatarTab = q.AtIndex(0);
            _playerBAvatarTab = q.AtIndex(1);
            _playerAAvatarTab.RegisterValueChangedCallback(x =>
            {
                if (x.newValue)
                {
                    _enabledPlayerAvatarTab = 0;
                    UpdateAvatarsView();
                    EventBus<MinorButtonClickedEvent>.RaiseEvent(new MinorButtonClickedEvent());
                }
            });
            _playerBAvatarTab.RegisterValueChangedCallback(x =>
            {
                if (x.newValue)
                {
                    _enabledPlayerAvatarTab = 1;
                    UpdateAvatarsView();
                    EventBus<MinorButtonClickedEvent>.RaiseEvent(new MinorButtonClickedEvent());
                }
            });

            _settingsCog = _uiDoument.rootVisualElement.LookFor<Button>("settingsButton", "Failed to found settings button");
            _settingsCog.RegisterCallback<ClickEvent>(ShowSettings);

            if (_playerAvatars.GetAvatarsCount >= 2) // Checking for more than 2 because we have a 2 player setup and we need atleast 2 avatars in collection
            {
                _playerAvatarControls = new RadioButton[_playerAvatars.GetAvatarsCount];

                VisualElement grid = _uiDoument.rootVisualElement.Query<VisualElement>("iconsGrid");
                VisualElement group = grid.Q<GroupBox>();
                grid.Q<ScrollView>().Add(group);
                for (int i = 0; i < _playerAvatars.GetAvatarsCount; i++)
                {
                    RadioButton r = new();
                    r.AddToClassList("gridIcon");
                    if (_playerAvatars.TryGetAvatarAtIndex(i, out Sprite avatarImage))
                    {
                        r.Q(className: "unity-radio-button__checkmark-background").style.backgroundImage = new StyleBackground(avatarImage);
                    }

                    int index = i;
                    r.RegisterValueChangedCallback(x =>
                    {
                        if (x.newValue)
                        {
                            if (!_updatingAvatarsView)
                            {
                                _playerAvatarIndices[_enabledPlayerAvatarTab] = index;
                                EventBus<AvatarClickedEvent>.RaiseEvent(new AvatarClickedEvent());
                            }

                            _updatingAvatarsView = false;
                        }
                    });
                    _playerAvatarControls[i] = r;
                    group.Add(r);
                }

                UpdateAvatarsView();
            }

            _startGameButton.clicked += StartGame;
        }

        private void OnDestroy()
        {
            _startGameButton.clicked -= StartGame;
            _settingsCog.UnregisterCallback<ClickEvent>(ShowSettings);
        }

        private void ShowSettings(ClickEvent evt)
        {
            EventBus<ShowSettingsEvent>.RaiseEvent(new ShowSettingsEvent());
            EventBus<MinorButtonClickedEvent>.RaiseEvent(new MinorButtonClickedEvent());
        }

        private void UpdateAvatarsView()
        {
            if (_playerAvatarControls != null)
            {
                _updatingAvatarsView = true;
                int index = _playerAvatarIndices[_enabledPlayerAvatarTab];
                _playerAvatarControls[index].SetEnabled(true);
                _playerAvatarControls[index].value = true;
                foreach (int iteratedIndex in _playerAvatarIndices)
                {
                    if (iteratedIndex != index) _playerAvatarControls[iteratedIndex].SetEnabled(false);
                }
            }
        }

        private void StartGame()
        {
            if (Application.isMobilePlatform)
                StartCoroutine(HideKeyboard());
            EventBus<MajorButtonClickedEvent>.RaiseEvent(new MajorButtonClickedEvent());
            if (string.IsNullOrEmpty(_playerAName.text) || string.IsNullOrEmpty(_playerBName.text))
            {
                _showMessageEvent.Message = "Player names are required";
                EventBus<ShowMessageEvent>.RaiseEvent(_showMessageEvent);
                return;
            }

            if (_playerAName.text.Length < 3 || _playerBName.text.Length < 3)
            {
                _showMessageEvent.Message = "Player names must be at least 3 characters";
                EventBus<ShowMessageEvent>.RaiseEvent(_showMessageEvent);
                return;
            }

            Player playerA = new(_playerAName.text, _playerAvatarIndices[0], 0, _gameData);
            Player playerB = new(_playerBName.text, _playerAvatarIndices[1], 1, _gameData);
            _gameData.InitializePlayerData(playerA, playerB);

            GameStates.Instance.CurrentState = GameStates.States.GAME;
        }

        private IEnumerator HideKeyboard()
        {
            yield return new WaitForEndOfFrame();
            _playerAName.Blur();
            _playerBName.Blur();
            if (TouchScreenKeyboard.visible)
            {
                var k = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default, false, false, false, false);
                k.active = false;
                k = null;
            }
        }
    }
}