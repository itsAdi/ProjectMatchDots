using UnityEngine;

namespace KemothStudios
{
    public class GameManager : MonoBehaviour
    {
        private void Start()
        {
            new CrashReporting().Initialize();
        }
    }
}