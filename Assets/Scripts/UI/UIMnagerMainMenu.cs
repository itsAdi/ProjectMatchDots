using System;
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
        private int[] _playerAvatarIndices = new int[] { 0, 1 }; // Indices of the selected playar avatar
        private int _enabledPlayareAvatarTab; // actually the index to use in _playerAvatarIndices
        private bool _updatingAvatarsView;

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
                    _enabledPlayareAvatarTab = 0;
                    UpdateAvatarsView();
                }
            });
            _playerBAvatarTab.RegisterValueChangedCallback(x =>
            {
                if (x.newValue)
                {
                    _enabledPlayareAvatarTab = 1;
                    UpdateAvatarsView();
                }
            });

            if (_playerAvatars.GetAvatarsCount >= 2) // Checking for more than 2 because we have a 2 player setup and we need atleast 2 avatars in collection
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
                    int index = i;
                    r.RegisterValueChangedCallback(x =>
                    {
                        if (x.newValue)
                        {
                            if (!_updatingAvatarsView)
                                _playerAvatarIndices[_enabledPlayareAvatarTab] = index;
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
        }

        private void UpdateAvatarsView()
        {
            if (_playerAvatarControls != null)
            {
                _updatingAvatarsView = true;
                int index = _playerAvatarIndices[_enabledPlayareAvatarTab];
                _playerAvatarControls[index].SetEnabled(true);
                _playerAvatarControls[index].value = true;
                foreach (int iteratedIndex in _playerAvatarIndices)
                {
                    if(iteratedIndex != index) _playerAvatarControls[iteratedIndex].SetEnabled(false);
                }
            }
        }

        private void AssignSelectedAvatarIndex(int index, ref int avatarIndexForEnabledTab) => avatarIndexForEnabledTab = index;

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
            Player playerA = new(_playerAName.text, _playerAvatarIndices[0]);
            Player playerB = new(_playerBName.text, _playerAvatarIndices[1]);
            _gameData.InitializePlayerData(playerA, playerB);

            GameStates.Instance.CurrentState = GameStates.States.GAME;
        }
    }
}