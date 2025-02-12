using System;
using System.Collections.Generic;
using KemothStudios.Board;
using KemothStudios.Utility;
using KemothStudios.Utility.Events;
using KemothStudios.Utility.SaveSystem;
using UnityEngine;
using Random = UnityEngine.Random;

namespace KemothStudios
{
    public class GameAudio : MonoBehaviour
    {
        [SerializeField] private AudioSource _gameAudio;
        [SerializeField] private AudioSource _uiAudio;
        [SerializeField] private AudioDataSO _audioData;

        [Header("Audio Clips")] [SerializeField]
        private AudioClip _majorButtonClick;

        [SerializeField] private AudioClip _minorButtonClick;
        [SerializeField] private AudioClip _avatarSelection;
        [SerializeField] private AudioClip _drawLine;
        [SerializeField] private AudioClip _victory;
        [SerializeField] private AudioClip _timerUp;
        [SerializeField] private AudioClip[] _cellsCompleted;

        private EventBinding<MajorButtonClickedEvent> _majorButtonClickEventBinding;
        private EventBinding<MinorButtonClickedEvent> _minorButtonClickEventBinding;
        private EventBinding<AvatarClickedEvent> _avatarSelectionEventBinding;
        private EventBinding<DrawLineEvent> _drawLineEventBinding;
        private EventBinding<CellAcquiredEvent> _cellCompletedEventBinding;
        private EventBinding<ShowVictoryEvent> _victoryEventBinding;
        private EventBinding<HideSettingsEvent> _hideSettingsEventBinding;
        private EventBinding<GameAudioVolumeChangedEvent> _gameAudioVolumeChangedEventBinding;
        private EventBinding<UIAudioVolumeChangedEvent> _uiAudioVolumeChangedEventBinding;
        private EventBinding<GameAudioMuteChangedEvent> _gameAudioMuteChangedEventBinding;
        private EventBinding<UIAudioMuteChangedEvent> _uiAudioMuteChangedEventBinding;
        private EventBinding<TurnTimerElapsedEvent> _turnTimerElapsedEventBinding;

        private int _cellsCompletedClipsCount;
        private FileDataService<AudioData, JSONSerializer<AudioData>> _audioDataFileService;

        private void Start()
        {
            _majorButtonClickEventBinding = new EventBinding<MajorButtonClickedEvent>(PlayMajorButtonClickAudio);
            EventBus<MajorButtonClickedEvent>.RegisterBinding(_majorButtonClickEventBinding);

            _minorButtonClickEventBinding = new EventBinding<MinorButtonClickedEvent>(PlayMinorButtonClickedAudio);
            EventBus<MinorButtonClickedEvent>.RegisterBinding(_minorButtonClickEventBinding);

            _avatarSelectionEventBinding = new EventBinding<AvatarClickedEvent>(PlayAvatarSelectionAudio);
            EventBus<AvatarClickedEvent>.RegisterBinding(_avatarSelectionEventBinding);

            _drawLineEventBinding = new EventBinding<DrawLineEvent>(PlayDrawLineAudio);
            EventBus<DrawLineEvent>.RegisterBinding(_drawLineEventBinding);

            _hideSettingsEventBinding = new EventBinding<HideSettingsEvent>(SaveAudioData);
            EventBus<HideSettingsEvent>.RegisterBinding(_hideSettingsEventBinding);
            
            _gameAudioMuteChangedEventBinding = new EventBinding<GameAudioMuteChangedEvent>(UpdateGameAudioMuteState);
            EventBus<GameAudioMuteChangedEvent>.RegisterBinding(_gameAudioMuteChangedEventBinding);
            
            _gameAudioVolumeChangedEventBinding = new EventBinding<GameAudioVolumeChangedEvent>(UpdateGameAudioVolume);
            EventBus<GameAudioVolumeChangedEvent>.RegisterBinding(_gameAudioVolumeChangedEventBinding);
            
            _uiAudioMuteChangedEventBinding = new EventBinding<UIAudioMuteChangedEvent>(UpdateUIAudioMuteState);
            EventBus<UIAudioMuteChangedEvent>.RegisterBinding(_uiAudioMuteChangedEventBinding);
            
            _uiAudioVolumeChangedEventBinding = new EventBinding<UIAudioVolumeChangedEvent>(UpdateUIAudioVolume);
            EventBus<UIAudioVolumeChangedEvent>.RegisterBinding(_uiAudioVolumeChangedEventBinding);

            _turnTimerElapsedEventBinding = new EventBinding<TurnTimerElapsedEvent>(TurnTimerElapsed);
            EventBus<TurnTimerElapsedEvent>.RegisterBinding(_turnTimerElapsedEventBinding);

            _cellsCompletedClipsCount = _cellsCompleted.Length;
            if (_cellsCompletedClipsCount == 0)
                DebugUtility.LogColored("yellow", "No Draw-Line audio clips provided");
            else
            {
                _cellCompletedEventBinding = new EventBinding<CellAcquiredEvent>(PlayCellCompletedAudio);
                EventBus<CellAcquiredEvent>.RegisterBinding(_cellCompletedEventBinding);
            }

            _victoryEventBinding = new EventBinding<ShowVictoryEvent>(PlayVictoryAudio);
            EventBus<ShowVictoryEvent>.RegisterBinding(_victoryEventBinding);

            AudioData audioData = new AudioData(_audioData.DefaultGameAudioVolume, _audioData.DefaultUIAudioVolume);
            _audioDataFileService = new FileDataService<AudioData, JSONSerializer<AudioData>>(audioData, new JSONSerializer<AudioData>());

            if (!_audioDataFileService.DataExists())
            {
                _audioData.SetAudioData(audioData);
                _audioDataFileService.Save();
            }
            else
                _audioData.SetAudioData(_audioDataFileService.Load());

            _gameAudio.volume = _audioData.GameAudioVolume;
            _uiAudio.volume = _audioData.UIAudioVolume;
            _gameAudio.Play();
        }
        
        private void OnDestroy()
        {
            EventBus<MajorButtonClickedEvent>.UnregisterBinding(_majorButtonClickEventBinding);
            EventBus<MinorButtonClickedEvent>.UnregisterBinding(_minorButtonClickEventBinding);
            EventBus<AvatarClickedEvent>.UnregisterBinding(_avatarSelectionEventBinding);
            EventBus<DrawLineEvent>.UnregisterBinding(_drawLineEventBinding);
            if (_cellsCompletedClipsCount > 0)
                EventBus<CellAcquiredEvent>.UnregisterBinding(_cellCompletedEventBinding);
            EventBus<ShowVictoryEvent>.UnregisterBinding(_victoryEventBinding);
            EventBus<HideSettingsEvent>.UnregisterBinding(_hideSettingsEventBinding);
            EventBus<GameAudioMuteChangedEvent>.UnregisterBinding(_gameAudioMuteChangedEventBinding);
            EventBus<GameAudioVolumeChangedEvent>.UnregisterBinding(_gameAudioVolumeChangedEventBinding);
            EventBus<UIAudioMuteChangedEvent>.UnregisterBinding(_uiAudioMuteChangedEventBinding);
            EventBus<UIAudioVolumeChangedEvent>.UnregisterBinding(_uiAudioVolumeChangedEventBinding);
            EventBus<TurnTimerElapsedEvent>.UnregisterBinding(_turnTimerElapsedEventBinding);
        }

        private void SaveAudioData()
        {
            _audioDataFileService.Save(true);
        }

        private void PlayDrawLineAudio(DrawLineEvent lineData)
        {
            using IEnumerator<Cell> cell = lineData.Line.SharedCells.GetEnumerator();
            if (cell.MoveNext() && cell.Current != null && !cell.Current.IsCellCompleted)
                PlayUIAudioClip(_drawLine);
        }
        
        private void TurnTimerElapsed() => PlayUIAudioClip(_timerUp);
        private void UpdateGameAudioVolume(GameAudioVolumeChangedEvent value) => _gameAudio.volume = value.Volume;
        private void UpdateUIAudioVolume(UIAudioVolumeChangedEvent value) => _uiAudio.volume = value.Volume;
        private void UpdateGameAudioMuteState(GameAudioMuteChangedEvent value) => _gameAudio.volume = _audioData.GameAudioVolume;
        private void UpdateUIAudioMuteState(UIAudioMuteChangedEvent value) => _uiAudio.volume = _audioData.UIAudioVolume;
        private void PlayVictoryAudio() => PlayUIAudioClip(_victory);
        private void PlayCellCompletedAudio() => PlayUIAudioClip(_cellsCompleted[Random.Range(0, _cellsCompletedClipsCount)]);
        private void PlayMajorButtonClickAudio() => PlayUIAudioClip(_majorButtonClick);
        private void PlayAvatarSelectionAudio() => PlayUIAudioClip(_avatarSelection);
        private void PlayMinorButtonClickedAudio() => PlayUIAudioClip(_minorButtonClick);

        private void PlayUIAudioClip(AudioClip audioClip)
        {
            _uiAudio.Stop();
            _uiAudio.clip = audioClip;
            _uiAudio.Play();
        }
    }
}