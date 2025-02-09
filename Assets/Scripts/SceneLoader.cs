using System;
using KemothStudios.Utility;
using KemothStudios.Utility.Events;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;

namespace KemothStudios
{
    public class SceneLoader : MonoBehaviour
    {
        [SerializeField, Tooltip("Scenes that will never unload")] private SceneField[] _defaultScenes;
        [SerializeField] SerializableDictionary<GameStates.States, SceneField[]> scenes;

        private GameStates.States _scenesToLoad, _scenesToUnload;
        private EventBinding<RestartSceneEvent> _restartSceneBinding;
        private EventBinding<LoadingScreenTransitionCompleteEvent> _loadingScreenTransitionCompleteBinding;

        // Used for the case where this is the first time we are loading the game and loading screen is already visible...
        private bool _firstLoadDone;

        private void Start()
        {
            GameStates.Instance.GameStateChanged += StartChangeScene;
            if (GameStates.Instance.CurrentState != GameStates.States.NONE)
                StartChangeScene(GameStates.Instance.CurrentState);
            _restartSceneBinding = new EventBinding<RestartSceneEvent>(RestartCurrentSceneWrapper);
            EventBus<RestartSceneEvent>.RegisterBinding(_restartSceneBinding);
        }

        private void OnDestroy()
        {
            GameStates.Instance.GameStateChanged -= StartChangeScene;
            EventBus<RestartSceneEvent>.UnregisterBinding(_restartSceneBinding);
        }

        // Wrapper method for RestartCurrentScene because it returns an UniTaskVoid and we cannot use it in our event binding
        private void RestartCurrentSceneWrapper() => RestartCurrentScene().Forget();
        
        private async UniTaskVoid RestartCurrentScene()
        {
            Statics.Assert(()=>_firstLoadDone, "Found nothing reload");
            try
            {
                _scenesToLoad = GameStates.Instance.CurrentState;
                
                bool loadingScreenTransitionCompleted = false;
                EventBus<LoadingScreenTransitionCompleteEvent>.RegisterBindingOnce(new EventBinding<LoadingScreenTransitionCompleteEvent>(() => loadingScreenTransitionCompleted = true));
                EventBus<ShowLoadingScreenEvent>.RaiseEvent(new ShowLoadingScreenEvent());
                
                while (!loadingScreenTransitionCompleted)
                {
                    await UniTask.Yield();
                }

                await UnloadScenes();
                await LoadScenes();

                EventBus<LoadingScreenTransitionCompleteEvent>.RegisterBindingOnce(new EventBinding<LoadingScreenTransitionCompleteEvent>(() => EventBus<SceneLoadingCompleteEvent>.RaiseEvent(new SceneLoadingCompleteEvent())));
                EventBus<HideLoadingScreenEvent>.RaiseEvent(new HideLoadingScreenEvent());
            }
            catch (Exception e)
            {
                DebugUtility.LogException(e);
            }
        }

        private async void StartChangeScene(GameStates.States currentState)
        {
            try
            {
                _scenesToLoad = currentState;
                if (_firstLoadDone)
                {
                    bool loadingScreenTransitionCompleted = false;
                    EventBus<LoadingScreenTransitionCompleteEvent>.RegisterBindingOnce(new EventBinding<LoadingScreenTransitionCompleteEvent>(() => loadingScreenTransitionCompleted = true));
                    EventBus<ShowLoadingScreenEvent>.RaiseEvent(new ShowLoadingScreenEvent());
                    while (!loadingScreenTransitionCompleted)
                    {
                        await UniTask.Yield();
                    }
                }
                else
                {
                    _firstLoadDone = true;
                    
                    // load default scene here because we are loading game for the first time
                    foreach (SceneField sceneName in _defaultScenes)
                    {
                        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
                        while (!op.isDone) await UniTask.Yield();
                    }
                }

                await UnloadScenes();
                await LoadScenes();

                EventBus<LoadingScreenTransitionCompleteEvent>.RegisterBindingOnce(new EventBinding<LoadingScreenTransitionCompleteEvent>(() => EventBus<SceneLoadingCompleteEvent>.RaiseEvent(new SceneLoadingCompleteEvent())));
                EventBus<HideLoadingScreenEvent>.RaiseEvent(new HideLoadingScreenEvent());
            }
            catch (Exception e)
            {
                DebugUtility.LogException(e);
            }
        }

        private async UniTask LoadScenes()
        {
            if (_scenesToLoad is GameStates.States.NONE) return;
            if (scenes.TryGetValue(_scenesToLoad, out SceneField[] scenesToLoad))
            {
                foreach (SceneField sceneName in scenesToLoad)
                {
                    AsyncOperation op = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
                    while (!op.isDone) await UniTask.Yield();
                }
            }

            _scenesToUnload = _scenesToLoad;
            _scenesToLoad = GameStates.States.NONE;
        }

        private async UniTask UnloadScenes()
        {
            if (_scenesToUnload is GameStates.States.NONE) return;
            foreach (SceneField scenes in scenes[_scenesToUnload])
            {
                AsyncOperation op = SceneManager.UnloadSceneAsync(scenes);
                while (!op.isDone) await UniTask.Yield();
            }

            _scenesToUnload = GameStates.States.NONE;
        }
    }
}