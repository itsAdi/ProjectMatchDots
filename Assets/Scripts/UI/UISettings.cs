using System;
using KemothStudios.Utility;
using KemothStudios.Utility.Events;
using UnityEngine;
using UnityEngine.UIElements;
using static KemothStudios.Statics;

namespace KemothStudios
{
    public class UISettings : MonoBehaviour
    {
        [SerializeField] private VisualTreeAsset _settingsUIAsset;

        [SerializeField, Tooltip("Reference to ui document which will hold the settings UI")]
        private UIDocument _uiDocumment;

        [SerializeField] private AudioDataSO _audioData;

        private bool _isInitialized;
        private VisualElement _windowBG;
        private VisualElement _window;
        private Slider _gameAudioVolumeSlider, _uiAudioVolumeSlider;
        private Toggle _gameAudioMuteToggle, _uiAudioMuteToggle;
        private GameAudioVolumeChangedEvent _gameAudioVolumeChangedEvent;
        private UIAudioVolumeChangedEvent _uiAudioVolumeChangedEvent;
        private GameAudioMuteChangedEvent _gameAudioMuteChangedEvent;
        private UIAudioMuteChangedEvent _uiAudioMuteChangedEvent;
        private EventBinding<ShowSettingsEvent> _showSettingsEvent;
        private EventBinding<HideSettingsEvent> _hideSettingsEvent;
        private Button _closeSettingsButton;

        private void Start()
        {
            if (_isInitialized) return;
            try
            {
                Assert(() => _uiDocumment != null, "Parent UI document is not assigned");
                Assert(() => _audioData != null, "Audio data is not assigned");
                Assert(() => _settingsUIAsset != null, "Settings UI asset is not assigned");
                TemplateContainer root = _settingsUIAsset.Instantiate();
                _windowBG = root.GetVisualElement("bg", "Unable to create UI bg element");
                _window = root.GetVisualElement("settingsWindow", "Unable to create UI window element");

                // because we will instantiate and extract first child from root and move them to parent, this will not fire any event so we
                // are using scheduler to add required classes after one frame
                _uiDocumment.rootVisualElement.schedule.Execute(() =>
                {
                    _windowBG.AddToClassList(COMMON_CSS_HIDE_SHORT);
                    _window.AddToClassList("settingsWindowDown");
                    _windowBG.pickingMode = PickingMode.Ignore;
                }).ExecuteLater(0); // this line will call scheduler after one frame

                // moving child out of root will remove css links,add them to parent
                VisualElementStyleSheetSet styleSheetSet = root.styleSheets;
                int count = styleSheetSet.count;
                for (int i = 0; i < count; i++)
                {
                    _uiDocumment.rootVisualElement.styleSheets.Add(styleSheetSet[i]);
                }

                _closeSettingsButton = root.GetVisualElement("done", "Visual element named done is not found in settings UI").GetVisualElement<Button>("No button found inside visual element named done");
                _closeSettingsButton.clicked += RequestToHideSettings;

                VisualElement bgMusic = root.GetVisualElement("bgMusic", "Visual element named bgMusic is not found in settings UI");
                VisualElement uiSound = root.GetVisualElement("uiSound", "Visual element named uiSound is not found in settings UI");

                _gameAudioMuteToggle = bgMusic.GetVisualElement<Toggle>("Game audio mute toggle isn not fount");
                _uiAudioMuteToggle = uiSound.GetVisualElement<Toggle>("UI audio mute toggle is not fount");
                _gameAudioMuteToggle.SetValueWithoutNotify(Mathf.Approximately(_audioData.GameAudioVolume, 0f));
                _uiAudioMuteToggle.SetValueWithoutNotify(Mathf.Approximately(_audioData.UIAudioVolume, 0f));
                _gameAudioMuteToggle.RegisterValueChangedCallback(GameAudioMuteStateChanged);
                _uiAudioMuteToggle.RegisterValueChangedCallback(UIAudioMuteStateChanged);

                _gameAudioVolumeSlider = bgMusic.GetVisualElement<Slider>("Game audio volume slider is not found");
                _uiAudioVolumeSlider = uiSound.GetVisualElement<Slider>("UI audio volume slider is not found");
                _gameAudioVolumeSlider.SetValueWithoutNotify(_audioData.GameAudioVolume);
                _uiAudioVolumeSlider.SetValueWithoutNotify(_audioData.UIAudioVolume);
                _gameAudioVolumeSlider.RegisterValueChangedCallback(GameAudioVolumeChanged);
                _uiAudioVolumeSlider.RegisterValueChangedCallback(UIAudioVolumeChanged);

                _uiDocumment.rootVisualElement.Add(root.ElementAt(0));
                root.RemoveFromHierarchy();

                _showSettingsEvent = new EventBinding<ShowSettingsEvent>(ShowSettings);
                EventBus<ShowSettingsEvent>.RegisterBinding(_showSettingsEvent);
                _hideSettingsEvent = new EventBinding<HideSettingsEvent>(HideSettings);
                EventBus<HideSettingsEvent>.RegisterBinding(_hideSettingsEvent);

                _windowBG.pickingMode = PickingMode.Ignore;
                
                _isInitialized = true;
            }
            catch (Exception _)
            {
                DebugUtility.LogColored("red", "Initializing SettingsUI failed !!!");
                throw;
            }
        }

        private void OnDestroy()
        {
            if (IsInitialized)
            {
                _closeSettingsButton.clicked -= RequestToHideSettings;
                EventBus<ShowSettingsEvent>.UnregisterBinding(_showSettingsEvent);
                EventBus<HideSettingsEvent>.UnregisterBinding(_hideSettingsEvent);
                _gameAudioVolumeSlider.UnregisterValueChangedCallback(GameAudioVolumeChanged);
                _uiAudioVolumeSlider.UnregisterValueChangedCallback(UIAudioVolumeChanged);
                _gameAudioMuteToggle.RegisterValueChangedCallback(GameAudioMuteStateChanged);
                _uiAudioMuteToggle.RegisterValueChangedCallback(UIAudioMuteStateChanged);
            }
        }

        private void RequestToHideSettings()
        {
            EventBus<HideSettingsEvent>.RaiseEvent(new HideSettingsEvent());
            EventBus<MajorButtonClickedEvent>.RaiseEvent(new MajorButtonClickedEvent());
        }

        private void ShowSettings()
        {
            if (IsInitialized)
            {
                _windowBG.pickingMode = PickingMode.Position;
                _windowBG.AddToClassList(COMMON_CSS_SHOW_SHORT);
                _window.AddToClassList("settingsWindowUp");
            }
        }

        private void HideSettings()
        {
            if (IsInitialized)
            {
                _windowBG.pickingMode = PickingMode.Ignore;
                _windowBG.RemoveFromClassList(COMMON_CSS_SHOW_SHORT);
                _window.RemoveFromClassList("settingsWindowUp");
            }
        }

        private void GameAudioMuteStateChanged(ChangeEvent<bool> v)
        {
            float finalVolume = v.newValue ? 0f : _audioData.DefaultGameAudioVolume;
            _audioData.GameAudioVolume = finalVolume;
            _gameAudioVolumeSlider.SetValueWithoutNotify(finalVolume);
            _gameAudioMuteChangedEvent.Mute = v.newValue;
            EventBus<GameAudioMuteChangedEvent>.RaiseEvent(_gameAudioMuteChangedEvent);
        }

        private void UIAudioMuteStateChanged(ChangeEvent<bool> v)
        {
            float finalVolume = v.newValue ? 0f : _audioData.DefaultUIAudioVolume;
            _audioData.UIAudioVolume = finalVolume;
            _uiAudioVolumeSlider.SetValueWithoutNotify(finalVolume);
            _uiAudioMuteChangedEvent.Muted = v.newValue;
            EventBus<UIAudioMuteChangedEvent>.RaiseEvent(_uiAudioMuteChangedEvent);
        }

        private void GameAudioVolumeChanged(ChangeEvent<float> v)
        {
            float value = v.newValue;
            _audioData.GameAudioVolume = value;
            _gameAudioVolumeChangedEvent.Volume = value;
            EventBus<GameAudioVolumeChangedEvent>.RaiseEvent(_gameAudioVolumeChangedEvent);
            if (Mathf.Approximately(value, 0f))
                _gameAudioMuteToggle.SetValueWithoutNotify(true);
            else if(_gameAudioMuteToggle.value) _gameAudioMuteToggle.SetValueWithoutNotify(false);
        }

        private void UIAudioVolumeChanged(ChangeEvent<float> v)
        {
            float value = v.newValue;
            _audioData.UIAudioVolume = value;
            _uiAudioVolumeChangedEvent.Volume = value;
            EventBus<UIAudioVolumeChangedEvent>.RaiseEvent(_uiAudioVolumeChangedEvent);
            if (Mathf.Approximately(value, 0f))
                _uiAudioMuteToggle.SetValueWithoutNotify(true);
            else if (_uiAudioMuteToggle.value) _uiAudioMuteToggle.SetValueWithoutNotify(false);
        }

        private bool IsInitialized
        {
            get
            {
                if (!_isInitialized)
                    DebugUtility.LogColored("red", "UI settings not initialized");
                return _isInitialized;
            }
        }
    }
}