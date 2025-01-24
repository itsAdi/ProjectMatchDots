using UnityEngine;

namespace KemothStudios
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            GameStates.Instance.CurrentState = GameStates.States.MAIN_MENU;
        }
    }
}