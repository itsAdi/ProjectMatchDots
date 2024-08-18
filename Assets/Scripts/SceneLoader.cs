using KemothStudios.Utility;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace KemothStudios
{
    public class SceneLoader : MonoBehaviour
    {
        [SerializeField] SerializableDictionary<GameStates.States, SceneField[]> _scenes;

        private GameStates.States _loadedScenes;

        private void Start()
        {
            GameStates.Instance.GameStateChanged += ChangeScene;
            if(GameStates.Instance.CurrentState != GameStates.States.NONE)
                ChangeScene(GameStates.Instance.CurrentState);
        }

        private async void ChangeScene(GameStates.States currentState)
        {
            if (_loadedScenes is not GameStates.States.NONE)
            {
                await UnloadScenes();
            }

            LoadScenes(currentState);
        }

        private async void LoadScenes(GameStates.States scene)
        {
            _loadedScenes = scene;
            if (_scenes.TryGetValue(scene, out SceneField[] scenes))
            {
                foreach (SceneField sceneName in scenes)
                {
                    AsyncOperation op = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
                    while (!op.isDone) await Task.Yield();
                }
            }
        }

        private async Task UnloadScenes()
        {
            foreach (SceneField scenes in _scenes[_loadedScenes])
            {
                AsyncOperation op = SceneManager.UnloadSceneAsync(scenes);
                while (!op.isDone) await Task.Yield();
            }
            _loadedScenes = GameStates.States.NONE;
        }
    }
}