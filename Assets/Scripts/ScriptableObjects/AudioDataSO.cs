using System;
using KemothStudios.Utility.Events;
using KemothStudios.Utility.SaveSystem;
using UnityEngine;

namespace KemothStudios
{
    [CreateAssetMenu(menuName = "KemothStudios/Audio Data", fileName = "AudioData")]
    public class AudioDataSO : ScriptableObject
    {
        [SerializeField, Range(0f, 1f)] float _defaultGameAudioVolume;
        [SerializeField, Range(0f, 1f)] float _defaultUIAudioVolume;

        private AudioData _audioData;

        public float GameAudioVolume
        {
            get => _audioData.GameAudioVolume;
            set => _audioData.GameAudioVolume = value;
        }

        public float UIAudioVolume
        {
            get => _audioData.UIAudioVolume;
            set => _audioData.UIAudioVolume = value;
        }

        public float DefaultGameAudioVolume => _defaultGameAudioVolume;
        public float DefaultUIAudioVolume => _defaultUIAudioVolume;

        public void SetAudioData(AudioData audioData) => _audioData = audioData;
    }

    [Serializable]
    public class AudioData : ISaveData
    {
        public string FileName => "AudioData";
        [SerializeField] float _gameAudioVolume;
        [SerializeField] float _uIAudioVolume;

        public float GameAudioVolume
        {
            get => _gameAudioVolume;
            set => _gameAudioVolume = value;
        }

        public float UIAudioVolume
        {
            get => _uIAudioVolume;
            set => _uIAudioVolume = value;
        }

        public AudioData(float gameAudioVolume, float uiAudioVolume)
        {
            _gameAudioVolume = gameAudioVolume;
            _uIAudioVolume = uiAudioVolume;
        }
    }
}