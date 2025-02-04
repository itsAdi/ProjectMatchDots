using UnityEngine;

namespace KemothStudios
{
    public class GameManager : MonoBehaviour
    {
        private void Start()
        {
            GameStates.Instance.CurrentState = GameStates.States.MAIN_MENU;
        }
    }
}