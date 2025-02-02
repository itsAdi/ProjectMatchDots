using System;
using KemothStudios.Utility.Events;
using UnityEngine;

namespace KemothStudios
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        
        private EventBinding<SceneLoadingCompleteEvent> _sceneLoadComplete;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            _sceneLoadComplete = new EventBinding<SceneLoadingCompleteEvent>(StartGame);
            EventBus<SceneLoadingCompleteEvent>.RegisterBinding(_sceneLoadComplete);
            GameStates.Instance.CurrentState = GameStates.States.MAIN_MENU;
        }

        private void StartGame()
        {
            if(GameStates.Instance.CurrentState == GameStates.States.GAME)
                EventBus<GameStartedEvent>.RaiseEvent(new GameStartedEvent());
        }

        private void OnDestroy()
        {
            EventBus<SceneLoadingCompleteEvent>.UnregisterBinding(_sceneLoadComplete);
        }
    }
}