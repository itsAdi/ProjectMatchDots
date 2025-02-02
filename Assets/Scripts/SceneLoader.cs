using System;
using KemothStudios.Utility;
using System.Threading.Tasks;
using KemothStudios.Utility.Events;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace KemothStudios
{
    public class SceneLoader : MonoBehaviour
    {
        [SerializeField] SerializableDictionary<GameStates.States, SceneField[]> scenes;
        [SerializeField] private UIDocument loadingScreen;

        private GameStates.States _scenesToLoad, _scenesToUnload;
        private VisualElement _loadingScreenVisualElement;

        // Used for the case where this is the first time we are loading the game and loading screen is already visible...
        private bool _firstLoadDone;

        private void Awake()
        {
            _loadingScreenVisualElement = loadingScreen.rootVisualElement.Q("background");
        }

        private void Start()
        {
            GameStates.Instance.GameStateChanged += StartChangeScene;
            _loadingScreenVisualElement.AddToClassList(Statics.COMMON_CSS_HIDE_LONG);
            _loadingScreenVisualElement.AddToClassList(Statics.COMMON_CSS_SHOW_LONG);
            if (GameStates.Instance.CurrentState != GameStates.States.NONE)
                StartChangeScene(GameStates.Instance.CurrentState);
        }

        private async void StartChangeScene(GameStates.States currentState)
        {
            _scenesToLoad = currentState;
            _loadingScreenVisualElement.pickingMode = PickingMode.Position;
            if (_firstLoadDone)
            {
                bool loadingScreenTransitionCompleted = false;
                _loadingScreenVisualElement.RegisterCallbackOnce<TransitionEndEvent>(_ => loadingScreenTransitionCompleted = true);
                _loadingScreenVisualElement.AddToClassList(Statics.COMMON_CSS_SHOW_LONG);
                while (!loadingScreenTransitionCompleted)
                {
                    await Task.Yield();
                }
            }
            else
            {
                _firstLoadDone = true;
            }

            await UnloadScenes();
            await LoadScenes();

            _loadingScreenVisualElement.RegisterCallbackOnce<TransitionEndEvent>(_ =>
            {
                _loadingScreenVisualElement.pickingMode = PickingMode.Ignore;
                EventBus<SceneLoadingCompleteEvent>.RaiseEvent(new SceneLoadingCompleteEvent());
            });
            _loadingScreenVisualElement.RemoveFromClassList(Statics.COMMON_CSS_SHOW_LONG);
        }

        private async Task LoadScenes()
        {
            if (_scenesToLoad is GameStates.States.NONE) return;
            if (scenes.TryGetValue(_scenesToLoad, out SceneField[] scenesToLoad))
            {
                foreach (SceneField sceneName in scenesToLoad)
                {
                    AsyncOperation op = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
                    while (!op.isDone) await Task.Yield();
                }
            }

            _scenesToUnload = _scenesToLoad;
            _scenesToLoad = GameStates.States.NONE;
        }

        private async Task UnloadScenes()
        {
            if (_scenesToUnload is GameStates.States.NONE) return;
            foreach (SceneField scenes in scenes[_scenesToUnload])
            {
                AsyncOperation op = SceneManager.UnloadSceneAsync(scenes);
                while (!op.isDone) await Task.Yield();
            }

            _scenesToUnload = GameStates.States.NONE;
        }
    }
}