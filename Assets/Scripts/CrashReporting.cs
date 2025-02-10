using Cysharp.Threading.Tasks;
using Firebase;
using Firebase.Crashlytics;
using KemothStudios.Utility.Events;

namespace KemothStudios
{
    public sealed class CrashReporting
    {
        // Initialize Firebase
        public void Initialize()
        {
            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => StartGame(task.Result));
        }

        private async UniTaskVoid StartGame(DependencyStatus dependencyStatus)
        {
            await UniTask.SwitchToMainThread();
            if (dependencyStatus == DependencyStatus.Available)
            {
                // When this property is set to true, Crashlytics will report all
                // uncaught exceptions as fatal events. This is the recommended behavior.
                Crashlytics.ReportUncaughtExceptionsAsFatal = true;
                // Crashlytics started, start the game
                GameStates.Instance.CurrentState = GameStates.States.MAIN_MENU;
            }
            else
            {
                ShowMessageEvent showMessageEvent = new ShowMessageEvent
                {
                    Message = $"Could not resolve all Firebase dependencies: {dependencyStatus}"
                };
                EventBus<ShowMessageEvent>.RaiseEvent(showMessageEvent);
            }
        }
    }
}