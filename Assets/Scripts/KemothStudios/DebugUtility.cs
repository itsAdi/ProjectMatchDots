using System.Diagnostics;
using Debug = UnityEngine.Debug;

namespace KemothStudios.Utility
{
    public static class DebugUtility
    {
        [Conditional("DEBUG")]
        public static void LogError(string message)
        {
                Debug.LogError(message);
        }

        [Conditional("DEBUG")]
        public static void LogColored(string colorName, string message)
        {
            Debug.Log($"<color={colorName}>{message}</color>");
        }
    }
}