using System.Collections.Generic;
using KemothStudios.Board;
using KemothStudios.Utility.Events;
using UnityEngine;

namespace KemothStudios
{
    public class GameAudio : MonoBehaviour
    {
        [SerializeField] private AudioSource _gameAudio;
        [SerializeField] private AudioSource _uiAudio;

        [Header("Audio Clips")]
        [SerializeField] private AudioClip _majorButtonClick;
        [SerializeField] private AudioClip _minorButtonClick;
        [SerializeField] private AudioClip _avatarSelection;
        [SerializeField] private AudioClip _drawLine;
        [SerializeField] private AudioClip _victory;
        [SerializeField] private AudioClip[] _cellsCompleted;

        private EventBinding<MajorButtonClickedEvent> _majorButtonClickEventBinding;
        private EventBinding<MinorButtonClickedEvent> _minorButtonClickEventBinding;
        private EventBinding<AvatarClickedEvent> _avatarSelectionEventBinding;
        private EventBinding<DrawLineEvent> _drawLineEventBinding;
        private EventBinding<CellAcquiredEvent> _cellCompletedEventBinding;
        private EventBinding<ShowVictoryEvent> _victoryEventBinding;

        private int _cellsCompletedClipsCount;

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

            _cellsCompletedClipsCount = _cellsCompleted.Length;
            if (_cellsCompletedClipsCount == 0)
                Debug.Log("<color=yellow>No Draw-Line audio clips provided</color>");
            else
            {
                _cellCompletedEventBinding = new EventBinding<CellAcquiredEvent>(PlayCellCompletedAudio);
                EventBus<CellAcquiredEvent>.RegisterBinding(_cellCompletedEventBinding);
            }

            _victoryEventBinding = new EventBinding<ShowVictoryEvent>(PlayVictoryAudio);
            EventBus<ShowVictoryEvent>.RegisterBinding(_victoryEventBinding);
            
            _gameAudio.Play();
        }

        private void OnDestroy()
        {
            EventBus<MajorButtonClickedEvent>.UnregisterBinding(_majorButtonClickEventBinding);
            EventBus<MinorButtonClickedEvent>.UnregisterBinding(_minorButtonClickEventBinding);
            EventBus<AvatarClickedEvent>.UnregisterBinding(_avatarSelectionEventBinding);
            EventBus<DrawLineEvent>.UnregisterBinding(_drawLineEventBinding);
            if(_cellsCompletedClipsCount > 0)
                EventBus<CellAcquiredEvent>.UnregisterBinding(_cellCompletedEventBinding);
            EventBus<ShowVictoryEvent>.UnregisterBinding(_victoryEventBinding);
        }

        private void PlayDrawLineAudio(DrawLineEvent lineData)
        {
            using IEnumerator<Cell> cell = lineData.Line.SharedCells.GetEnumerator();
            if (cell.MoveNext() && cell.Current != null && !cell.Current.IsCellCompleted)
                PlayUIAudioClip(_drawLine);
        }
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